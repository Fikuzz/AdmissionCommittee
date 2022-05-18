using PriyemnayaKomissiya.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PriyemnayaKomissiya
{
    abstract class PLib
    {
        public static void ClearError(object sender)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).Tag = "Error";
            }
            else
            {
                ((TextBox)sender).Tag = "";
            }
        }
        public static void SetStartPosition(object sender)
        {
            TextBox textBox = (TextBox)sender;
            char[] arr = textBox.Text.ToCharArray();
            if(arr[0] == '_')
            {
                textBox.SelectionStart = 0;
            }
            else if (textBox.SelectionStart == textBox.Text.Length)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == '_')
                    {
                        textBox.SelectionStart = i;
                        break;
                    }
                }
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");
        public static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public static void CorrectData(object value, ref bool result)
        {
            if(value is Xceed.Wpf.Toolkit.MaskedTextBox textBox){
                if(textBox.IsMaskCompleted == false || textBox.Tag == "Error")
                {
                    result = false;
                    textBox.Tag = "Error";
                }
            }
            else if(value is TextBox tb)
            {
                if(tb.Text == "" || tb.Tag == "Error")
                {
                    result = false;
                    tb.Tag = "Error";
                }
            }
        }

        public static bool FormIsCorrect<T>(Panel panel) where T : IDataForm
        {
            bool correct = true;
            for (int i = 0; i < panel.Children.Count; i++)
            {
                if (panel.Children[i] is T ctCertificate)
                {
                    if (!ctCertificate.Validate())
                    {
                        correct = false;
                    }
                }
            }
            return correct;
        }
    }
}
