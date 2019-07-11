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
    public class LegGl
    {


        static int counter = 0;
        static int redCounter = 255;
        int name;
        public int red;

        private Leg leg;
        public Leg Leg
        {
            get { return leg; }
        }

        public LegGl(Leg leg)
        {
            this.leg = leg;
            name = counter++;
            red = redCounter--;
        }

        public void Draw()
        {
            GL.PushName(counter);

            GL.Begin(PrimitiveType.Polygon );
            GL.Color3(System.Drawing.Color.FromArgb(red, 0, 0));

            double step = Math.PI / 20;
            double angle = 0.0;
            while (angle < Math.PI * 2)
            {               
                double x = 10 * Math.Sin(angle) ;
                double y = 10 * Math.Cos(angle);

                System.Windows.Point p = new System.Windows.Point(x + leg.Position.X, y + leg.Position.Y);
                //GL.Color3(200, 0, 0);
                GL.Vertex2(GlConverter.ToGlCoord(p));
                angle += step;
            }

            GL.End();
        }
    }

}
