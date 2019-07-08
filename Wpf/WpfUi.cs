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


    public class LegShape
    {
        private Leg leg;
        public Leg Leg
        {
            get { return leg; }
        }

        public Ellipse point = new Ellipse();
        public Line line = new Line();
        public List<UIElement> uiList = new List<UIElement>();

        public LegShape()
        {
            leg = new Leg();

            point.Height = point.Width = 20;
            point.Fill = Brushes.Black;
            point.Margin = new Thickness(30, 30, 0, 0);

            line.StrokeThickness = 5;
            line.Stroke = Brushes.Black;

            uiList.Add(point);
            uiList.Add(line);

            point.MouseLeftButtonDown += (o, e) => { if (MouseLeftButtonDown != null) MouseLeftButtonDown(this, e); };
            point.MouseLeftButtonUp += (o, e) => { if (MouseLeftButtonUp != null) MouseLeftButtonUp(this, e); };

            leg.PosUpdate += Update;
        }

        public event MouseButtonEventHandler MouseLeftButtonDown;
        public event MouseButtonEventHandler MouseLeftButtonUp;

        public void Update()
        {
           var  From = Leg.Prev.Position;
            var To = Leg.Position;

            point.Margin = new Thickness(To.X - point.Width / 2, To.Y - point.Width / 2, 0, 0);

            line.X1 = From.X;
            line.Y1 = From.Y;
            line.X2 = To.X;
            line.Y2 = To.Y;
        }

    }
}
