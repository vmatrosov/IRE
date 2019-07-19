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
    public class Node : INotifyPropertyChanged
    {
        public const double ToDeg = 180.0 / Math.PI;
        public const double ToRad = Math.PI / 180.0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event Action PosUpdate;
        protected void OnPositionUpdate()
        {
            if (PosUpdate != null)
                PosUpdate();
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

        protected double pitch = 0.0;
        public virtual double Pitch
        {
            get
            {
                return pitch * ToDeg;
            }
            set
            {
                value = value % 360.0;
                pitch = value * ToRad;
                NotifyPropertyChanged("Pitch");
            }
        }

        protected string caption;
        public string Caption
        {
            get { return caption; }
            set { caption = value; NotifyPropertyChanged("Caption"); }
        }

        protected bool allowFullRotate = true;
        public bool AllowFullRotate
        {
            get { return allowFullRotate; }
            set { allowFullRotate = value; NotifyPropertyChanged("AllowFullRotate"); }
        }

        public virtual void UpdateRelativePos(bool rigid)
        {

        }

        public virtual void SetPosition(Vector2d newPos)
        {

        }

        public Node Prev
        {
            get; set;
        }

        public Node Next
        {
            get; set;
        }

        protected bool rigid;
        public bool Rigid
        {
            get
            {
                return rigid;
            }

            set
            {
                rigid = value;
                NotifyPropertyChanged("Rigid");
            }
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

            Length = length_;
            LinkedAngle = angle_;
        }

        private double length = 1.0;
        public double Length
        {
            get
            {
                return length;
            }
            set
            {
                if (value > 0)
                {
                    length = value;
                    NotifyPropertyChanged("Length");

                    UpdatePos();
                }
            }
        }

        public override double Pitch
        {
            get
            {
                return base.Pitch;
            }
            set
            {
                var newLinkedAngle = (value - Prev.Pitch) * ToRad;

                if (Math.Abs(newLinkedAngle * ToDeg) < 160.0 || Prev.AllowFullRotate)
                {
                    base.Pitch = value;
                    linkedAngle = newLinkedAngle;
                }

                UpdatePos();
                NotifyPropertyChanged("LinkedAngle");
            }
        }

        double linkedAngle = 0.0;
        public double LinkedAngle
        {
            get
            {
                return linkedAngle * ToDeg;
            }
            set
            {
                var newLinkedAngle = value * ToRad;

                if (Math.Abs(newLinkedAngle * ToDeg) < 160.0 || Prev.AllowFullRotate)
                {
                    linkedAngle = newLinkedAngle;
                    pitch = (Prev.Pitch + value) * ToRad;

                }

                UpdatePos();
                NotifyPropertyChanged("LinkedAngle");
                NotifyPropertyChanged("Pitch");

            }
        }

        public override void SetPosition(Vector2d newPos)
        {
            position = newPos;
            UpdateRelativePos(false);
            NotifyPropertyChanged("Position");
        }

        private void UpdatePos()
        {
            var X = (Prev.Position.X + Math.Cos(pitch) * length);
            var Y = (Prev.Position.Y + Math.Sin(pitch) * length);

            Position = new Vector2d(X, Y);

            OnPositionUpdate();
        }

        public override void UpdateRelativePos(bool rigid_)
        {
            if (rigid_)
            {
                Pitch = Prev.Pitch + LinkedAngle;
            }
            else
            {
                var dxy = Position - Prev.Position;
                Pitch = Math.Acos(dxy.X / dxy.Length) * Math.Sign(dxy.Y) * ToDeg;
            }

            if (Next != null)
                Next.UpdateRelativePos(rigid);
        }
    }

    public class Slider : Node
    {
        public double width = 100;
        public double heigth = 10;

        public Vector2d center;

        public override void SetPosition(Vector2d newPos)
        {
            var Y = center.Y;
            var X = newPos.X;

            var dX = position.X - center.X;

            if (dX > width)
                X = center.X + width;

            if (dX < -width)
                X = center.X - width;

            Position = new Vector2d(X, Y);

            OnPositionUpdate();
        }

        public override void UpdateRelativePos(bool rigid_)
        {
            if (Next != null)
                Next.UpdateRelativePos(rigid);
        }
    }
}
