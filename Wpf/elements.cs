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

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Wpf
{
    static class Helper
    {
        public const double ToDeg = 180.0 / Math.PI;
        public const double ToRad = Math.PI / 180.0;
    }

    public enum SliderType
    {
        Horizontal, Vertical
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

        protected Vector2d position;
        public Vector2d Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                NotifyPropertyChanged("Position");
            }
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

        public Node Prev
        {
            get; set;
        }

        public Node Next
        {
            get; set;
        }

        public event Action PosUpdate;

        protected void OnPositionUpdate()
        {
            if (PosUpdate != null)
                PosUpdate();
        }

    }

    public class Leg : Node
    {
        public Leg(Node prev)
        {
            this.Prev = prev;

            if (prev != null)
                Prev.Next = this;
        }

        public Leg(Node prev, double angle_, double length_)
        {
            Prev = prev;
            if (Prev != null)
                Prev.Next = this;

            Angle = angle_;
            Length = length_;

            UpdatePos();
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

        double angle = 0.0;
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

        public void SetPosition(Vector2d newPos)
        {
            position = newPos;
            UpdateRelativePos();
            NotifyPropertyChanged("Position");
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

        public void UpdatePos()
        {
            position.X = ( Prev.Position.X + Math.Sin(angle) * length);
            position.Y = (Prev.Position.Y + Math.Cos(angle) * length);

            OnPositionUpdate();
        }

    }
    
    public class Slider : Node
    {
        public double width = 100;
        public double heigth = 10;

        public Vector2d center;

        private SliderType type = SliderType.Horizontal;
        public SliderType Type
        {
            get { return type; }
            set { type = value; NotifyPropertyChanged("Type"); }
        }

        public void SetPosition(Vector2d newPos)
        {
            if(type == SliderType.Horizontal)
            {
                position.Y = center.Y;
                position.X = newPos.X;

                var dX = position.X - center.X;

                if (dX > width)
                    position.X = center.X + width;

                if (dX < -width)
                    position.X = center.X - width;
            }

            OnPositionUpdate();
        }

        public void UpdateRelativePos()
        {
            if (Next != null)
            {
                if (Next is Leg)
                {
                    (Next as Leg).UpdateRelativePos();
                }
            }
        }
    }
}
