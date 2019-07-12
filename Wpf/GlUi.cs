using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Wpf
{
    static class glObgectCounter
    {
        public static int redCounter = 255;
    }

    public class GlShape
    {
        public virtual void Draw()
        {

        }
    }

    public class LegGl: GlShape
    {
        public int red;
        public float size = 10;

        private Leg leg;
        public Leg Leg
        {
            get { return leg; }
        }

        public LegGl(Leg leg)
        {
            this.leg = leg;
            red = glObgectCounter.redCounter--;
        }

        public override void Draw()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            var mid = (leg.Position + leg.Prev.Position) / 2;
            mid = GlConverter.ToGlVector2(mid);
            GL.Translate(mid.X, mid.Y, 0);
            GL.Rotate(-((float)leg.Angle - 90), Vector3.UnitZ);
            GL.Scale((leg.Length - size) / 2, size/4, 1);

            GL.Begin(PrimitiveType.Polygon);

            GL.Color3(System.Drawing.Color.Green);
            GL.Vertex2(1, 1);
            GL.Vertex2(1, -1);
            GL.Vertex2(-1, -1);
            GL.Vertex2(-1, 1);

            GL.End();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            var v = GlConverter.ToGlVector2(leg.Position);
            GL.Translate(v.X, v.Y, 0);

            GL.Begin(PrimitiveType.Polygon );
            GL.Color3(System.Drawing.Color.FromArgb(red, 0, 0));
            double step = Math.PI / 20;
            double angle = 0.0;
            while (angle < Math.PI * 2)
            {               
                GL.Vertex2(size * Math.Sin(angle), size * Math.Cos(angle));
                angle += step;
            }
            GL.End();
        }
    }

    public class SliderGl: GlShape
    {
        public int red;
        public float size = 10;

        private Slider slider;
        public Slider Slider
        {
            get { return slider; }
        }

        public SliderGl(Slider slider)
        {
            this.slider = slider;
            red = glObgectCounter.redCounter--;
        }

        public override void Draw()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Translate(GlConverter.ToGlVector3(slider.center));
            GL.Scale(slider.width , size / 4, 1);

            GL.Begin(PrimitiveType.Polygon);

            GL.Color3(System.Drawing.Color.BlueViolet);
            GL.Vertex2(1, 1);
            GL.Vertex2(1, -1);
            GL.Vertex2(-1, -1);
            GL.Vertex2(-1, 1);

            GL.End();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            var left = GlConverter.ToGlVector3(slider.center);
            left.X -= slider.width;
            GL.Translate(left);
            GL.Scale(size, size, 0);

            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(System.Drawing.Color.Black);
            GL.Vertex2(0, 0);
            GL.Vertex2(1, 1);
            GL.Vertex2(-1, 1);
            GL.End();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            var right = GlConverter.ToGlVector3(slider.center);
            right.X += slider.width;
            GL.Translate(right);
            GL.Scale(size, size, 0);

            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(System.Drawing.Color.Black);
            GL.Vertex2(0, 0);
            GL.Vertex2(1, 1);
            GL.Vertex2(-1, 1);
            GL.End();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            var v = GlConverter.ToGlVector2(slider.Position);
            GL.Translate(v.X, v.Y, 0);

            GL.Begin(PrimitiveType.Polygon);
            GL.Color3(System.Drawing.Color.FromArgb(red, 0, 0));
            double step = Math.PI / 20;
            double angle = 0.0;
            while (angle < Math.PI * 2)
            {
                GL.Vertex2(size * Math.Sin(angle), size * Math.Cos(angle));
                angle += step;
            }
            GL.End();
        }
    }
}
