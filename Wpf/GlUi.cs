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
    public abstract class NodeGlShape
    {
        private static int counter = 0;
        protected int Counter
        {
            get { return counter; }
        }

        protected int hashColor;
        public int HashColor
        {
            get { return hashColor; }            
        }

        private Node node;
        public Node Node
        {
            get { return node; }
        }

        public NodeGlShape(Node n)
        {
            hashColor = byte.MaxValue - Counter;
            counter++;
            node = n;
        }

        public abstract void Draw();
    }

    public class LegGlShape: NodeGlShape
    {
        public float size = 10;

        private Leg leg;
        public Leg Leg
        {
            get { return leg; }
        }

        public LegGlShape(Leg leg):base(leg)
        {
            this.leg = leg;
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
            GL.Color3(System.Drawing.Color.FromArgb(hashColor, 0, 0));
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

    public class SliderGlShape: NodeGlShape
    {
        public float size = 10;

        private Slider slider;
        public Slider Slider
        {
            get { return slider; }
        }

        public SliderGlShape(Slider slider): base(slider)
        {
            this.slider = slider;
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
            GL.Scale(size, 2 * size, 0);

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
            GL.Scale(size, 2 * size, 0);

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
            GL.Color3(System.Drawing.Color.FromArgb(hashColor, 0, 0));
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
