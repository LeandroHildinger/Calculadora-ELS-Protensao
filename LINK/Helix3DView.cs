using System;
using System.Collections.Generic;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace LINK
{
    internal sealed class Helix3DHost : ElementHost
    {
        private readonly HelixViewport3D viewport;
        private readonly ModelVisual3D sceneRoot;
        // Transformação para alinhar o Y (Geometria) ao Z (Viewport), igual ao MainWindow
        private readonly Transform3D _geometryCorrectionTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));

        public Helix3DHost()
        {
            viewport = new HelixViewport3D
            {
                ShowViewCube = true,
                ShowCoordinateSystem = true,
                ZoomExtentsWhenLoaded = true,
                CameraInertiaFactor = 0.75,
                RotateGesture = new System.Windows.Input.MouseGesture(System.Windows.Input.MouseAction.LeftClick),
                PanGesture = new System.Windows.Input.MouseGesture(System.Windows.Input.MouseAction.MiddleClick),
                ZoomGesture = null, // Zoom geralmente é roda do mouse por padrão no Helix
                ZoomSensitivity = 1.0
            };

            // CORREÇÃO 1: Define Z como o eixo vertical (Up) para o controle de rotação (Turntable)
            viewport.ModelUpDirection = new Vector3D(0, 0, 1);

            viewport.Camera = new PerspectiveCamera
            {
                Position = new Point3D(4, 2.8, 4),      // Posição ajustada similar ao MainWindow
                LookDirection = new Vector3D(-4, -1.5, -4),
                UpDirection = new Vector3D(0, 0, 1),    // Câmera aponta Z para cima
                FieldOfView = 45
            };

            viewport.Children.Add(new SunLight());

            sceneRoot = new ModelVisual3D
            {
                // CORREÇÃO 2: Aplica a rotação na raiz da cena para levantar a estrutura (Y -> Z)
                Transform = _geometryCorrectionTransform
            };

            viewport.Children.Add(sceneRoot);

            Dock = System.Windows.Forms.DockStyle.Fill;
            Child = viewport;
        }

        public void SetScene(Model3DGroup scene)
        {
            if (sceneRoot == null) return;
            sceneRoot.Content = scene;
            // O Transform já está aplicado na propriedade sceneRoot.Transform no construtor
        }

        public void ResetCamera()
        {
            if (viewport?.Camera is PerspectiveCamera cam)
            {
                cam.Position = new Point3D(4, 2.8, 4);
                cam.LookDirection = new Vector3D(-4, -1.5, -4);
                cam.UpDirection = new Vector3D(0, 0, 1);
            }
            viewport?.ZoomExtents();
        }
    }

    internal sealed class HelixSceneBuilder
    {
        // Lógica de normalização mantida intacta
        private static GeometryParameters Normalize(GeometryParameters p) => new GeometryParameters
        {
            BeamLength = Math.Max(0.1, 0.75 * p.BeamHeight),
            BeamHeight = p.BeamHeight,
            BeamThickness = p.BeamThickness,
            BeamFlangeThickness = p.BeamFlangeThickness,
            BeamWebThickness = p.BeamWebThickness,
            ColumnDepth = p.ColumnDepth,
            ColumnWidth = p.ColumnWidth,
            ColumnFlangeThickness = p.ColumnFlangeThickness,
            ColumnWebThickness = p.ColumnWebThickness,
            ColumnWebWidth = p.ColumnWebWidth,
            PlateHeight = Math.Max(p.PlateHeight, p.BeamHeight),
            PlateWidth = p.PlateWidth,
            PlateThickness = p.PlateThickness,
            PlateEdgeHorizontal = p.PlateEdgeHorizontal,
            PlateEdgeTop = p.PlateEdgeTop,
            PlateEdgeBottom = p.PlateEdgeBottom,
            BoltCount = p.BoltCount,
            BoltSpacing = p.BoltSpacing,
            BoltDiameter = p.BoltDiameter,
            BoltEdgeTop = p.BoltEdgeTop,
            BoltGauge = p.BoltGauge,
            WeldSize = p.WeldSize
        };

        public Model3DGroup BuildScene(GeometryParameters pRaw)
        {
            var p = Normalize(pRaw);
            var scene = new Model3DGroup();
            scene.Children.Add(BuildBeam(p));
            scene.Children.Add(BuildPlate(p));
            scene.Children.Add(BuildColumn(p));
            foreach (var bolt in BuildBolts(p))
            {
                scene.Children.Add(bolt);
            }
            var welds = BuildWelds(p);
            if (welds != null) scene.Children.Add(welds);
            return scene;
        }

        // --- Métodos de Construção Geométrica (Geometria local: Y para baixo na raiz, mas o host gira Y->Z) ---

        private static GeometryModel3D BuildBeam(GeometryParameters p)
        {
            var mesh = new MeshBuilder();
            var hw = p.BeamHeight;
            var tf = p.BeamFlangeThickness;
            var tw = p.BeamWebThickness;
            var bf = p.BeamThickness;
            var len = p.BeamLength;

            mesh.AddBox(new Point3D(len / 2, -tf / 2, 0), len, tf, bf);
            mesh.AddBox(new Point3D(len / 2, -(tf + hw / 2), 0), len, hw, tw);
            mesh.AddBox(new Point3D(len / 2, -(tf + hw + tf / 2), 0), len, tf, bf);

            return CreateMetalModel(mesh, Color.FromRgb(31, 41, 55));
        }

        private static GeometryModel3D BuildPlate(GeometryParameters p)
        {
            var tp = p.PlateThickness;
            var hp = p.PlateHeight;
            var lp = p.PlateWidth;
            var tf = p.BeamFlangeThickness;

            var center = new Point3D(-tp / 2, -(tf + hp / 2), 0);
            var mesh = new MeshBuilder();
            mesh.AddBox(center, tp, hp, lp);
            return CreateMetalModel(mesh, Color.FromRgb(169, 169, 169));
        }

        private static GeometryModel3D BuildColumn(GeometryParameters p)
        {
            var tp = p.PlateThickness;
            var tf = p.BeamFlangeThickness;
            var hw = p.BeamHeight;

            var hwc = p.ColumnDepth;
            var tfc = p.ColumnFlangeThickness;
            var twc = p.ColumnWebThickness;
            var bfc = p.ColumnWidth;

            var yMax = 0.75 * hwc;
            var yMin = -(tf + hw + tf + 0.75 * hwc);
            var hCol = yMax - yMin;
            var yCenter = (yMax + yMin) / 2.0;

            var mesh = new MeshBuilder();
            mesh.AddBox(new Point3D(-tp - tfc / 2.0, yCenter, 0), tfc, hCol, bfc);
            mesh.AddBox(new Point3D(-tp - tfc - hwc / 2.0, yCenter, 0), hwc, hCol, twc);
            mesh.AddBox(new Point3D(-tp - tfc - hwc - tfc / 2.0, yCenter, 0), tfc, hCol, bfc);

            return CreateMetalModel(mesh, Color.FromRgb(37, 99, 235));
        }

        private static IEnumerable<Model3D> BuildBolts(GeometryParameters p)
        {
            var models = new List<Model3D>();
            if (p.BoltCount <= 0) return models;

            const double clearance = 0.005;
            var sectionDepth = Math.Max(p.PlateThickness, 0.001) + Math.Max(p.ColumnFlangeThickness, 0.001);
            var startX = -sectionDepth - clearance;
            var endX = clearance;
            var radius = Math.Max(p.BoltDiameter / 2.0, 0.001);
            var gauge = Math.Max(p.BoltGauge, 0.0);
            var spacing = Math.Max(p.BoltSpacing, 0.0);

            var tf = p.BeamFlangeThickness;
            var plateTopY = -tf;
            var firstRowY = plateTopY - Math.Max(p.BoltEdgeTop, 0.0);
            var offsetsZ = gauge > 0.0 ? new[] { -gauge / 2.0, gauge / 2.0 } : new[] { 0.0 };

            for (var i = 0; i < p.BoltCount; i++)
            {
                var y = firstRowY - i * spacing;
                foreach (var z in offsetsZ)
                {
                    var mesh = new MeshBuilder();
                    mesh.AddCylinder(new Point3D(startX, y, z), new Point3D(endX, y, z), radius, 14, true, true);
                    models.Add(CreateMetalModel(mesh, Color.FromRgb(224, 231, 240)));
                }
            }
            return models;
        }

        private static Model3D BuildWelds(GeometryParameters p)
        {
            var s = p.WeldSize;
            if (s <= 0) return null;

            var tf = p.BeamFlangeThickness;
            var hw = p.BeamHeight;
            var bf = p.BeamThickness;
            var tw = p.BeamWebThickness;

            var group = new Model3DGroup();
            var color = Color.FromRgb(253, 126, 20);

            void AddPrism(Point3D p1, Point3D p2, Point3D p3, Point3D p4, Point3D p5, Point3D p6)
            {
                var builder = new MeshBuilder();
                builder.AddTriangle(p1, p2, p3);
                builder.AddTriangle(p4, p6, p5);
                builder.AddQuad(p1, p3, p6, p4);
                builder.AddQuad(p2, p5, p6, p3);
                builder.AddQuad(p1, p4, p5, p2);
                group.Children.Add(CreateMetalModel(builder, color));
            }

            // Fillets conforme lógica original (Geometria local)
            // Fillet 1
            AddPrism(new Point3D(0, -tf, +0.5 * bf), new Point3D(-s, -tf, +0.5 * bf), new Point3D(0, -tf + s, +0.5 * bf),
                     new Point3D(0, -tf, -0.5 * bf), new Point3D(-s, -tf, -0.5 * bf), new Point3D(0, -tf + s, -0.5 * bf));
            // Fillet 4
            AddPrism(new Point3D(0, -tf - hw, +0.5 * bf), new Point3D(-s, -tf - hw, +0.5 * bf), new Point3D(0, -tf - hw - s, +0.5 * bf),
                     new Point3D(0, -tf - hw, -0.5 * bf), new Point3D(-s, -tf - hw, -0.5 * bf), new Point3D(0, -tf - hw - s, -0.5 * bf));
            // Fillet 2
            AddPrism(new Point3D(0, -tf, +0.5 * tw), new Point3D(s, -tf, +0.5 * tw), new Point3D(0, -tf + s, +0.5 * tw + s),
                     new Point3D(0, -tf - hw, +0.5 * tw), new Point3D(s, -tf - hw, +0.5 * tw), new Point3D(0, -tf - hw, +0.5 * tw + s));
            // Fillet 3
            AddPrism(new Point3D(0, -tf, -0.5 * tw), new Point3D(s, -tf, -0.5 * tw), new Point3D(0, -tf + s, -0.5 * tw - s),
                     new Point3D(0, -tf - hw, -0.5 * tw), new Point3D(s, -tf - hw, -0.5 * tw), new Point3D(0, -tf - hw, -0.5 * tw - s));

            return group;
        }

        private static GeometryModel3D CreateMetalModel(MeshBuilder builder, Color color)
        {
            var mesh = builder.ToMesh(true);
            var material = new DiffuseMaterial(new SolidColorBrush(color));
            var specular = new SpecularMaterial(new SolidColorBrush(Colors.White), 30);
            var group = new MaterialGroup();
            group.Children.Add(material);
            group.Children.Add(specular);

            return new GeometryModel3D
            {
                Geometry = mesh,
                Material = group,
                BackMaterial = group
            };
        }
    }
}
