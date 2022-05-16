using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PriyemnayaKomissiya
{
    class ButtonAdmissionPlanThemeProperties
    {
        //Финансирование
        public static string GetFundingType(DependencyObject obj)
        {
            return (string)obj.GetValue(dp: FundingTypeProperty);
        }

        public static void SetFundingType(DependencyObject obj, string value)
        {
            obj.SetValue(dp: FundingTypeProperty, value);
        }

        public static readonly DependencyProperty FundingTypeProperty =
            DependencyProperty.RegisterAttached(
                "FundingType",
                typeof(string),
                typeof(ButtonAdmissionPlanThemeProperties),
                new FrameworkPropertyMetadata("null"));
        //Вид обучения
        public static string GetStudyType(DependencyObject obj)
        {
            return (string)obj.GetValue(dp: StudyTypeProperty);
        }

        public static void SetStudyType(DependencyObject obj, string value)
        {
            obj.SetValue(dp: StudyTypeProperty, value);
        }

        public static readonly DependencyProperty StudyTypeProperty =
            DependencyProperty.RegisterAttached(
                "StudyType",
                typeof(string),
                typeof(ButtonAdmissionPlanThemeProperties),
                new FrameworkPropertyMetadata("null"));
        //Количество записей
        public static string GetWritesCount(DependencyObject obj)
        {
            return obj.GetValue(dp: WritesCountProperty).ToString();
        }

        public static void SetWritesCount(DependencyObject obj, string value)
        {
            obj.SetValue(dp: WritesCountProperty, value);
        }

        public static readonly DependencyProperty WritesCountProperty =
            DependencyProperty.RegisterAttached(
                "WritesCount",
                typeof(string),
                typeof(ButtonAdmissionPlanThemeProperties),
                new FrameworkPropertyMetadata("0"));
        //Цвет фона
        public static Brush GetTickBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(TickBrushProperty);
        }

        public static void SetTickBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(TickBrushProperty, value);
        }

        public static readonly DependencyProperty TickBrushProperty =
            DependencyProperty.RegisterAttached(
                "TickBrush",
                typeof(Brush),
                typeof(ButtonAdmissionPlanThemeProperties),
                new FrameworkPropertyMetadata(Brushes.Black));
    }
}
