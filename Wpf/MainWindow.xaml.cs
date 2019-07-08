using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LegShape captureEl = null;

        public MainWindow()
        {
            InitializeComponent();

            StaticHinge h1 = new StaticHinge();
            h1.Position = new Point(100, 100);
            h1.UpdateShape();

            LegShape leg1 = new LegShape();
            leg1.Leg.Angle = 90;
            leg1.Leg.Length = 100;            
            leg1.Leg.Prev = h1;
            leg1.Leg.UpdatePos();

            LegShape leg2 = new LegShape();
            leg2.Leg.Angle = 135;
            leg2.Leg.Length = 200;            
            leg2.Leg.Prev = leg1.Leg;
            leg2.Leg.UpdatePos();

            canvas.Children.Add(leg1.line);
            canvas.Children.Add(leg2.line);

            canvas.Children.Add(h1.el);
            canvas.Children.Add(leg1.point);
            canvas.Children.Add(leg2.point);

            leg1.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
            leg2.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

            leg1.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
            leg2.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;

            canvas.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(sender as Canvas);
            tbX.Text = pos.X.ToString();
            tbY.Text = pos.Y.ToString();

            if (captureEl != null)
            {
                if (captureEl is LegShape)
                {
                    (captureEl as LegShape).Leg.Position = pos;
                    (captureEl as LegShape).Leg.UpdateRelativePos();
                }
            }
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            captureEl = sender as LegShape;

            gbLegData.DataContext = (sender as LegShape).Leg;
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            captureEl = null;
        }


    }
}
