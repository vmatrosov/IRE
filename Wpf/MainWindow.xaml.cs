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

        public static Vector2 ToGlCoord(Point p)
        {
            float halfWidth = width / 2;
            float halfHeigth = heigth / 2;

            return new Vector2((float)(p.X - halfWidth) / halfWidth, -(float)(p.Y - halfHeigth) / halfHeigth);
        }

        public static Point ToWpfwCoord(Vector2 p)
        {
            double halfWidth = width / 2;
            double halfHeigth = heigth / 2;

            return new Point(p.X * halfWidth + halfWidth, p.Y + halfHeigth * -halfHeigth);
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NodeShape captureEl = null;
        GLControl glControl;
        LegGl lg1, lg2, lg3;

        Dictionary<int, LegGl> glObjects = new Dictionary<int, LegGl>();
        LegGl captureGl = null;

        public MainWindow()
        {
            InitializeComponent();
            
            Slider sl = new Slider();
            sl.Position = sl.center = new Point(200, 200);

            Leg leg1 = new Leg(sl, 90, 100);
            Leg leg2 = new Leg(leg1, 90, 100);
            Leg leg3 = new Leg(leg2, 90, 100);

            SliderShape sls = new SliderShape(sl);
            LegShape ls1 = new LegShape(leg1);
            LegShape ls2 = new LegShape(leg2);
            LegShape ls3 = new LegShape(leg3);

            lg1 = new LegGl(leg1);
            lg2 = new LegGl(leg2);
            lg3 = new LegGl(leg3);

            glObjects.Add(lg1.red, lg1);
            glObjects.Add(lg2.red, lg2);
            glObjects.Add(lg3.red, lg3);

            AddLegShape(sls);
            AddLegShape(ls1);
            AddLegShape(ls2);
            AddLegShape(ls3);

            canvas.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;

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
        }

        private void GlControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(captureGl != null)
            {
                captureGl.Leg.SetPosition(new Point(e.X, e.Y));
                captureGl.Leg.UpdateRelativePos();

                glControl.Invalidate();
            }
        }

        private void GlControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            captureGl = null;
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
                GL.ReadPixels(x, y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

                tbPixR.Text = pixels[0].ToString();
                tbPixG.Text = pixels[1].ToString();
                tbPixB.Text = pixels[2].ToString();
                tbPixA.Text = pixels[3].ToString();

                var res = glObjects.TryGetValue(pixels[0], out captureGl);
            }
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            GlConverter.heigth = (sender as GLControl).Height ;
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

        void rendertriangle()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            float halfWidth = (float)(glControl.Width / 2);
            float halfHeight = (float)(glControl.Height / 2);
            //GL.Ortho(-halfWidth, halfWidth, halfHeight, -halfHeight, 1000, -1000);
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
            GL.Viewport(glControl.Size);

            //GL.Begin(PrimitiveType.Triangles);
            //GL.Color3(255, 0, 0);
            //GL.Vertex2(-1.0f, 1.0f);
            //GL.Color3(System.Drawing.Color.SpringGreen);
            //GL.Vertex2(0.0f, -1.0f);
            //GL.Color3(System.Drawing.Color.Ivory);
            //GL.Vertex2(1.0f, 1.0f);
            //GL.End();

            lg1.Draw();
            lg2.Draw();
            lg3.Draw();
        }

        void AddLegShape(NodeShape ns)
        {

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

            ns.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
            ns.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
        }

        private void canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var pos = e.GetPosition(sender as Canvas);
            tbX.Text = pos.X.ToString();
            tbY.Text = pos.Y.ToString();

            if (captureEl != null)
            {
                if (captureEl is LegShape)
                {
                    (captureEl as LegShape).Leg.SetPosition( pos);
                    (captureEl as LegShape).Leg.UpdateRelativePos();
                }

                if (captureEl is SliderShape)
                {
                    (captureEl as SliderShape).Slider.SetPosition( pos);
                    (captureEl as SliderShape).Slider.UpdateRelativePos();
                }

            }
        }

        private void Ellipse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            captureEl = sender as NodeShape;

            if (sender is LegShape)
            {
                gbLegData.DataContext = (sender as LegShape).Leg;
            }
        }

        private void Ellipse_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            captureEl = null;
        }
    }
}

