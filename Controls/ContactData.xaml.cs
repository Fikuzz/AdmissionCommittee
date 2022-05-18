using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для ContactData.xaml
    /// </summary>
    public partial class ContactData : UserControl, IDataForm
    {
        private readonly string connectionString;
        
        public ContactData(Visibility ButtonClose, int Num)
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            tbHeader.Text = "КОНТАКТНЫЕ ДАННЫЕ " + (Num/10 < 1 ? "0":"") + Num;

            btClose.Visibility = ButtonClose;
            try
            {
                string sql = "SELECT Наименование FROM ТипКонтакта";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                    cbContactType.Items.Add(reader[0]);
                cbContactType.SelectedIndex = 0;
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_CloseNote(object sender, RoutedEventArgs e)
        {
            Panel panel = this.Parent as Panel;
            panel.Children.Remove(this);
            panel.Tag = (int)panel.Tag - 1;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Xceed.Wpf.Toolkit.MaskedTextBox textBox = ((StackPanel)((ComboBox)sender).Parent).Children[5] as Xceed.Wpf.Toolkit.MaskedTextBox;
            switch (((ComboBox)sender).SelectedIndex)
            {
                case 0:
                    textBox.Mask = "+0## 00 000-00-00";
                    textBox.Text = "+375";
                    break;
                default:
                    textBox.Mask = "";
                    textBox.Text = "";
                    break;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PLib.ClearError(sender);
        }

        private void SetStartPosition(object sender, RoutedEventArgs e)
        {
            PLib.SetStartPosition(sender);
        }

        public bool Validate()
        {
            bool result = true;
            PLib.CorrectData(mtbData, ref result);
            return result;
        }
    }
}
