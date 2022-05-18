using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace PriyemnayaKomissiya.Controls
{
    /// <summary>
    /// Логика взаимодействия для СT.xaml
    /// </summary>
    public partial class CtCertificate : UserControl, IDataForm
    {
        private readonly string connectionString;
        public CtCertificate(int Num)
        {
            InitializeComponent();

            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            PLib.IsTextAllowed(e.Text);
        }

        private void SetStartPosition(object sender, TextCompositionEventArgs e)
        {
            PLib.SetStartPosition(sender);
        }

        private void ClearError(object sender, TextChangedEventArgs e)
        {
            PLib.ClearError(sender);
        }

        private void Button_CloseNote(object sender, RoutedEventArgs e)
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
