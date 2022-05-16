using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PriyemnayaKomissiya
{
    class TabItemThemeProperties
    {
        public static string GetDescription(DependencyObject obj)
        {
            return (string)obj.GetValue(dp: DescriptionProperty);
        }

        public static void SetDescription(DependencyObject obj, string value)
        {
            obj.SetValue(dp: DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.RegisterAttached(
                "Description",
                typeof(string),
                typeof(TabItemThemeProperties),
                new FrameworkPropertyMetadata(""));

        public static string GetNumber(DependencyObject obj)
        {
            return (string)obj.GetValue(NumberProperty);
        }

        public static void SetNumber(DependencyObject obj, string value)
        {
            obj.SetValue(NumberProperty, value);
        }

        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.RegisterAttached(
                "Number",
                typeof(string),
                typeof(TabItemThemeProperties),
                new FrameworkPropertyMetadata(""));
    }
}
