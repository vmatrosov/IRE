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

using OpenTK;

namespace Wpf
{
    /// <summary>
    /// Логика взаимодействия для VectorEditor.xaml
    /// </summary>
    public partial class VectorEditor : UserControl
    {
        public static readonly DependencyProperty Vector2Property;
        public static readonly DependencyProperty Vector3Property;

        public Vector2d Vector2
        {
            get { return (Vector2d)GetValue(Vector2Property); }
            set { SetValue(Vector2Property, value); }
        }

        public Vector3d Vector3
        {
            get { return (Vector3d)GetValue(Vector3Property); }
            set { SetValue(Vector3Property, value); }
        }

        public VectorEditor()
        {
            InitializeComponent();
        }

        static VectorEditor()
        {
            FrameworkPropertyMetadata vector2metadata = new FrameworkPropertyMetadata(new Vector2d(), FrameworkPropertyMetadataOptions.AffectsMeasure, Vector2Changed);
            Vector2Property = DependencyProperty.Register("Vector2", typeof(Vector2d), typeof(VectorEditor), vector2metadata, ValidateCurrentNumber);

            FrameworkPropertyMetadata vector3metadata = new FrameworkPropertyMetadata(new Vector3d(), FrameworkPropertyMetadataOptions.AffectsMeasure, Vector2Changed);
            Vector3Property = DependencyProperty.Register("Vector3", typeof(Vector3d), typeof(VectorEditor), vector3metadata, ValidateCurrentNumber);
        }

        public static bool ValidateCurrentNumber(object value)
        {
            return true;
        }

        private static void Vector2Changed(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            VectorEditor v = (VectorEditor)depObj;

            v.tbX.Text = ((Vector2d)args.NewValue).X.ToString("F2");
            v.tbY.Text = ((Vector2d)args.NewValue).Y.ToString("F2");
            v.tbZ.Text = "NA";
        }

        private static void Vector3Changed(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            VectorEditor v = (VectorEditor)depObj;

            v.tbX.Text = ((Vector3d)args.NewValue).X.ToString("F2");
            v.tbY.Text = ((Vector3d)args.NewValue).Y.ToString("F2");
            v.tbZ.Text = ((Vector3d)args.NewValue).Z.ToString("F2");
        }
    }
}
