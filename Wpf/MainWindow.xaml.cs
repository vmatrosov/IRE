using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Wpf
{
    public static class GlConverter
    {
        public static float width = 0;
        public static float heigth = 0;

        public static Vector2d ToGlVector2(Vector2d p)
        {
            float halfWidth = width / 2;
            float halfHeigth = heigth / 2;

            return new Vector2d((p.X - halfWidth), (p.Y - halfHeigth));
        }

        public static Vector3d ToGlVector3(Vector2d p)
        {
            float halfWidth = width / 2;
            float halfHeigth = heigth / 2;

            return new Vector3d((p.X - halfWidth), (p.Y - halfHeigth), 0);
        }

        public static Point ToWpfCoord(Vector2 p)
        {
            double halfWidth = width / 2;
            double halfHeigth = heigth / 2;

            return new Point(p.X + halfWidth, p.Y + halfHeigth);
        }
    }

    public partial class MainWindow : Window
    {
        NodeShape captNodeShape = null;
        NodeGlShape captGlShape = null;
        GLControl glControl;
        Slider slider;
        List<NodeShape> shapeList = new List<NodeShape>();
        List<NodeGlShape> glList = new List<NodeGlShape>();

        public MainWindow()
        {
            InitializeComponent();

            // WPF
            canvas.MouseLeftButtonUp += ShapeMouseLeftButtonUp;

            // GL
            var flags = GraphicsContextFlags.Default;
            glControl = new GLControl(new GraphicsMode(32, 24), 2, 0, flags);
            glControl.MakeCurrent();
            glControl.Paint += GlControl_Paint;
            glControl.Dock = DockStyle.Fill;
            wfh.Child = glControl;
            glControl.Resize += GlControl_Resize;
            glControl.MouseDown += GlControl_MouseClick;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseMove += GlControl_MouseMove;

            // MODEL
            slider = new Slider();
            slider.Position = slider.center = new Vector2d(200, 200);
            AddShapeToCanves(new SliderShape(slider));
            glList.Add(new SliderGlShape(slider));
        }

        void rendertriangle()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            float halfWidth = (float)(glControl.Width / 2);
            float halfHeight = (float)(glControl.Height / 2);
            GL.Ortho(-halfWidth, halfWidth, halfHeight, -halfHeight, 1000, -1000);
            GL.Viewport(glControl.Size);

            foreach (var v in glList)
                v.Draw();
        }

        void AddShapeToCanves(NodeShape ns)
        {
            shapeList.Add(ns);

            if (ns is LegShape)
            {
                canvas.Children.Add((ns as LegShape).line);
                canvas.Children.Add((ns as LegShape).point);
            }

            if (ns is SliderShape)
            {
                canvas.Children.Add((ns as SliderShape).line);
                canvas.Children.Add((ns as SliderShape).point);
            }

            ns.MouseLeftButtonDown += ShapeMouseLeftButtonDown;
            ns.MouseLeftButtonUp += ShapeMouseLeftButtonUp;
        }

        void RemoveLegShape(NodeShape ns)
        {
            if (ns is LegShape)
            {
                canvas.Children.Remove((ns as LegShape).line);
                canvas.Children.Remove((ns as LegShape).point);
            }

            if (ns is SliderShape)
            {
                canvas.Children.Remove((ns as SliderShape).line);
                canvas.Children.Remove((ns as SliderShape).point);
            }
        }

        void AddLeg(Node parent)
        {
            while (parent.Next != null)
                parent = parent.Next;

            var leg = new Leg(parent, 45, 100);

            parent.Next = leg;
            AddShapeToCanves(new LegShape(leg));
            glList.Add(new LegGlShape(leg));
        }

        void RemoveLeg(Node n)
        {
            while (n.Next != null)
                n = n.Next;
            n.Prev.Next = null;

            RemoveLegShape(shapeList.Where(e => e.Node == n).First());
            glList.Remove(glList.Where(e => e.Node == n).First());
        }

        #region WPF event

        private void canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var pos = e.GetPosition(sender as Canvas);

            tbX.Text = pos.X.ToString();
            tbY.Text = pos.Y.ToString();

            if (captNodeShape != null)
            {
                if (captNodeShape is LegShape)
                {
                    (captNodeShape as LegShape).Leg.SetPosition(new Vector2d(pos.X, pos.Y));
                    (captNodeShape as LegShape).Leg.UpdateRelativePos();
                }

                if (captNodeShape is SliderShape)
                {
                    (captNodeShape as SliderShape).Slider.SetPosition(new Vector2d(pos.X, pos.Y));
                    (captNodeShape as SliderShape).Slider.UpdateRelativePos();
                }

            }
        }

        private void ShapeMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            captNodeShape = sender as NodeShape;

            if (sender is LegShape)
            {
                gbLegData.DataContext = (sender as LegShape).Leg;
            }
        }

        private void ShapeMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            captNodeShape = null;
        }

        #endregion

        #region GL event

        private void GlControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (captGlShape != null)
            {
                if (captGlShape is LegGlShape)
                {
                    (captGlShape as LegGlShape).Leg.SetPosition(new Vector2d(e.X, e.Y));
                    (captGlShape as LegGlShape).Leg.UpdateRelativePos();
                }

                if (captGlShape is SliderGlShape)
                {
                    (captGlShape as SliderGlShape).Slider.SetPosition(new Vector2d(e.X, e.Y));
                    (captGlShape as SliderGlShape).Slider.UpdateRelativePos();
                }

                glControl.Invalidate();
            }
        }

        private void GlControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            captGlShape = null;
        }

        private void GlControl_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                tbX.Text = e.X.ToString();
                tbY.Text = e.Y.ToString();

                int x = e.X;
                int y = glControl.Height - e.Y;

                byte[] pixels = new byte[4];

                glControl.Invalidate();
                GL.ReadPixels(x, y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

                tbPixR.Text = pixels[0].ToString();
                tbPixG.Text = pixels[1].ToString();
                tbPixB.Text = pixels[2].ToString();

                captGlShape = glList.Where(ee => ee.HashColor == pixels[0]).FirstOrDefault();
                if (captGlShape != null)
                    if (captGlShape is LegGlShape)
                        gbLegData.DataContext = (captGlShape as LegGlShape).Leg;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddLeg(slider);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RemoveLeg(slider);
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GlConverter.heigth = (sender as GLControl).Height;
            GlConverter.width = (sender as GLControl).Width;
        }

        private void GlControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

            GL.ClearColor(0, 1, 1, 1);
            GL.Clear(
                ClearBufferMask.ColorBufferBit |
                ClearBufferMask.DepthBufferBit |
                ClearBufferMask.StencilBufferBit);

            rendertriangle();

            glControl.SwapBuffers();
        }

        #endregion
    }
}

