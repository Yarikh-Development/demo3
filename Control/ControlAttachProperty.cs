using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace demo.Control
{
    class ControlAttachProperty:DependencyObject
    {


        public static Selector GetSelectedIndex(DependencyObject obj)
        {
            return (Selector)obj.GetValue(SelectedIndexProperty);
        }

        public static void SetSelectedIndex(DependencyObject obj, Selector value)
        {
            obj.SetValue(SelectedIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.RegisterAttached("SelectedIndex", typeof(Selector), typeof(ControlAttachProperty), new PropertyMetadata(0));


    }
}
