using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LINK
{
    /// <summary>
    /// Lógica de desenho 2D (vista lateral e frontal) equivalente ao HTML/SVG de referência.
    /// Mantém coordenadas em milímetros e aplica zoom extents automático para caber no painel.
    /// </summary>
    internal static class DrawingLogic
    {
        #region Estilos
        private static readonly Color ColorDarkGray = ColorTranslator.FromHtml("#1F2937"); // viga / solda
        private static readonly Color ColorBlue = ColorTranslator.FromHtml("#2563EB");     // pilar
        private static readonly Color ColorPlateFill = ColorTranslator.FromHtml("#A9A9A9"); // chapa
        private static readonly Color ColorPlateStroke = ColorTranslator.FromHtml("#4B5563");
        private static readonly Color ColorDimension = ColorTranslator.FromHtml("#1F2937");
        private static readonly Color ColorBoltFill = ColorTranslator.FromHtml("#1F2937");

        private static Pen PenBeam => new Pen(ColorDarkGray, 1.5f);
        private static Pen PenColumn => new Pen(ColorBlue, 1.5f);
        private static Pen PenPlate => new Pen(ColorPlateStroke, 1.5f);
        private static Pen PenDimensionLine => new Pen(ColorDimension, 1f);
        private static Pen PenWeld => new Pen(ColorDarkGray, 1.5f);

        private static Pen PenCenterLine
        {
            get
            {
                var p = new Pen(Color.Gray, 1f) { DashStyle = DashStyle.Dash };
                return p;
            }
        }

        private static Font FontDimension => new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
        private static Font FontWeld => new Font("Segoe UI", 9f, FontStyle.Italic, GraphicsUnit.Point);
        #endregion

        #region Estruturas auxiliares
        public struct ViewPort
        {
            public float Scale;
            public float OffsetX;
            public float OffsetY;
        }
        #endregion

        #region API pública
        public static void DrawSideView(Graphics g, GeometryParameters p, int canvasWidth, int canvasHeight)
        {
            if (g == null || p == null || canvasWidth <= 0 || canvasHeight <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.White);

            // Variáveis em mm (consistentes com o HTML)
            float h_w = (float)(p.BeamHeight * 1000);
            float t_f = (float)(p.BeamFlangeThickness * 1000);
            float t_p = (float)(p.PlateThickness * 1000);
            float s_w = (float)(p.WeldSize * 1000);
            float d_b = (float)(p.BoltDiameter * 1000);
            int n = p.BoltCount;
            float e_t = (float)(p.BoltEdgeTop * 1000);
            float p_v = (float)(p.BoltSpacing * 1000);
            float t_f_c = (float)(p.ColumnFlangeThickness * 1000);
            float h_w_c = (float)(p.ColumnDepth * 1000);

            // Limites para zoom extents
            float minX = -t_p - t_f_c - h_w_c - 150; // margem esquerda para cotas
            float maxX = Math.Max(200, 0.75f * h_w) + 100;
            float maxY = Math.Max(200, 0.75f * h_w_c) + 50;
            float minY = -(t_f * 2 + h_w) - Math.Max(200, 0.75f * h_w_c) - 50;

            var viewport = CalculateViewport(canvasWidth, canvasHeight, minX, maxX, minY, maxY);

            var mx = new Matrix();
            mx.Translate(viewport.OffsetX, viewport.OffsetY);
            mx.Scale(viewport.Scale, -viewport.Scale); // Y para cima
            g.Transform = mx;

            // Comprimentos auxiliares
            float L_VIGA = Math.Max(200, 0.75f * h_w);
            float L_PILAR_EXT_TOP = Math.Max(200, 0.75f * h_w_c);
            float L_PILAR_EXT_BOTTOM = Math.Max(200, 0.75f * h_w_c);

            // Viga
            DrawPolygon(g, PenBeam, Brushes.WhiteSmoke,
                new PointF(0, 0), new PointF(L_VIGA, 0), new PointF(L_VIGA, -t_f), new PointF(0, -t_f));
            DrawPolygon(g, PenBeam, Brushes.WhiteSmoke,
                new PointF(0, -t_f), new PointF(L_VIGA, -t_f), new PointF(L_VIGA, -t_f - h_w), new PointF(0, -t_f - h_w));
            DrawPolygon(g, PenBeam, Brushes.WhiteSmoke,
                new PointF(0, -t_f - h_w), new PointF(L_VIGA, -t_f - h_w), new PointF(L_VIGA, -t_f - h_w - t_f), new PointF(0, -t_f - h_w - t_f));

            // Chapa
            float y_chapa_top = -t_f;
            float y_chapa_bottom = -t_f - h_w;
            DrawPolygon(g, PenPlate, new SolidBrush(ColorPlateFill),
                new PointF(-t_p, y_chapa_top), new PointF(0, y_chapa_top), new PointF(0, y_chapa_bottom), new PointF(-t_p, y_chapa_bottom));

            // Pilar
            float y_pilar_top = L_PILAR_EXT_TOP;
            float y_pilar_bottom = -(t_f * 2 + h_w) - L_PILAR_EXT_BOTTOM;
            float x_pilar_face = -t_p;
            float x_pilar_web_direita = x_pilar_face - t_f_c;
            float x_pilar_web_esquerda = x_pilar_web_direita - h_w_c;
            float x_pilar_costa = x_pilar_web_esquerda - t_f_c;

            DrawPolygon(g, PenColumn, Brushes.AliceBlue,
                new PointF(x_pilar_web_direita, y_pilar_top), new PointF(x_pilar_face, y_pilar_top),
                new PointF(x_pilar_face, y_pilar_bottom), new PointF(x_pilar_web_direita, y_pilar_bottom));
            DrawPolygon(g, PenColumn, Brushes.AliceBlue,
                new PointF(x_pilar_web_esquerda, y_pilar_top), new PointF(x_pilar_web_direita, y_pilar_top),
                new PointF(x_pilar_web_direita, y_pilar_bottom), new PointF(x_pilar_web_esquerda, y_pilar_bottom));
            DrawPolygon(g, PenColumn, Brushes.AliceBlue,
                new PointF(x_pilar_costa, y_pilar_top), new PointF(x_pilar_web_esquerda, y_pilar_top),
                new PointF(x_pilar_web_esquerda, y_pilar_bottom), new PointF(x_pilar_costa, y_pilar_bottom));

            // Parafusos (vista lateral)
            float x_bolt_start = -t_p - t_f_c;
            float x_bolt_end = 0;
            float bolt_width = Math.Abs(x_bolt_end - x_bolt_start);

            using (var boltFill = new SolidBrush(ColorBoltFill))
            using (var boltBorder = new Pen(Color.Black, 0.5f))
            {
                for (int i = 0; i < n; i++)
                {
                    float y_bolt_center = -e_t - (i * p_v);
                    g.FillRectangle(boltFill, x_bolt_start, y_bolt_center - d_b / 2, bolt_width, d_b);
                    g.DrawRectangle(boltBorder, x_bolt_start, y_bolt_center - d_b / 2, bolt_width, d_b);
                    g.DrawLine(PenCenterLine, x_bolt_start - 30, y_bolt_center, x_bolt_end + 30, y_bolt_center);
                }
            }

            // Soldas (símbolos)
            DrawWeldSymbol(g, mx, 0, -t_f, s_w.ToString("0.#"), 45, true, true, false);
            DrawWeldSymbol(g, mx, 0, -t_f - h_w, s_w.ToString("0.#"), -45, true, true, false);
            float y_web_center = -t_f - (h_w / 2);
            DrawWeldSymbol(g, mx, 0, y_web_center, s_w.ToString("0.#"), 0, false, true, true);

            // Cotas
            g.ResetTransform();
            DrawDimensionVertical(g, mx, 0, -e_t, 60, $"{e_t:0.#}");
            if (n > 1)
            {
                float y_first = -e_t;
                float y_second = -e_t - p_v;
                DrawDimensionVertical(g, mx, y_first, y_second, x_bolt_start - 60, $"{p_v:0.#}", false);
            }

            float x_dim_viga = L_VIGA + 30;
            DrawDimensionVertical(g, mx, 0, -t_f, x_dim_viga, $"tf={t_f:0.#}");
            DrawDimensionVertical(g, mx, -t_f, -t_f - h_w, x_dim_viga, $"hw={h_w:0.#}");

            float y_dim_pilar = y_pilar_top + 30;
            DrawDimensionHorizontal(g, mx, x_pilar_costa, x_pilar_web_esquerda, y_dim_pilar, $"tf,c={t_f_c:0.#}");
            DrawDimensionHorizontal(g, mx, x_pilar_web_esquerda, x_pilar_web_direita, y_dim_pilar, $"hw,c={h_w_c:0.#}");

            mx.Dispose();
        }

        public static void DrawFrontView(Graphics g, GeometryParameters p, int canvasWidth, int canvasHeight)
        {
            if (g == null || p == null || canvasWidth <= 0 || canvasHeight <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.White);

            float h_w = (float)(p.BeamHeight * 1000);
            float t_f = (float)(p.BeamFlangeThickness * 1000);
            float t_w = (float)(p.BeamWebThickness * 1000);
            float l_p = (float)(p.PlateWidth * 1000);
            float g_h = (float)(p.BoltGauge * 1000);
            float d_b = (float)(p.BoltDiameter * 1000);
            int n = p.BoltCount;
            float e_t = (float)(p.BoltEdgeTop * 1000);
            float p_v = (float)(p.BoltSpacing * 1000);

            // Derivados (HTML)
            float e_h = (l_p - g_h) / 2.0f;
            float e_t_p = e_t - t_f;
            float e_b_p = h_w - e_t_p - ((n - 1) * p_v);

            float margin = 80f;
            float minX = -l_p / 2 - margin;
            float maxX = l_p / 2 + margin;
            float maxY = h_w / 2 + margin;
            float minY = -h_w / 2 - margin;

            var viewport = CalculateViewport(canvasWidth, canvasHeight, minX, maxX, minY, maxY);

            var mx = new Matrix();
            mx.Translate(viewport.OffsetX, viewport.OffsetY);
            mx.Scale(viewport.Scale, -viewport.Scale);
            g.Transform = mx;

            float r_b = d_b / 2;

            // Alma (tracejado)
            using (var penDashed = new Pen(ColorDarkGray, 1.5f) { DashStyle = DashStyle.Dash })
            {
                g.DrawRectangle(penDashed, -t_w / 2, -h_w / 2, t_w, h_w);
            }

            // Chapa
            using (var platePath = new GraphicsPath())
            {
                platePath.AddRectangle(new RectangleF(-l_p / 2, -h_w / 2, l_p, h_w));
                using (var plateFill = new SolidBrush(Color.FromArgb(128, 211, 211, 211)))
                {
                    g.FillPath(plateFill, platePath);
                    g.DrawPath(PenPlate, platePath);
                }
            }

            // Parafusos
            float y_top_plate = h_w / 2;
            float y_first_bolt = y_top_plate - e_t_p;
            float x_col_left = -g_h / 2;
            float x_col_right = g_h / 2;
            float y_last_bolt = y_first_bolt;

            for (int i = 0; i < n; i++)
            {
                float y_bolt = y_first_bolt - (i * p_v);
                y_last_bolt = y_bolt;

                g.FillEllipse(Brushes.White, x_col_left - r_b, y_bolt - r_b, d_b, d_b);
                g.DrawEllipse(Pens.Black, x_col_left - r_b, y_bolt - r_b, d_b, d_b);

                g.FillEllipse(Brushes.White, x_col_right - r_b, y_bolt - r_b, d_b, d_b);
                g.DrawEllipse(Pens.Black, x_col_right - r_b, y_bolt - r_b, d_b, d_b);
            }

            // Cotas
            g.ResetTransform();

            DrawDimensionHorizontal(g, mx, -l_p / 2, l_p / 2, h_w / 2 + 30, $"{l_p:0.#}");
            DrawDimensionHorizontal(g, mx, -g_h / 2, g_h / 2, -h_w / 2 - 30, $"{g_h:0.#}");

            float y_dim_eh = -h_w / 2 - 60;
            DrawDimensionHorizontal(g, mx, -l_p / 2, -g_h / 2, y_dim_eh, $"{e_h:0.#}");
            DrawDimensionHorizontal(g, mx, g_h / 2, l_p / 2, y_dim_eh, $"{e_h:0.#}");

            float x_dim_side = l_p / 2 + 40;
            DrawDimensionVertical(g, mx, y_top_plate, y_first_bolt, x_dim_side, $"{e_t_p:0.#}");
            DrawDimensionVertical(g, mx, y_last_bolt, -h_w / 2, x_dim_side, $"{e_b_p:0.#}");

            if (n > 1)
            {
                float x_dim_pv = -l_p / 2 - 40;
                DrawDimensionVertical(g, mx, y_first_bolt, y_first_bolt - p_v, x_dim_pv, $"{p_v:0.#}", false);
            }

            mx.Dispose();
        }
        #endregion

        #region Helpers
        private static void DrawPolygon(Graphics g, Pen pen, Brush brush, params PointF[] points)
        {
            if (brush != null) g.FillPolygon(brush, points);
            if (pen != null) g.DrawPolygon(pen, points);
        }

        private static void DrawDimensionVertical(Graphics g, Matrix mx, float yWorld1, float yWorld2, float xWorldLine, string text, bool alignRight = true)
        {
            PointF[] pts = { new PointF(xWorldLine, yWorld1), new PointF(xWorldLine, yWorld2) };
            mx.TransformPoints(pts);

            float screenX = pts[0].X;
            float screenY1 = pts[0].Y;
            float screenY2 = pts[1].Y;

            g.DrawLine(PenDimensionLine, screenX, screenY1, screenX, screenY2);

            const float tick = 4f;
            g.DrawLine(PenDimensionLine, screenX - tick, screenY1, screenX + tick, screenY1);
            g.DrawLine(PenDimensionLine, screenX - tick, screenY2, screenX + tick, screenY2);

            float midY = (screenY1 + screenY2) / 2;
            SizeF sz = g.MeasureString(text, FontDimension);

            float textX = alignRight ? screenX + 5 : screenX - sz.Width - 5;
            g.DrawString(text, FontDimension, Brushes.Black, textX, midY - sz.Height / 2);
        }

        private static void DrawDimensionHorizontal(Graphics g, Matrix mx, float xWorld1, float xWorld2, float yWorldLine, string text)
        {
            PointF[] pts = { new PointF(xWorld1, yWorldLine), new PointF(xWorld2, yWorldLine) };
            mx.TransformPoints(pts);

            float screenY = pts[0].Y;
            float screenX1 = pts[0].X;
            float screenX2 = pts[1].X;

            g.DrawLine(PenDimensionLine, screenX1, screenY, screenX2, screenY);

            const float tick = 4f;
            g.DrawLine(PenDimensionLine, screenX1, screenY - tick, screenX1, screenY + tick);
            g.DrawLine(PenDimensionLine, screenX2, screenY - tick, screenX2, screenY + tick);

            float midX = (screenX1 + screenX2) / 2;
            SizeF sz = g.MeasureString(text, FontDimension);
            g.DrawString(text, FontDimension, Brushes.Black, midX - sz.Width / 2, screenY - sz.Height - 2);
        }

        private static ViewPort CalculateViewport(int canvasW, int canvasH, float minX, float maxX, float minY, float maxY)
        {
            float contentW = maxX - minX;
            float contentH = maxY - minY;

            if (contentW < 1) contentW = 100;
            if (contentH < 1) contentH = 100;

            float scaleX = (canvasW * 0.9f) / contentW;
            float scaleY = (canvasH * 0.9f) / contentH;
            float scale = Math.Min(scaleX, scaleY);

            float midX = (minX + maxX) / 2;
            float midY = (minY + maxY) / 2;

            float offsetX = (canvasW / 2f) - (midX * scale);
            float offsetY = (canvasH / 2f) + (midY * scale);

            return new ViewPort { Scale = scale, OffsetX = offsetX, OffsetY = offsetY };
        }

        private static void DrawWeldSymbol(Graphics g, Matrix mx, float xJoint, float yJoint, string text,
            float angleDeg, bool directionLeft, bool arrowSide, bool bothSides)
        {
            PointF[] pts = { new PointF(xJoint, yJoint) };
            mx.TransformPoints(pts);
            float jx = pts[0].X;
            float jy = pts[0].Y;

            float refLength = 40f;
            float elbowOffsetX = directionLeft ? -25f : 25f;
            float elbowOffsetY = (float)(Math.Tan(angleDeg * Math.PI / 180.0) * Math.Abs(elbowOffsetX));
            if (angleDeg != 0) elbowOffsetY = -elbowOffsetY;

            float ex = jx + elbowOffsetX;
            float ey = jy + elbowOffsetY;
            float rx = ex + (directionLeft ? -refLength : refLength);
            float ry = ey;

            using (var path = new GraphicsPath())
            {
                path.AddLine(jx, jy, ex, ey);
                path.AddLine(ex, ey, rx, ry);
                g.DrawPath(PenWeld, path);
            }

            float arrowSize = 4f;
            float dx = jx - ex;
            float dy = jy - ey;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len > 0) { dx /= len; dy /= len; }

            float px = -dy * arrowSize;
            float py = dx * arrowSize;

            float backX = jx - dx * (arrowSize * 2.5f);
            float backY = jy - dy * (arrowSize * 2.5f);

            PointF[] arrowPts = {
                new PointF(jx, jy),
                new PointF(backX + px, backY + py),
                new PointF(backX - px, backY - py)
            };
            g.FillPolygon(Brushes.Black, arrowPts);

            float triSize = 8f;
            float symbolX = rx - (directionLeft ? -10 : 10);

            if (arrowSide)
            {
                g.FillPolygon(Brushes.Black, new PointF[]
                {
                    new PointF(symbolX, ry),
                    new PointF(symbolX, ry + triSize),
                    new PointF(symbolX - triSize, ry)
                });
            }
            if (bothSides)
            {
                g.FillPolygon(Brushes.Black, new PointF[]
                {
                    new PointF(symbolX, ry),
                    new PointF(symbolX, ry - triSize),
                    new PointF(symbolX - triSize, ry)
                });
            }

            SizeF txtSz = g.MeasureString(text, FontWeld);
            float txtX = symbolX + (directionLeft ? 5 : -5) - txtSz.Width;
            float txtY = ry - triSize - txtSz.Height - 2;
            if (!bothSides && arrowSide) txtY = ry - txtSz.Height - 2;

            g.DrawString(text, FontWeld, Brushes.Black, txtX, txtY);
        }
        #endregion
    }
}
