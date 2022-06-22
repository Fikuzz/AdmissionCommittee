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
    /// <summary>
    /// Статический класс с общими методами
    /// </summary>
    abstract class PLib
    {
        /// <summary>
        /// Очистка тега Error
        /// </summary>
        /// <param name="sender">элемент</param>
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
        /// <summary>
        /// Установить карретку в начало маски
        /// </summary>
        /// <param name="sender">MaskedTextBox</param>
        public static void SetStartPosition(object sender)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "") return;
            char[] arr = textBox.Text.ToCharArray();
            if (arr[0] == '_')
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
        /// <summary>
        /// Проверка на ввод только числовых значений
        /// </summary>
        /// <param name="text">значение</param>
        /// <returns></returns>
        public static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        /// <summary>
        /// Проверка что элемент не пустой и заполненность маски ввода
        /// </summary>
        /// <param name="value">текстовый элемент</param>
        /// <param name="result">значение для результата проверки</param>
        public static void CorrectData(object value, ref bool result)
        {
            if (value is Xceed.Wpf.Toolkit.MaskedTextBox textBox)
            {
                if (textBox.IsMaskCompleted == false || (string)textBox.Tag == "Error")
                {
                    result = false;
                    textBox.Tag = "Error";
                }
            }
            else if (value is TextBox tb)
            {
                if (tb.Text == "" || (string)tb.Tag == "Error")
                {
                    result = false;
                    tb.Tag = "Error";
                }
            }
        }
        /// <summary>
        /// Проверка корректности форм реализующих интерфейс IDataForm
        /// </summary>
        /// <typeparam name="T">Название формы</typeparam>
        /// <param name="panel">контейнер в котором находяться формамы</param>
        /// <returns></returns>
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
        /// <summary>
        /// очистка текстовых полей чекбоксов и тд
        /// </summary>
        /// <typeparam name="T">Тип элемента</typeparam>
        /// <param name="obj">Элемент</param>
    }
}
