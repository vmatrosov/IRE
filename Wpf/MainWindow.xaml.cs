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
        public MainWindow()
        {
            InitializeComponent();

            StaticHinge h1 = new StaticHinge();
            h1.Position = new Point(100, 100);
            h1.UpdateShape();

            Leg h2 = new Leg(h1);
            h2.Angle = 90;
            h2.Length = 100;
            h2.UpdatePos();

            Leg h3 = new Leg(h2);
            h3.Angle = 135;
            h3.Length = 200;
            h3.UpdatePos();

            canvas.Children.Add(h1.el);
            canvas.Children.Add(h2.Shape);
            canvas.Children.Add(h3.Shape);

            h2.Shape.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
            h3.Shape.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;

            h2.Shape.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
            h3.Shape.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
        }

        FrameworkElement captureEl = null;

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(sender as Canvas);
            tbX.Text = pos.X.ToString();
            tbY.Text = pos.Y.ToString();

            if (captureEl != null)
            {
                if (captureEl is Ellipse)
                    (captureEl as LegShape).Pos = pos;
            }
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            captureEl = sender as LegShape;
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            captureEl = null;
        }
    }
}
