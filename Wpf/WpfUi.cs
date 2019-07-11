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
    public abstract class NodeShape
    {
        public event MouseButtonEventHandler MouseLeftButtonDown;
        public event MouseButtonEventHandler MouseLeftButtonUp;

        protected void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseLeftButtonDown != null)
                MouseLeftButtonDown(sender, e);
        }

        protected void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MouseLeftButtonUp != null)
                MouseLeftButtonUp(sender, e);
        }

        public abstract void Update();
    }

    public class LegShape: NodeShape
    {
        private Leg leg;
        public Leg Leg
        {
            get { return leg; }
        }

        public Ellipse point = new Ellipse();
        public Line line = new Line();
        public List<UIElement> uiList = new List<UIElement>();

        public LegShape(Leg leg)
        {
            this.leg = leg;

            point.Height = point.Width = 20;
            point.Fill = Brushes.Black;
            point.Margin = new Thickness(30, 30, 0, 0);

            line.StrokeThickness = 5;
            line.Stroke = Brushes.Black;

            uiList.Add(point);
            uiList.Add(line);

            point.MouseLeftButtonDown += (o, e) => { OnMouseLeftButtonDown(this, e); };

            leg.PosUpdate += Update;
            Update();
        }

        public override void Update()
        {
            var From = Leg.Prev.Position;
            var To = Leg.Position;

            point.Margin = new Thickness(To.X - point.Width / 2, To.Y - point.Width / 2, 0, 0);

            line.X1 = From.X;
            line.Y1 = From.Y;
            line.X2 = To.X;
            line.Y2 = To.Y;
        }
    }

    public class SliderShape : NodeShape
    {
        private Slider slider;
        public Slider Slider
        {
            get { return slider; }
        }

        public Line line = new Line();
        public Ellipse point = new Ellipse();

        public SliderShape(Slider slider)
        {
            point.Height = point.Width = 20;
            point.Fill = Brushes.Red;
            point.Margin = new Thickness(30, 30, 0, 0);
            point.MouseLeftButtonDown += (o, e) => { OnMouseLeftButtonDown(this, e); };

            line.StrokeThickness = 5;
            line.Stroke = Brushes.Red;

            this.slider = slider;
            slider.PosUpdate += Update;
            Update();
        }

        public override void Update()
        {
            line.X1 = slider.center.X + slider.min;
            line.X2 = slider.center.X + slider.max;
            line.Y1 = line.Y2 = slider.center.Y;

            point.Margin = new Thickness(slider.Position.X - point.Width / 2, slider.Position.Y - point.Width / 2, 0, 0);
        }

    }
}
