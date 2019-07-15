using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Drawing;

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

        protected TextRenderer textRenderer;


        public NodeGlShape(Node n)
        {
            hashColor = byte.MaxValue - Counter;
            counter++;
            node = n;

            //Text render
            textRenderer = new TextRenderer(40, 40);
            textRenderer.Clear(Color.FromArgb(0, Color.White));
            textRenderer.DrawString(n.Caption, new Font(FontFamily.GenericMonospace, 24, FontStyle.Bold), Brushes.Black, PointF.Empty);
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
            GL.Rotate(((float)leg.Pitch), Vector3.UnitZ);
            GL.Scale((leg.Length - size) / 2, size/4, 1);

            GL.Begin(PrimitiveType.Polygon);
            GL.Color3(Color.Green);
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
                GL.Vertex3(size * Math.Sin(angle), size * Math.Cos(angle), -1);
                angle += step;
            }
            GL.End();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(v.X, v.Y - 30, 0);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textRenderer.Texture);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-10f, -10f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(10f, -10f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(10f, 10f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-10f, 10f);
            GL.End();
            GL.Disable(EnableCap.Texture2D);
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

        public SliderGlShape(Slider slider) : base(slider)
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
            GL.Color3(Color.BlueViolet);
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
                GL.Vertex3(size * Math.Sin(angle), size * Math.Cos(angle), -1);
                angle += step;
            }
            GL.End();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(v.X, v.Y - 30, 0);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textRenderer.Texture);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-10f, -10f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(10f, -10f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(10f, 10f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-10f, 10f);
            GL.End();
            GL.Disable(EnableCap.Texture2D);
        }
    }
}
