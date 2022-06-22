using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace PriyemnayaKomissiya.Controls
{
    /// <summary>
    /// Логика взаимодействия для формы добавления и редактирования сертификата
    /// </summary>
    public partial class CtCertificate : UserControl, IDataForm
    {
        private readonly string connectionString;
        /// <summary>
        /// Конструктор формы контактных данных
        /// </summary>
        /// <param name="Num">Номер формы</param>
        public CtCertificate(int Num)
        {
            InitializeComponent();
            tbTitle.Text = "СЕРТИФИКАТ ЦТ " + Num;
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = 315,
                Duration = TimeSpan.FromSeconds(0.2)
            };
            this.BeginAnimation(UserControl.HeightProperty, animation);
        }
        /// <summary>
        /// Обработчик для ввода только числовых значений
        /// </summary>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PLib.IsTextAllowed(e.Text);
        }
        /// <summary>
        /// Устанавливеет курсо в начало маски
        /// </summary>
        private void SetStartPosition(object sender, TextCompositionEventArgs e)
        {
            PLib.SetStartPosition(sender);
        }
        /// <summary>
        /// Очистка тега Error при изменении текста
        /// </summary>
        private void ClearError(object sender, TextChangedEventArgs e)
        {
            PLib.ClearError(sender);
        }
        /// <summary>
        /// Закрытие формы
        /// </summary>
        private void Button_CloseNote(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = this.Height,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2)
            };
            animation.Completed += (_, __) => CloseForm();
            this.BeginAnimation(UserControl.HeightProperty, animation);
        }
        void CloseForm()
        {
            Panel panel = this.Parent as Panel;
            panel.Children.Remove(this);
            panel.Tag = (int)panel.Tag - 1;
        }

        public bool Validate()
        {
            bool corect = true;
            PLib.CorrectData(mtbYear, ref corect);
            PLib.CorrectData(tbScore, ref corect);
            PLib.CorrectData(tbSeries, ref corect);
            return corect;
        }
        /// <summary>
        /// Проверка корректности вода балла ЦТ
        /// </summary>
        private void ScoreTextInput(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!int.TryParse(textBox.Text, out int value))
            {
                textBox.Tag = "Error";
            }
            else if (value < 0 || value > 100)
            {
                textBox.Tag = "Error";
            }
            else
            {
                textBox.Tag = "";
            }
        }
    }
}
