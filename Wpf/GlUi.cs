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
            mid = GlConverter.ToGlCoord(mid);
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

            var v = GlConverter.ToGlCoord(leg.Position);
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

            var  mid = GlConverter.ToGlCoord(slider.center);
            GL.Translate(mid.X, mid.Y, 0);
            //GL.Rotate(-((float)leg.Angle - 90), Vector3.UnitZ);
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

            var v = GlConverter.ToGlCoord(slider.Position);
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
