using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataLink;

namespace LINK
{
    public partial class Form1 : Form
    {
        #region Campos privados
        private TQS.Drawing2D.DrawingComponent dc; // Componente de desenho TQS.
        private Data data; // Dados do modelo.
        private readonly List<Button> navigationButtons = new List<Button>(); // Botões de navegação.
        private StatusStrip statusStrip; // Barra de status do rodapé.
        private ToolStripStatusLabel statusLabel; // Rótulo de status do rodapé.
        private RichTextBox reportBox; // Exibe o memorial.
        private Panel schematicPanel; // Painel do esquema 2D.
        private Button calculateButton; // Aciona o cálculo.
        private Button generateReportButton; // Gera o relatório.
        private TableLayoutPanel mainLayout;
        private Panel rightPanelHost; // Painel lateral para desenho ou resultados.
        private Panel view3DPanel; // Placeholder da visualização 3D.
        private bool linkTypeSelected; // Indica se já escolheu o tipo de ligação.
        private GeometryParameters geometryParams = new GeometryParameters(); // Parâmetros detalhados de geometria.
        private bool geometryInputsUpdating; // Evita laços durante sincronização de campos.
        private ComboBox beamProfileCombo;
        private ComboBox columnProfileCombo;
        private TextBox beamHeightBox;
        private TextBox beamFlangeBox;
        private TextBox beamWebBox;
        private TextBox beamFlangeWidthBox;
        private TextBox columnHeightBox;
        private TextBox columnFlangeBox;
        private TextBox columnWebBox;
        private TextBox columnFlangeWidthBox;
        private TextBox plateThicknessBox;
        private TextBox plateWidthBox;
        private TextBox plateEdgeHorizontalBox;
        private TextBox plateEdgeTopBox;
        private TextBox plateEdgeBottomBox;
        private TextBox boltCountBox;
        private TextBox boltSpacingBox;
        private TextBox boltEdgeTopBox;
        private TextBox boltGaugeBox;
        private TextBox boltDiameterBox;
        private TextBox weldSizeBox;
        private Panel sideViewPanel;
        private Panel frontViewPanel;
        private Helix3DHost view3DHost;
        private HelixSceneBuilder helixSceneBuilder;
        private Control geometryInputsLayout;
        private FlowLayoutPanel parametersHost;
        private static readonly IReadOnlyList<ProfileDefinition> ProfileLibrary = ProfileLibraryData.Profiles;
        #endregion

        public Form1()
        {
            InitializeComponent();
            EngineeringStyle.ApplyModernStyle(this);
            // Marcar campos de placeholder como usados para evitar avisos e inicializar contêiner de inputs
            _ = dc;
            parametersHost = new FlowLayoutPanel();
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            MinimumSize = new Size(1280, 800); // Garante que o layout principal não seja comprimido
            WindowState = FormWindowState.Maximized; // Inicia maximizado para melhor experiência
            data = new Data();
            helixSceneBuilder = new HelixSceneBuilder();
            ConfigureUI();
            ConfigureLayout();
            LoadGeometryToInputs();
        }

        private void ConfigureUI()
        {
            if (menuStrip1 != null)
            {
                menuStrip1.Dock = DockStyle.Top;
                // Menu principal com fonte e altura maiores (pedido do usuário)
                menuStrip1.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
                menuStrip1.AutoSize = false;
                menuStrip1.Height = 48;
                menuStrip1.Padding = new Padding(12, 10, 12, 10);
                MainMenuStrip = menuStrip1;
            }

            if (toolStrip1 != null)
            {
                toolStrip1.Items.Clear();
                toolStrip1.ImageScalingSize = new Size(32, 32);
                toolStrip1.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
                toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
                toolStrip1.RenderMode = ToolStripRenderMode.System;
                toolStrip1.Dock = DockStyle.Top;
                toolStrip1.AutoSize = false;
                toolStrip1.Height = 54;
                toolStrip1.Padding = new Padding(6, 6, 6, 6);

                toolStrip1.Items.Add(CreateIconButton("Geometria", LoadIcon("IconGeometria", SystemIcons.Information), OnToolbarStageClicked, "Geometria"));
                toolStrip1.Items.Add(CreateIconButton("Propriedades", LoadIcon("IconPropriedades", SystemIcons.Question), OnToolbarStageClicked, "Propriedades"));
                toolStrip1.Items.Add(CreateIconButton("Cargas", LoadIcon("IconCargas", SystemIcons.Warning), OnToolbarStageClicked, "Cargas"));
                toolStrip1.Items.Add(CreateIconButton("Critérios", LoadIcon("IconCriterios", SystemIcons.Shield), OnToolbarStageClicked, "Critérios"));
                toolStrip1.Items.Add(CreateIconButton("Resultados", LoadIcon("IconResultados", SystemIcons.Application), OnToolbarStageClicked, "Resultados"));

                toolStrip1.Items.Add(new ToolStripSeparator());

                toolStrip1.Items.Add(CreateIconButton("VD", LoadIcon("IconVD", SystemIcons.Warning), OnToolbarStageClicked, "Esforço cortante VD"));
                toolStrip1.Items.Add(CreateIconButton("ND", LoadIcon("IconND", SystemIcons.Shield), OnToolbarStageClicked, "Esforço normal ND"));
                toolStrip1.Items.Add(CreateIconButton("MD", LoadIcon("IconMD", SystemIcons.Information), OnToolbarStageClicked, "Momento MD"));

                toolStrip1.Items.Add(new ToolStripSeparator());
                toolStrip1.Items.Add(CreateIconButton("Calcular", LoadIcon("IconRun", SystemIcons.Exclamation), OnToolbarStageClicked, "Calcular"));

                toolStrip1.Items.Add(new ToolStripSeparator());
                toolStrip1.Items.Add(CreateIconButton("ZoomIn", LoadIcon("IconZoomIn", SystemIcons.Asterisk), OnToolbarStageClicked, "Zoom in"));
                toolStrip1.Items.Add(CreateIconButton("ZoomOut", LoadIcon("IconZoomOut", SystemIcons.Asterisk), OnToolbarStageClicked, "Zoom out"));
                toolStrip1.Items.Add(CreateIconButton("ZoomFit", LoadIcon("IconZoomFit", SystemIcons.Application), OnToolbarStageClicked, "Zoom extents"));

                toolStrip1.Items.Add(new ToolStripSeparator());
                toolStrip1.Items.Add(CreateIconButton("Login", LoadIcon("IconLogin", SystemIcons.WinLogo), OnToolbarStageClicked, "Credenciais / TQS"));
            }
        }

        private Image LoadIcon(string resourceKey, Icon fallbackIcon)
        {
            try
            {
                var obj = Properties.Resources.ResourceManager.GetObject(resourceKey);
                if (obj is Bitmap bmp) return bmp;
                if (obj is Icon ico) return ico.ToBitmap();
            }
            catch
            {
            }
            return fallbackIcon?.ToBitmap();
        }

        private ToolStripButton CreateIconButton(string tag, Image icon, EventHandler onClick, string toolTip)
        {
            var btn = new ToolStripButton
            {
                Tag = tag,
                Image = icon,
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                TextImageRelation = TextImageRelation.ImageAboveText,
                ToolTipText = toolTip,
                AutoSize = false,
                Size = new Size(100, 48),
                Margin = new Padding(4, 2, 4, 2),
                Font = new Font("Segoe UI", 11F, FontStyle.Regular)
            };
            btn.Click += onClick;
            return btn;
        }

        private void OnQuickActionClicked(object sender, EventArgs e)
        {
            if (sender is ToolStripButton button)
            {
                ShowStatus($"Ação rápida: {button.Text} (placeholder)", false);
            }
        }

        private void OnToolbarStageClicked(object sender, EventArgs e)
        {
            if (sender is ToolStripButton button)
            {
                var tag = button.Tag as string;

                if ((tag == "Geometria" || tag == "Propriedades" || tag == "Cargas" || tag == "Critérios" || tag == "Resultados") && !linkTypeSelected)
                {
                    ShowStatus("Escolha primeiro o tipo de ligação na coluna à esquerda.", true);
                    return;
                }

                switch (tag)
                {
                    case "Geometria":
                        tabControl1.SelectedTab = tabPage1;
                        // A visualização da geometria agora está sempre visível no layout principal.
                        break;
                    case "Propriedades":
                        // Placeholder para implementação futura: mostrar painel de propriedades.
                        ShowStatus("Área de Propriedades (em desenvolvimento).", false);
                        break;
                    case "Cargas":
                        // Placeholder para implementação futura: mostrar painel de cargas.
                        ShowStatus("Área de Cargas (em desenvolvimento).", false);
                        break;
                    case "Critérios":
                        // Placeholder para implementação futura: mostrar painel de critérios.
                        ShowStatus("Área de Critérios (em desenvolvimento).", false);
                        break;
                    case "Resultados":
                        // Placeholder para implementação futura: mostrar painel de resultados.
                        ShowStatus("Área de Resultados (em desenvolvimento).", false);
                        break;
                    case "VD":
                        // Placeholder para implementação futura: mostrar painel de VD.
                        ShowStatus("Área de Esforço Cortante (VD) (em desenvolvimento).", false);
                        break;
                    case "ND":
                        // Placeholder para implementação futura: mostrar painel de ND.
                        ShowStatus("Área de Esforço Normal (ND) (em desenvolvimento).", false);
                        break;
                    case "MD":
                        // Placeholder para implementação futura: mostrar painel de MD.
                        ShowStatus("Área de Momento (MD) (em desenvolvimento).", false);
                        break;
                    case "Calcular":
                        OnCalculateClicked(sender, EventArgs.Empty);
                        break;
                    case "ZoomIn":
                    case "ZoomOut":
                    case "ZoomFit":
                        ShowStatus($"Ação de zoom: {tag} (placeholder)", false);
                        break;
                    case "Login":
                        ShowStatus("Ação de login/acesso (placeholder)", false);
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // código TQS comentado
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "NULF.EXE";
            cmd.StartInfo.Arguments = "";
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = true;
            cmd.StartInfo.Verb = "runas";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            cmd.Start();
            cmd.WaitForExit();

            InitializeComponent2();
        }

        private void InitializeComponent2()
        {
            if (TQS.TQSTST.delta() != 0)
            {
                // lógica TQS comentada
            }
            else
            {
                // lógica TQS comentada
            }
        }

        private void ConfigureLayout()
        {
            if (tableLayoutPanel1 != null)
            {
                tableLayoutPanel1.RowCount = 4;
                tableLayoutPanel1.RowStyles.Clear();
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                if (menuStrip1 != null) tableLayoutPanel1.SetRow(menuStrip1, 0);
                if (toolStrip1 != null) tableLayoutPanel1.SetRow(toolStrip1, 1);
            }

            ConfigureNavigation();
            ConfigureTabs();
            BuildMainLayout();
            ConfigureFooter();

            tabControl1.SelectedIndexChanged -= OnTabChanged;
            tabControl1.SelectedIndexChanged += OnTabChanged;

            if (navigationButtons.Any())
            {
                SetActiveNavigationButton(navigationButtons.First());
            }
        }

        private void BuildMainLayout()
        {
            if (tableLayoutPanel1 == null)
            {
                return;
            }

            if (mainLayout != null && tableLayoutPanel1.Controls.Contains(mainLayout))
            {
                tableLayoutPanel1.Controls.Remove(mainLayout);
            }

            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3, // Navegação, Parâmetros, Visualização
                RowCount = 1,
                Padding = new Padding(6),
                Margin = Padding.Empty
            };
            // Colunas: Navegação (fixa), Parâmetros (relativa), Visualização (relativa)
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 210f));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f)); // 40% para parâmetros
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f)); // 60% para visualização
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            // Sidebar host (tableLayoutPanel2)
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Margin = Padding.Empty;
            tableLayoutPanel2.AutoScroll = true;
            tableLayoutPanel2.MinimumSize = new Size(180, 0);
            mainLayout.Controls.Add(tableLayoutPanel2, 0, 0);

            parametersHost = GetSharedInputsLayout() as FlowLayoutPanel ?? new FlowLayoutPanel();
            parametersHost.Dock = DockStyle.Fill;
            parametersHost.MinimumSize = new Size(250, 0);

            if (rightPanelHost == null)
            {
                rightPanelHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8), BackColor = Color.WhiteSmoke };
            }
            rightPanelHost.Controls.Clear();
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Margin = Padding.Empty;
            rightPanelHost.Controls.Add(tabControl1);
            rightPanelHost.MinimumSize = new Size(320, 0);

            // Agora, os parâmetros (esquerda) e as visualizações (direita) estão diretamente no mainLayout
            mainLayout.Controls.Add(parametersHost, 1, 0);
            mainLayout.Controls.Add(rightPanelHost, 2, 0);

            tableLayoutPanel1.Controls.Add(mainLayout, 0, 2);
        }

        private void ConfigureNavigation()
        {
            navigationButtons.Clear();
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.RowStyles.Clear();
            tableLayoutPanel2.ColumnStyles.Clear();
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.RowCount = 0;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var sections = new[]
            {
                "Flexível - cantoneira",
                "Flexível - chapa extremidade",
                "Flexível - chapa simples",
                "Rígida soldada",
                "Base rotulada",
                "Base engastada",
                "Mistas / anexos"
            };

            float rowPercent = sections.Length > 0 ? 100f / sections.Length : 100f;

            foreach (var section in sections)
            {
                var button = new Button
                {
                    Text = section,
                    Dock = DockStyle.Fill,
                    FlatStyle = FlatStyle.Standard,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(16, 10, 12, 10),
                    Tag = section,
                    Height = 45,
                    Cursor = Cursors.Hand
                };
                EngineeringStyle.StyleSidebarButton(button, false);
                button.Click += OnNavigationClicked;

                navigationButtons.Add(button);
                tableLayoutPanel2.RowCount++;
                tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, rowPercent));
                tableLayoutPanel2.Controls.Add(button, 0, tableLayoutPanel2.RowCount - 1);
            }
        }

        private void ConfigureTabs()
        {
            tabPage1.Controls.Clear();
            tabPage2.Controls.Clear();
            if (tabControl1.TabPages.Contains(tabPage3))
            {
                tabControl1.TabPages.Remove(tabPage3);
            }

            tabPage1.Text = "2D";
            tabPage2.Text = "3D";

            ConfigureTab2D();
            ConfigureTab3D();
        }

        private void ConfigureTab2D()
        {
            tabPage1.Controls.Clear();
            schematicPanel = Build2DViewsPanel();
            tabPage1.Controls.Add(schematicPanel);
        }

        private void ConfigureTab3D()
        {
            tabPage2.Controls.Clear();

            view3DHost = new Helix3DHost
            {
                Dock = DockStyle.Fill
            };

            var title = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Text = "Visualização 3D - ligação flexível",
                Height = 28,
                Font = new Font(Font.FontFamily, 14F, FontStyle.Bold)
            };

            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0)
            };
            view3DHost.Dock = DockStyle.Fill;
            rightPanel.Controls.Add(view3DHost);
            rightPanel.Controls.Add(title);

            view3DPanel = rightPanel;
        }

        private Control GetSharedInputsLayout()
        {
            if (geometryInputsLayout == null)
            {
                geometryInputsLayout = BuildInputsLayout();
            }

            return geometryInputsLayout;
        }

        private void MoveInputsLayoutTo(TabPage targetTab)
        {
            if (targetTab == null) return;

            var layout = GetSharedInputsLayout();
            if (layout.Parent == targetTab)
            {
                return;
            }

            if (layout.Parent != null)
            {
                layout.Parent.Controls.Remove(layout);
            }

            targetTab.Controls.Add(layout);
            layout.Dock = DockStyle.Fill;
        }

        private void EnsureInputsLayoutOnSelectedTab()
        {
            MoveInputsLayoutTo(tabControl1?.SelectedTab ?? tabPage1);
        }

        private Control BuildInputsLayout()
        {
            var layout = new FullWidthFlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(12),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.White
            };

            layout.Controls.Add(BuildBeamGroup());
            layout.Controls.Add(BuildColumnGroup());
            layout.Controls.Add(BuildPlateGroup());
            layout.Controls.Add(BuildBoltGroup());
            layout.Controls.Add(BuildWeldGroup());

            return layout;
        }

        private Control BuildBeamGroup()
        {
            var group = new GroupBox
            {
                Text = "Viga (perfil ou entrada manual)",
                Dock = DockStyle.Top,
                Padding = new Padding(15, 30, 15, 15),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12)
            };
            group.Font = new Font(Font.FontFamily, 14F, FontStyle.Bold);

            var table = CreateFormTable();
            table.Font = this.Font;
            var row = 0;

            beamProfileCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            EngineeringStyle.StyleInputControl(beamProfileCombo);
            PopulateProfileCombo(beamProfileCombo);
            beamProfileCombo.SelectedIndexChanged += BeamProfileCombo_OnSelectedIndexChanged;

            var perfilLabel = new Label { Text = "Perfil", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            EngineeringStyle.StyleInputControl(perfilLabel);
            table.Controls.Add(perfilLabel, 0, row);
            table.Controls.Add(beamProfileCombo, 1, row);
            table.SetColumnSpan(beamProfileCombo, 2);
            row++;

            beamHeightBox = CreateNumericBox("BeamHeight", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Altura h_w", beamHeightBox);

            beamFlangeBox = CreateNumericBox("BeamFlangeThickness", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Espessura mesa t_f", beamFlangeBox);

            beamWebBox = CreateNumericBox("BeamWebThickness", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Espessura alma t_w", beamWebBox);

            beamFlangeWidthBox = CreateNumericBox("BeamFlangeWidth", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Largura mesa bf_w", beamFlangeWidthBox);

            group.Controls.Add(table);
            return group;
        }

        private Control BuildColumnGroup()
        {
            var group = new GroupBox
            {
                Text = "Pilar (perfil ou entrada manual)",
                Dock = DockStyle.Top,
                Padding = new Padding(15, 30, 15, 15),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12)
            };
            group.Font = new Font(Font.FontFamily, 14F, FontStyle.Bold);

            var table = CreateFormTable();
            table.Font = this.Font;
            var row = 0;

            columnProfileCombo = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            EngineeringStyle.StyleInputControl(columnProfileCombo);
            PopulateProfileCombo(columnProfileCombo);
            columnProfileCombo.SelectedIndexChanged += ColumnProfileCombo_OnSelectedIndexChanged;

            var perfilLabel = new Label { Text = "Perfil", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Anchor = AnchorStyles.Left | AnchorStyles.Right };
            EngineeringStyle.StyleInputControl(perfilLabel);
            table.Controls.Add(perfilLabel, 0, row);
            table.Controls.Add(columnProfileCombo, 1, row);
            table.SetColumnSpan(columnProfileCombo, 2);
            row++;

            columnHeightBox = CreateNumericBox("ColumnDepth", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Altura h_w,c", columnHeightBox);

            columnFlangeBox = CreateNumericBox("ColumnFlangeThickness", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Espessura mesa t_f,c", columnFlangeBox);

            columnWebBox = CreateNumericBox("ColumnWebThickness", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Espessura alma t_w,c", columnWebBox);

            columnFlangeWidthBox = CreateNumericBox("ColumnWidth", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Largura bf_c", columnFlangeWidthBox);

            group.Controls.Add(table);
            return group;
        }

        private Control BuildPlateGroup()
        {
            var group = new GroupBox
            {
                Text = "Placa e bordas",
                Dock = DockStyle.Top,
                Padding = new Padding(15, 30, 15, 15),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12)
            };

            var table = CreateFormTable();
            group.Font = new Font(Font.FontFamily, 14F, FontStyle.Bold);
            table.Font = this.Font;
            var row = 0;

            plateThicknessBox = CreateNumericBox("PlateThickness", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Espessura t_p", plateThicknessBox);

            plateWidthBox = CreateNumericBox("PlateWidth", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Largura l_p", plateWidthBox);

            plateEdgeHorizontalBox = CreateNumericBox("PlateEdgeHorizontal", OnGeometryTextChanged, true);
            AddInputRow(table, ref row, "Borda lateral e_h", plateEdgeHorizontalBox);

            plateEdgeTopBox = CreateNumericBox("PlateEdgeTop", OnGeometryTextChanged, true);
            AddInputRow(table, ref row, "Borda superior e_t,p", plateEdgeTopBox);

            plateEdgeBottomBox = CreateNumericBox("PlateEdgeBottom", OnGeometryTextChanged, true);
            AddInputRow(table, ref row, "Borda inferior e_b,p", plateEdgeBottomBox);

            group.Controls.Add(table);
            return group;
        }

        private Control BuildBoltGroup()
        {
            var group = new GroupBox
            {
                Text = "Parafusos (ligação flexível)",
                Dock = DockStyle.Top,
                Padding = new Padding(15, 30, 15, 15),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12)
            };

            var table = CreateFormTable();
            group.Font = new Font(Font.FontFamily, 14F, FontStyle.Bold);
            table.Font = this.Font;
            var row = 0;

            boltDiameterBox = CreateNumericBox("BoltDiameter", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Diâmetro d_b", boltDiameterBox);

            boltCountBox = CreateNumericBox("BoltCount", OnBoltCountChanged);
            AddInputRow(table, ref row, "Nº linhas n", boltCountBox, "un");

            boltEdgeTopBox = CreateNumericBox("BoltEdgeTop", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Borda superior e_t", boltEdgeTopBox);

            boltSpacingBox = CreateNumericBox("BoltSpacing", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Passo vertical p_v", boltSpacingBox);

            boltGaugeBox = CreateNumericBox("BoltGauge", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Gabarito g_h", boltGaugeBox);

            group.Controls.Add(table);
            return group;
        }

        private Control BuildWeldGroup()
        {
            var group = new GroupBox
            {
                Text = "Solda / geral",
                Dock = DockStyle.Top,
                Padding = new Padding(15, 30, 15, 15),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12)
            };

            var table = CreateFormTable();
            group.Font = new Font(Font.FontFamily, 14F, FontStyle.Bold);
            table.Font = this.Font;
            var row = 0;

            weldSizeBox = CreateNumericBox("WeldSize", OnGeometryTextChanged);
            AddInputRow(table, ref row, "Perna solda s_w", weldSizeBox);

            group.Controls.Add(table);
            return group;
        }

        private Panel Build2DViewsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = EngineeringStyle.BackgroundColor,
                Padding = new Padding(8)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Título dentro de um painel auto-ajustável para evitar corte
            var titlePanel = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 10) };
            var title = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Text = "Visualização 2D (vista lateral e frontal) - ligação flexível",
                Font = new Font(Font.FontFamily, 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            titlePanel.Controls.Add(title);
            layout.Controls.Add(titlePanel, 0, 0);

            sideViewPanel = new DoubleBufferedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                MinimumSize = new Size(200, 150)
            };
            sideViewPanel.Paint += DrawSideView;

            frontViewPanel = new DoubleBufferedPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                MinimumSize = new Size(200, 150)
            };
            frontViewPanel.Paint += DrawFrontView;

            layout.Controls.Add(sideViewPanel, 0, 1);
            layout.Controls.Add(frontViewPanel, 0, 2);

            panel.Controls.Add(layout);
            return panel;
        }

    private static TableLayoutPanel CreateFormTable()
    {
        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(6)
        };
        // Proporção mais equilibrada: 45% rótulo, 40% campo, 15% unidade
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
        return table;
    }

        private TextBox CreateNumericBox(string tag, EventHandler handler, bool readOnly = false)
        {
            var box = new TextBox
            {
                Dock = DockStyle.Fill,
                TextAlign = HorizontalAlignment.Right,
                ReadOnly = readOnly
            };
            box.Tag = tag;
            box.TextChanged += handler;
            EngineeringStyle.StyleInputControl(box);
            return box;
        }

        private void AddInputRow(TableLayoutPanel table, ref int row, string labelText, TextBox box, string unit = "mm")
        {
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));

            var label = new Label
            {
                Text = labelText,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(0, 4, 4, 0)
            };
            EngineeringStyle.StyleInputControl(label);

            var unitLabel = new Label
            {
                Text = unit,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(4, 4, 0, 0)
            };
            EngineeringStyle.StyleInputControl(unitLabel);

            EngineeringStyle.StyleInputControl(box);

            table.Controls.Add(label, 0, row);
            table.Controls.Add(box, 1, row);
            table.Controls.Add(unitLabel, 2, row);
            row++;
        }

        private void ConfigureFooter()
        {
            if (tableLayoutPanel1.RowStyles.Count > 3)
            {
                tableLayoutPanel1.RowStyles[3] = new RowStyle(SizeType.AutoSize);
            }

            var existingFooter = tableLayoutPanel1.GetControlFromPosition(0, 3);
            if (existingFooter != null)
            {
                tableLayoutPanel1.Controls.Remove(existingFooter);
            }

            var footerPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(6)
            };
            footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            footerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            footerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            calculateButton = new Button
            {
                Text = "Calcular",
                AutoSize = true,
                Dock = DockStyle.Left,
                Padding = new Padding(10, 6, 10, 6)
            };
            calculateButton.Click += OnCalculateClicked;

            generateReportButton = new Button
            {
                Text = "Gerar Relatório",
                AutoSize = true,
                Dock = DockStyle.Left,
                Padding = new Padding(10, 6, 10, 6)
            };
            generateReportButton.Click += OnGenerateReportClicked;

            if (reportBox == null)
            {
                reportBox = new RichTextBox();
            }

            statusLabel = new ToolStripStatusLabel { Text = "Pronto" };
            statusStrip = new StatusStrip
            {
                Dock = DockStyle.Fill,
                SizingGrip = false
            };
            statusStrip.Items.Add(statusLabel);

            footerPanel.Controls.Add(calculateButton, 0, 0);
            footerPanel.Controls.Add(generateReportButton, 1, 0);
            footerPanel.Controls.Add(statusStrip, 2, 0);

            tableLayoutPanel1.Controls.Add(footerPanel, 0, 3);
        }

        private void OnNavigationClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                SetActiveNavigationButton(button);
                tabControl1.SelectedTab = tabPage1;
                ShowStatus($"Etapa selecionada: {button.Text}", false);
                linkTypeSelected = true;
                UpdateRightPanelContent();
            }
        }

        private void OnTabChanged(object sender, EventArgs e)
        {
            EnsureInputsLayoutOnSelectedTab();
            UpdateRightPanelContent();
        }

        private void SetActiveNavigationButton(Button selected)
        {
            foreach (var button in navigationButtons)
            {
                if (button == selected)
                {
                    EngineeringStyle.StyleSidebarButton(button, true);
                }
                else
                {
                    EngineeringStyle.StyleSidebarButton(button, false);
                }
            }
        }

        private void UpdateRightPanelContent()
        {
            if (rightPanelHost == null)
            {
                return;
            }

            rightPanelHost.Controls.Clear();

            if (tabControl1.SelectedTab == tabPage1 && schematicPanel != null)
            {
                rightPanelHost.Controls.Add(schematicPanel);
            }
            else if (tabControl1.SelectedTab == tabPage2 && view3DPanel != null)
            {
                rightPanelHost.Controls.Add(view3DPanel);
            }
            // Aba de relatório removida na geometria; mantém 2D/3D.
        }

        private void OnCalculateClicked(object sender, EventArgs e)
        {
            if (!TrySyncGeometryFromInputs())
            {
                ShowStatus("Valores de geometria inválidos. Revise os campos.", true);
                return;
            }

            data.geometry.hx = geometryParams.BeamHeight * 1000.0;
            data.geometry.hy = geometryParams.BeamThickness * 1000.0;

            ShowStatus($"Geometria (flexível) atualizada: h_w = {data.geometry.hx:0.#} mm, bf_w = {data.geometry.hy:0.#} mm, n = {geometryParams.BoltCount} Ø{geometryParams.BoltDiameter * 1000:0.#} mm.", false);
            RefreshGeometryViews();
        }

        private void OnGenerateReportClicked(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Memorial de Cálculo - Geometria (flexível)");
            sb.AppendLine("------------------------------------------");
            sb.AppendLine($"Viga: h_w = {geometryParams.BeamHeight * 1000:0.#} mm, t_f = {geometryParams.BeamFlangeThickness * 1000:0.#} mm, t_w = {geometryParams.BeamWebThickness * 1000:0.#} mm, bf_w = {geometryParams.BeamThickness * 1000:0.#} mm");
            sb.AppendLine($"Pilar: h_w,c = {geometryParams.ColumnDepth * 1000:0.#} mm, t_f,c = {geometryParams.ColumnFlangeThickness * 1000:0.#} mm, t_w,c = {geometryParams.ColumnWebThickness * 1000:0.#} mm, bf_c = {geometryParams.ColumnWidth * 1000:0.#} mm");
            sb.AppendLine($"Placa: t_p = {geometryParams.PlateThickness * 1000:0.#} mm, l_p = {geometryParams.PlateWidth * 1000:0.#} mm, e_h = {geometryParams.PlateEdgeHorizontal * 1000:0.#} mm, e_t = {geometryParams.PlateEdgeTop * 1000:0.#} mm, e_b = {geometryParams.PlateEdgeBottom * 1000:0.#} mm");
            sb.AppendLine($"Parafusos: n = {geometryParams.BoltCount}, p_v = {geometryParams.BoltSpacing * 1000:0.#} mm, g_h = {geometryParams.BoltGauge * 1000:0.#} mm, d_b = {geometryParams.BoltDiameter * 1000:0.#} mm");
            sb.AppendLine($"Solda (filete): s_w = {geometryParams.WeldSize * 1000:0.#} mm");
            sb.AppendLine($"Data/Hora: {DateTime.Now}");

            if (reportBox != null)
            {
                reportBox.Text = sb.ToString();
            }

            ShowStatus("Relatório gerado na aba Relatório.", false);
        }

        private void LoadGeometryToInputs()
        {
            geometryInputsUpdating = true;

            if (data?.geometry != null)
            {
                if (data.geometry.hx > 0) geometryParams.BeamHeight = data.geometry.hx / 1000.0;
                if (data.geometry.hy > 0) geometryParams.BeamThickness = data.geometry.hy / 1000.0;
            }

            PushGeometryToInputs();
            geometryInputsUpdating = false;

            UpdateDerivedGeometry();
            RefreshGeometryViews();
        }

        private void PushGeometryToInputs()
        {
            SetText(beamHeightBox, geometryParams.BeamHeight);
            SetText(beamFlangeBox, geometryParams.BeamFlangeThickness);
            SetText(beamWebBox, geometryParams.BeamWebThickness);
            SetText(beamFlangeWidthBox, geometryParams.BeamThickness);

            SetText(columnHeightBox, geometryParams.ColumnDepth);
            SetText(columnFlangeBox, geometryParams.ColumnFlangeThickness);
            SetText(columnWebBox, geometryParams.ColumnWebThickness);
            SetText(columnFlangeWidthBox, geometryParams.ColumnWidth);

            SetText(plateThicknessBox, geometryParams.PlateThickness);
            SetText(plateWidthBox, geometryParams.PlateWidth);
            SetText(plateEdgeHorizontalBox, geometryParams.PlateEdgeHorizontal);
            SetText(plateEdgeTopBox, geometryParams.PlateEdgeTop);
            SetText(plateEdgeBottomBox, geometryParams.PlateEdgeBottom);

            boltCountBox.Text = geometryParams.BoltCount.ToString(CultureInfo.InvariantCulture);
            SetText(boltSpacingBox, geometryParams.BoltSpacing);
            SetText(boltEdgeTopBox, geometryParams.BoltEdgeTop);
            SetText(boltGaugeBox, geometryParams.BoltGauge);
            SetText(boltDiameterBox, geometryParams.BoltDiameter);

            SetText(weldSizeBox, geometryParams.WeldSize);
        }

        private void UpdateDerivedGeometry(bool pushToInputs = true)
        {
            geometryParams.BeamLength = Math.Max(0.1, 0.75 * geometryParams.BeamHeight);
            geometryParams.PlateHeight = geometryParams.BeamHeight;

            var eH = (geometryParams.PlateWidth - geometryParams.BoltGauge) / 2.0;
            var eTop = geometryParams.BoltEdgeTop - geometryParams.BeamFlangeThickness;
            var pitchTotal = Math.Max(0, geometryParams.BoltCount - 1) * geometryParams.BoltSpacing;
            var eBottom = geometryParams.BeamHeight - eTop - pitchTotal;

            geometryParams.PlateEdgeHorizontal = eH;
            geometryParams.PlateEdgeTop = eTop;
            geometryParams.PlateEdgeBottom = eBottom;

            if (pushToInputs)
            {
                geometryInputsUpdating = true;
                SetText(plateEdgeHorizontalBox, geometryParams.PlateEdgeHorizontal);
                SetText(plateEdgeTopBox, geometryParams.PlateEdgeTop);
                SetText(plateEdgeBottomBox, geometryParams.PlateEdgeBottom);
                geometryInputsUpdating = false;
            }
        }

        private void RefreshGeometryViews()
        {
            sideViewPanel?.Invalidate();
            frontViewPanel?.Invalidate();
            UpdateHelixScene();
        }

        private void UpdateHelixScene()
        {
            if (helixSceneBuilder == null || view3DHost == null || geometryParams == null)
            {
                return;
            }

            var scene = helixSceneBuilder.BuildScene(geometryParams);
            view3DHost.SetScene(scene);
        }

        private void OnGeometryTextChanged(object sender, EventArgs e)
        {
            if (geometryInputsUpdating || !(sender is TextBox box) || box.ReadOnly)
            {
                return;
            }

            if (!TryParseDouble(box.Text, out var valueMm) || valueMm <= 0)
            {
                return;
            }

            var valueMeters = valueMm / 1000.0;
            switch (box.Tag as string)
            {
                case "BeamHeight":
                    geometryParams.BeamHeight = valueMeters;
                    break;
                case "BeamFlangeThickness":
                    geometryParams.BeamFlangeThickness = valueMeters;
                    break;
                case "BeamWebThickness":
                    geometryParams.BeamWebThickness = valueMeters;
                    break;
                case "BeamFlangeWidth":
                    geometryParams.BeamThickness = valueMeters;
                    break;
                case "ColumnDepth":
                    geometryParams.ColumnDepth = valueMeters;
                    break;
                case "ColumnFlangeThickness":
                    geometryParams.ColumnFlangeThickness = valueMeters;
                    break;
                case "ColumnWebThickness":
                    geometryParams.ColumnWebThickness = valueMeters;
                    break;
                case "ColumnWidth":
                    geometryParams.ColumnWidth = valueMeters;
                    geometryParams.ColumnWebWidth = valueMeters;
                    break;
                case "PlateThickness":
                    geometryParams.PlateThickness = valueMeters;
                    break;
                case "PlateWidth":
                    geometryParams.PlateWidth = valueMeters;
                    break;
                case "BoltSpacing":
                    geometryParams.BoltSpacing = valueMeters;
                    break;
                case "BoltEdgeTop":
                    geometryParams.BoltEdgeTop = valueMeters;
                    break;
                case "BoltGauge":
                    geometryParams.BoltGauge = valueMeters;
                    break;
                case "BoltDiameter":
                    geometryParams.BoltDiameter = valueMeters;
                    break;
                case "WeldSize":
                    geometryParams.WeldSize = valueMeters;
                    break;
                default:
                    return;
            }

            UpdateDerivedGeometry();
            RefreshGeometryViews();
        }

        private void OnBoltCountChanged(object sender, EventArgs e)
        {
            if (geometryInputsUpdating || !(sender is TextBox box))
            {
                return;
            }

            if (int.TryParse(box.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) && count > 0)
            {
                geometryParams.BoltCount = count;
                UpdateDerivedGeometry();
                RefreshGeometryViews();
            }
        }

        private void BeamProfileCombo_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (beamProfileCombo?.SelectedItem is ProfileDefinition profile)
            {
                ApplyBeamProfile(profile);
            }
            else
            {
                SetBeamInputsReadOnly(false);
            }
        }

        private void ColumnProfileCombo_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (columnProfileCombo?.SelectedItem is ProfileDefinition profile)
            {
                ApplyColumnProfile(profile);
            }
            else
            {
                SetColumnInputsReadOnly(false);
            }
        }

        private void PopulateProfileCombo(ComboBox combo)
        {
            if (combo == null) return;
            combo.Items.Clear();
            combo.Items.Add("-- Entrada manual --");
            foreach (var profile in ProfileLibrary)
            {
                combo.Items.Add(profile);
            }
            combo.SelectedIndex = 0;
        }

        private void SelectProfileByName(ComboBox combo, string name)
        {
            if (combo == null || string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            for (var i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is ProfileDefinition profile && profile.Name == name)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        private void ApplyBeamProfile(ProfileDefinition profile)
        {
            geometryInputsUpdating = true;
            SetText(beamHeightBox, profile.DepthMm / 1000.0);
            SetText(beamFlangeBox, profile.FlangeThicknessMm / 1000.0);
            SetText(beamWebBox, profile.WebThicknessMm / 1000.0);
            SetText(beamFlangeWidthBox, profile.FlangeWidthMm / 1000.0);
            geometryInputsUpdating = false;
            SetBeamInputsReadOnly(true);

            geometryParams.BeamHeight = profile.DepthMm / 1000.0;
            geometryParams.BeamFlangeThickness = profile.FlangeThicknessMm / 1000.0;
            geometryParams.BeamWebThickness = profile.WebThicknessMm / 1000.0;
            geometryParams.BeamThickness = profile.FlangeWidthMm / 1000.0;
            UpdateDerivedGeometry();
            RefreshGeometryViews();
        }

        private void ApplyColumnProfile(ProfileDefinition profile)
        {
            geometryInputsUpdating = true;
            SetText(columnHeightBox, profile.DepthMm / 1000.0);
            SetText(columnFlangeBox, profile.FlangeThicknessMm / 1000.0);
            SetText(columnWebBox, profile.WebThicknessMm / 1000.0);
            SetText(columnFlangeWidthBox, profile.FlangeWidthMm / 1000.0);
            geometryInputsUpdating = false;
            SetColumnInputsReadOnly(true);

            geometryParams.ColumnDepth = profile.DepthMm / 1000.0;
            geometryParams.ColumnFlangeThickness = profile.FlangeThicknessMm / 1000.0;
            geometryParams.ColumnWebThickness = profile.WebThicknessMm / 1000.0;
            geometryParams.ColumnWidth = profile.FlangeWidthMm / 1000.0;
            geometryParams.ColumnWebWidth = profile.FlangeWidthMm / 1000.0;
            UpdateDerivedGeometry();
            RefreshGeometryViews();
        }

        private void SetBeamInputsReadOnly(bool isReadOnly)
        {
            if (beamHeightBox != null) beamHeightBox.ReadOnly = isReadOnly;
            if (beamFlangeBox != null) beamFlangeBox.ReadOnly = isReadOnly;
            if (beamWebBox != null) beamWebBox.ReadOnly = isReadOnly;
            if (beamFlangeWidthBox != null) beamFlangeWidthBox.ReadOnly = isReadOnly;
        }

        private void SetColumnInputsReadOnly(bool isReadOnly)
        {
            if (columnHeightBox != null) columnHeightBox.ReadOnly = isReadOnly;
            if (columnFlangeBox != null) columnFlangeBox.ReadOnly = isReadOnly;
            if (columnWebBox != null) columnWebBox.ReadOnly = isReadOnly;
            if (columnFlangeWidthBox != null) columnFlangeWidthBox.ReadOnly = isReadOnly;
        }

        private bool TrySyncGeometryFromInputs()
        {
            var ok = true;

            ok &= TryParseMillimeters(beamHeightBox, out var bh);
            ok &= TryParseMillimeters(beamFlangeBox, out var bft);
            ok &= TryParseMillimeters(beamWebBox, out var bwt);
            ok &= TryParseMillimeters(beamFlangeWidthBox, out var bfw);

            ok &= TryParseMillimeters(columnHeightBox, out var ch);
            ok &= TryParseMillimeters(columnFlangeBox, out var cft);
            ok &= TryParseMillimeters(columnWebBox, out var cwt);
            ok &= TryParseMillimeters(columnFlangeWidthBox, out var cfw);

            ok &= TryParseMillimeters(plateThicknessBox, out var pt);
            ok &= TryParseMillimeters(plateWidthBox, out var pw);
            ok &= TryParseMillimeters(boltSpacingBox, out var bs);
            ok &= TryParseMillimeters(boltEdgeTopBox, out var be);
            ok &= TryParseMillimeters(boltGaugeBox, out var bg);
            ok &= TryParseMillimeters(boltDiameterBox, out var bd);
            ok &= TryParseMillimeters(weldSizeBox, out var ws);

            if (!int.TryParse(boltCountBox?.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var boltCount) || boltCount <= 0)
            {
                ok = false;
            }

            if (!ok)
            {
                return false;
            }

            geometryParams.BeamHeight = bh;
            geometryParams.BeamFlangeThickness = bft;
            geometryParams.BeamWebThickness = bwt;
            geometryParams.BeamThickness = bfw;

            geometryParams.ColumnDepth = ch;
            geometryParams.ColumnFlangeThickness = cft;
            geometryParams.ColumnWebThickness = cwt;
            geometryParams.ColumnWidth = cfw;
            geometryParams.ColumnWebWidth = cfw;

            geometryParams.PlateThickness = pt;
            geometryParams.PlateWidth = pw;

            geometryParams.BoltCount = boltCount;
            geometryParams.BoltSpacing = bs;
            geometryParams.BoltEdgeTop = be;
            geometryParams.BoltGauge = bg;
            geometryParams.BoltDiameter = bd;

            geometryParams.WeldSize = ws;

            UpdateDerivedGeometry();
            return true;
        }

        private bool TryParseMillimeters(TextBox box, out double meters)
        {
            meters = 0;
            if (box == null)
            {
                return false;
            }

            if (TryParseDouble(box.Text, out var valueMm) && valueMm > 0)
            {
                meters = valueMm / 1000.0;
                return true;
            }

            return false;
        }

        private bool TryParseDouble(string input, out double value)
        {
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
            {
                return true;
            }

            return double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private void SetText(TextBox box, double valueMeters)
        {
            if (box == null) return;
            box.Text = (valueMeters * 1000.0).ToString("0.0", CultureInfo.InvariantCulture);
        }

        private void DrawSideView(object sender, PaintEventArgs e)
        {
            if (sideViewPanel == null || geometryParams == null) return;
            DrawingLogic.DrawSideView(e.Graphics, geometryParams, sideViewPanel.ClientSize.Width, sideViewPanel.ClientSize.Height);
        }

        private void DrawFrontView(object sender, PaintEventArgs e)
        {
            if (frontViewPanel == null || geometryParams == null) return;
            DrawingLogic.DrawFrontView(e.Graphics, geometryParams, frontViewPanel.ClientSize.Width, frontViewPanel.ClientSize.Height);
        }

        private void ShowStatus(string message, bool isError)
        {
            if (statusLabel == null) return;
            statusLabel.Text = message;
            statusLabel.ForeColor = isError ? Color.Firebrick : SystemColors.ControlText;
        }
    }

    internal sealed class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }
    }

    internal sealed class FullWidthFlowLayoutPanel : FlowLayoutPanel
    {
        public FullWidthFlowLayoutPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            UpdateStyles();
            FlowDirection = FlowDirection.TopDown;
            WrapContents = false;
            AutoScroll = true;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            var client = ClientSize.Width - Padding.Horizontal;
            const int maxWidth = 460; // mantém blocos compactos para inputs
            foreach (Control ctrl in Controls)
            {
                var targetWidth = Math.Min(client, maxWidth) - ctrl.Margin.Horizontal;
                if (targetWidth > 0 && ctrl.Width != targetWidth)
                {
                    ctrl.Width = targetWidth;
                }
            }
        }
    }

    internal class GeometryParameters
    {
        // Defaults alinhados aos valores do HTML (mm -> m)
        public double BeamLength { get; set; } = 0.225;          // 225 mm (0.75 * h_w)
        public double BeamHeight { get; set; } = 0.3;            // h_w = 300 mm
        public double BeamThickness { get; set; } = 0.15;        // bf_w = 150 mm
        public double BeamFlangeThickness { get; set; } = 0.012; // t_f = 12 mm
        public double BeamWebThickness { get; set; } = 0.008;    // t_w = 8 mm
        public double ColumnHeight { get; set; } = 0.26;         // h_w,c = 260 mm (unused)
        public double ColumnWidth { get; set; } = 0.2;           // bf_c = 200 mm
        public double ColumnDepth { get; set; } = 0.26;          // h_w,c = 260 mm
        public double ColumnFlangeThickness { get; set; } = 0.02;// t_f,c = 20 mm
        public double ColumnWebThickness { get; set; } = 0.01;   // t_w,c = 10 mm
        public double ColumnWebWidth { get; set; } = 0.2;        // bf_c = 200 mm
        public double PlateHeight { get; set; } = 0.3;           // h_p = h_w = 300 mm
        public double PlateWidth { get; set; } = 0.2;            // l_p = 200 mm
        public double PlateThickness { get; set; } = 0.016;      // t_p = 16 mm
        public double PlateEdgeHorizontal { get; set; } = 0.04;  // e_h = (200-120)/2 = 40 mm
        public double PlateEdgeTop { get; set; } = 0.038;        // e_t,p = 50 - 12 = 38 mm
        public double PlateEdgeBottom { get; set; } = 0.037;     // e_b,p = 37 mm
        public int BoltCount { get; set; } = 4;                  // n = 4
        public double BoltSpacing { get; set; } = 0.075;         // p_v = 75 mm
        public double BoltDiameter { get; set; } = 0.022;        // d_b = 22 mm
        public double BoltEdgeTop { get; set; } = 0.05;          // e_t = 50 mm
        public double BoltGauge { get; set; } = 0.12;            // g_h = 120 mm
        public double WeldSize { get; set; } = 0.005;            // s_w = 5 mm
    }

    internal sealed class ProfileDefinition
    {
        public string Name { get; }
        public double DepthMm { get; }
        public double WebThicknessMm { get; }
        public double FlangeThicknessMm { get; }
        public double FlangeWidthMm { get; }

        public ProfileDefinition(string name, double depthMm, double webThicknessMm, double flangeThicknessMm, double flangeWidthMm)
        {
            Name = name;
            DepthMm = depthMm;
            WebThicknessMm = webThicknessMm;
            FlangeThicknessMm = flangeThicknessMm;
            FlangeWidthMm = flangeWidthMm;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    internal static class ProfileLibraryData
    {
        public static readonly IReadOnlyList<ProfileDefinition> Profiles = new List<ProfileDefinition>
        {
            new ProfileDefinition("W 150x13,0", 150, 4.0, 5.0, 100),
            new ProfileDefinition("W 150x18,0", 153, 5.8, 7.1, 102),
            new ProfileDefinition("W 150x22,5", 152, 5.8, 6.6, 152),
            new ProfileDefinition("W 150x24,0", 160, 6.6, 10.3, 103),
            new ProfileDefinition("W 150x29,8", 157, 6.6, 9.3, 154),
            new ProfileDefinition("W 150x37,1", 162, 8.1, 11.6, 156),
            new ProfileDefinition("W 200x15,0", 200, 4.3, 5.2, 100),
            new ProfileDefinition("W 200x19,3", 203, 5.8, 6.5, 102),
            new ProfileDefinition("W 200x22,5", 207, 6.2, 8.5, 133),
            new ProfileDefinition("W 200x26,6", 201, 5.8, 8.4, 133),
            new ProfileDefinition("W 200x31,3", 204, 6.4, 10.2, 134),
            new ProfileDefinition("W 200x35,9", 203, 6.2, 9.5, 165),
            new ProfileDefinition("W 200x41,7", 207, 7.1, 11.5, 166),
            new ProfileDefinition("W 200x46,1", 202, 7.2, 11.0, 203),
            new ProfileDefinition("W 200x52,0", 206, 8.0, 12.6, 204),
            new ProfileDefinition("W 250x17,9", 250, 4.8, 5.3, 101),
            new ProfileDefinition("W 250x22,3", 254, 5.6, 6.8, 102),
            new ProfileDefinition("W 250x25,3", 257, 6.1, 8.3, 146),
            new ProfileDefinition("W 250x28,4", 260, 6.1, 9.8, 147),
            new ProfileDefinition("W 250x32,7", 251, 6.1, 9.7, 177),
            new ProfileDefinition("W 250x38,5", 255, 6.9, 11.9, 178),
            new ProfileDefinition("W 250x44,8", 259, 7.9, 14.0, 180),
            new ProfileDefinition("W 250x49,1", 249, 7.4, 11.7, 203),
            new ProfileDefinition("W 250x54,9", 252, 8.0, 13.5, 204),
            new ProfileDefinition("W 250x67,3", 258, 9.1, 16.5, 206),
            new ProfileDefinition("W 250x72,9", 252, 8.6, 13.2, 254),
            new ProfileDefinition("W 250x80,3", 256, 9.5, 15.2, 255),
            new ProfileDefinition("W 250x88,6", 260, 10.4, 17.3, 257),
            new ProfileDefinition("W 310x21,0", 303, 5.1, 5.7, 101),
            new ProfileDefinition("W 310x23,8", 307, 5.5, 6.7, 102),
            new ProfileDefinition("W 310x28,3", 312, 6.6, 8.3, 165),
            new ProfileDefinition("W 310x32,7", 314, 7.1, 10.0, 166),
            new ProfileDefinition("W 310x38,7", 307, 6.1, 9.7, 165),
            new ProfileDefinition("W 310x44,5", 311, 6.9, 11.8, 167),
            new ProfileDefinition("W 310x52,0", 315, 7.9, 13.9, 169),
            new ProfileDefinition("W 310x52,0 (alt)", 310, 7.5, 13.1, 204),
            new ProfileDefinition("W 310x60,0", 309, 7.6, 13.2, 203),
            new ProfileDefinition("W 310x74,4", 316, 9.4, 16.5, 206),
            new ProfileDefinition("W 310x86,3", 308, 9.4, 15.2, 254),
            new ProfileDefinition("W 310x101,0", 314, 10.7, 18.7, 257),
            new ProfileDefinition("W 310x118,0", 320, 12.4, 21.3, 260),
            new ProfileDefinition("W 310x129,0", 324, 13.6, 23.4, 262),
            new ProfileDefinition("W 360x32,9", 352, 6.4, 8.5, 127),
            new ProfileDefinition("W 360x39,0", 356, 6.9, 10.3, 128),
            new ProfileDefinition("W 360x44,6", 360, 7.6, 12.2, 129),
            new ProfileDefinition("W 360x51,0", 353, 7.1, 11.4, 171),
            new ProfileDefinition("W 360x51,0 (alt)", 355, 7.9, 11.0, 172),
            new ProfileDefinition("W 360x58,0", 357, 8.0, 13.5, 172),
            new ProfileDefinition("W 360x64,0", 360, 8.8, 15.4, 173),
            new ProfileDefinition("W 360x72,0", 368, 9.8, 17.4, 175),
            new ProfileDefinition("W 360x79,0", 355, 8.9, 14.4, 253),
            new ProfileDefinition("W 360x79,0 (alt)", 362, 10.0, 16.4, 255),
            new ProfileDefinition("W 360x91,0", 360, 10.0, 16.8, 254),
            new ProfileDefinition("W 360x101,0", 363, 11.2, 18.3, 256),
            new ProfileDefinition("W 360x110,0", 367, 12.2, 20.3, 258),
            new ProfileDefinition("W 360x122,0", 359, 11.2, 18.0, 305),
            new ProfileDefinition("W 360x134,0", 363, 12.2, 20.1, 306),
            new ProfileDefinition("W 360x147,0", 367, 13.3, 22.1, 308),
            new ProfileDefinition("W 360x179,0", 375, 16.1, 26.2, 312),
            new ProfileDefinition("W 410x38,8", 400, 6.4, 8.5, 140),
            new ProfileDefinition("W 410x46,1", 404, 6.9, 10.3, 141),
            new ProfileDefinition("W 410x53,0", 408, 7.6, 12.2, 142),
            new ProfileDefinition("W 410x60,0", 403, 7.7, 12.8, 177),
            new ProfileDefinition("W 410x60,0 (alt)", 408, 8.8, 12.8, 178),
            new ProfileDefinition("W 410x67,0", 407, 8.6, 14.7, 178),
            new ProfileDefinition("W 410x75,0", 411, 9.7, 16.5, 180),
            new ProfileDefinition("W 410x85,0", 417, 10.9, 18.8, 181),
            new ProfileDefinition("W 410x85,0 (alt)", 417, 10.9, 17.7, 256),
            new ProfileDefinition("W 460x52,0", 449, 7.1, 10.8, 152),
            new ProfileDefinition("W 460x60,0", 454, 7.9, 12.7, 154),
            new ProfileDefinition("W 460x68,0", 459, 8.9, 14.7, 155),
            new ProfileDefinition("W 460x74,0", 457, 8.0, 14.5, 190),
            new ProfileDefinition("W 460x82,0", 461, 8.9, 16.5, 191),
            new ProfileDefinition("W 460x89,0", 464, 9.5, 18.0, 192),
            new ProfileDefinition("W 460x97,0", 467, 10.3, 19.8, 194),
            new ProfileDefinition("W 460x106,0", 470, 11.2, 21.5, 195),
            new ProfileDefinition("W 530x66,0", 525, 8.6, 12.7, 165),
            new ProfileDefinition("W 530x72,0", 528, 9.1, 14.0, 166),
            new ProfileDefinition("W 530x74,0", 526, 8.5, 13.3, 208),
            new ProfileDefinition("W 530x82,0", 530, 9.5, 15.2, 209),
            new ProfileDefinition("W 530x85,0", 530, 9.8, 15.6, 209),
            new ProfileDefinition("W 530x92,0", 533, 10.2, 17.3, 210),
            new ProfileDefinition("W 530x101,0", 536, 11.2, 18.8, 211),
            new ProfileDefinition("W 530x109,0", 539, 11.9, 20.3, 212),
            new ProfileDefinition("W 530x123,0", 544, 13.5, 22.9, 214),
            new ProfileDefinition("W 530x138,0", 550, 14.7, 25.9, 216),
            new ProfileDefinition("W 610x82,0", 599, 9.4, 12.7, 178),
            new ProfileDefinition("W 610x92,0", 603, 10.3, 14.7, 179),
            new ProfileDefinition("W 610x101,0", 605, 10.9, 16.1, 228),
            new ProfileDefinition("W 610x113,0", 608, 11.9, 18.0, 229),
            new ProfileDefinition("W 610x125,0", 612, 12.8, 20.1, 230),
            new ProfileDefinition("W 610x140,0", 616, 14.1, 22.2, 231),
            new ProfileDefinition("W 610x153,0", 619, 15.1, 23.9, 232),
            new ProfileDefinition("W 610x155,0", 608, 11.9, 19.3, 305),
            new ProfileDefinition("W 610x174,0", 613, 13.5, 21.6, 306),
            new ProfileDefinition("W 610x195,0", 619, 15.1, 24.5, 308),
            new ProfileDefinition("W 610x217,0", 624, 16.5, 27.0, 310)
        };
    }
}
