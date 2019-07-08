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

namespace Wpf
{

    public class Node : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected Point position;
        public Point Position
        {
            get { return position; }
            set
            {
                position = value;
                NotifyPropertyChanged("Position");
            }
        }
    }

    public class StaticHinge : Node
    {
        public StaticHinge()
        {
            el.Height = 10;
            el.Width = 10;
            el.Fill = Brushes.Black;
        }

        public Leg next;

        public Ellipse el = new Ellipse();


        public void UpdateShape()
        {
            el.Margin = new Thickness(position.X - el.Width / 2, position.Y - el.Width / 2, 0, 0);
        }
    }

    public class Leg : Node
    {
        public Leg(Node parent)
        {
            prev = parent;
            if (parent is Leg)
                (parent as Leg).next = this;

            Shape = new LegShape(this);
        }

        public LegShape Shape;

        public Node prev;
        public Node next;

        private double length = 1.0;
        public double Length
        {
            get { return length; }
            set {
                if (value > 0)
                {
                    length = value;
                    NotifyPropertyChanged("Length ");
                }
            }
        }

        public double angle;
        public double Angle
        {
            get
            {
                return angle * 180.0 / Math.PI;
            }
            set
            {
                if(value >=0 && value < 360.0)
                {
                    angle = value * Math.PI / 360.0;
                    NotifyPropertyChanged("Angle");
                }
            }
        }

        public void UpdateRelativePos()
        {
            var dist = Math.Sqrt(Math.Pow(position.Y - prev.Position.Y, 2) + Math.Pow(position.X - prev.Position.X, 2));
            angle = Math.Acos(position.Y - prev.Position.Y) / dist;
            if (position.X < prev.Position.X)
                angle += Math.PI;

            UpdatePos();

            if (next != null)
            {
                if (next is Leg)
                {
                    (next as Leg).UpdateRelativePos();
                }
            }
        }

        public void  UpdatePos()
        {
            position.X = prev.Position.X + Math.Sin(angle) * length;
            position.Y = prev.Position.Y + Math.Cos(angle) * length;

            Shape.From = prev.Position;
            Shape.To = this.position;
            Shape.Update();

        }

    }

}
