using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections;

namespace Wpf
{
   public  class LegShape : Panel
    {
        public Leg leg;

        public Ellipse point = new Ellipse();
        public Line line = new Line();

        public Point From;
        public Point To;

        private Point pos;
        public Point Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
                leg.Position = value;
            }
        }

        public LegShape(Leg leg)
        {
            this.leg = leg;

            point.Height = point.Width = 20;
            point.Fill = Brushes.Black;
            point.Margin = new Thickness(30, 30, 0, 0);

            line.StrokeThickness = 5;
            line.Stroke = Brushes.Black;

            Children.Add(point);
            Children.Add(line);

            //el.MouseLeftButtonDown += (o, e) => { if (MouseLeftButtonDown != null) MouseLeftButtonDown(o, e); };
            //el.MouseLeftButtonUp += (o, e) => { if (mouseDown != null) mouseUp(o, e); };

            Background = Brushes.Chocolate;

            Width = Height = 100;
            //Margin = new Thickness(200, 200, 0, 0);
        }

        public void Update()
        {
            // point.Margin = new Thickness(To.X - point.Width / 2, To.Y - point.Width / 2, 0, 0);

            

            Margin = new Thickness(To.X - point.Width / 2, To.Y - point.Width / 2, 0, 0);

            line.X1 = From.X;
            line.Y1 = From.Y;
            line.X2 = To.X;
            line.Y2 = To.Y;

        }


    }
}
