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
    static class Helper
    {
        public const double ToDeg = 180.0 / Math.PI;
        public const double ToRad = Math.PI / 180.0;
    }

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

        public virtual Point Position
        {
            get { return position; }
            set { position = value; }
        }

       public double X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public double Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        private Node prev;
        public Node Prev
        {


            get
            {
                return prev;
            }
            set
            {
                prev = value;

                if (prev != null)
                    prev.next = this;
            }
        }

        private Node next;
        public Node Next
        {
            get
            {
                return next;
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
        public override Point Position
        {
            set
            {
                position = value;
                UpdateRelativePos();
                NotifyPropertyChanged("Position");
            }
        }

        private double length = 1.0;
        public double Length
        {
            get { return length; }
            set {
                if (value > 0)
                {
                    length = value;
                    NotifyPropertyChanged("Length");
                }
            }
        }

        public double angle;
        public double Angle
        {
            get
            {
                return angle * Helper.ToDeg;
            }
            set
            {
                if(value >=0 && value < 360.0)
                {
                    angle = value * Helper.ToRad;
                    NotifyPropertyChanged("Angle");

                }
            }
        }

        public void UpdateRelativePos()
        {
            var dist = Math.Sqrt(Math.Pow(position.Y - Prev.Position.Y, 2) + Math.Pow(position.X - Prev.Position.X, 2));

            angle = Math.Acos((position.Y - Prev.Position.Y) / dist);

            if (position.X < Prev.Position.X)
                angle += Math.PI;

            NotifyPropertyChanged("Angle");

            UpdatePos();

            if (Next != null)
            {
                if (Next is Leg)
                {
                    (Next as Leg).UpdateRelativePos();
                }
            }
        }

        public void  UpdatePos()
        {

            position.X = Prev.Position.X + Math.Sin(angle) * length;
            position.Y = Prev.Position.Y + Math.Cos(angle) * length;

            OnPositionUpdate();
        }

        public Action PosUpdate;

        protected void OnPositionUpdate()
        {
            if (PosUpdate != null)
                PosUpdate();
        }


    }

}
