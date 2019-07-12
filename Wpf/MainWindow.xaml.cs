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

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NodeShape captureEl = null;
        GLControl glControl;
        LegGl lg1, lg2, lg3;
        SliderGl slgl;

        Dictionary<int, GlShape> glObjects = new Dictionary<int, GlShape>();
        GlShape captureGl = null;

        public MainWindow()
        {
            InitializeComponent();
            
            Slider sl = new Slider();
            sl.Position = sl.center = new Vector2d(200, 200);

            Leg leg1 = new Leg(sl, 90, 100);
            Leg leg2 = new Leg(leg1, 90, 100);
            Leg leg3 = new Leg(leg2, 90, 100);

            SliderShape sls = new SliderShape(sl);
            LegShape ls1 = new LegShape(leg1);
            LegShape ls2 = new LegShape(leg2);
            LegShape ls3 = new LegShape(leg3);

            slgl = new SliderGl(sl);
            lg1 = new LegGl(leg1);
            lg2 = new LegGl(leg2);
            lg3 = new LegGl(leg3);

            glObjects.Add(slgl.red, slgl);
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
                if(captureGl is LegGl)
                {
                    (captureGl as LegGl) .Leg.SetPosition(new Vector2d(e.X, e.Y));
                    (captureGl as LegGl).Leg.UpdateRelativePos();
                }

                if(captureGl  is SliderGl)
                {
                    (captureGl as SliderGl).Slider.SetPosition(new Vector2d(e.X, e.Y));
                    (captureGl as SliderGl).Slider.UpdateRelativePos();
                }



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

                glControl.Invalidate();
                GL.ReadPixels(x, y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

                tbPixR.Text = pixels[0].ToString();
                tbPixG.Text = pixels[1].ToString();
                tbPixB.Text = pixels[2].ToString();
                tbPixA.Text = pixels[3].ToString();

                var res = glObjects.TryGetValue(pixels[0], out captureGl);

                if (captureGl != null)
                    if (captureGl is LegGl)
                {
                    gbLegData.DataContext = (captureGl as LegGl).Leg;
                }
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
            GL.Ortho(-halfWidth, halfWidth, halfHeight, -halfHeight, 1000, -1000);
            GL.Viewport(glControl.Size);

            slgl.Draw();
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
                    (captureEl as LegShape).Leg.SetPosition( new Vector2d(pos.X, pos.Y));
                    (captureEl as LegShape).Leg.UpdateRelativePos();
                }

                if (captureEl is SliderShape)
                {
                    (captureEl as SliderShape).Slider.SetPosition(new Vector2d(pos.X, pos.Y));
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

