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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PriyemnayaKomissiya.Controls
{
	/// <summary>
	/// Логика взаимодействия для формы редактирования сертификата
	/// </summary>
	public partial class Certificate : UserControl, IDataForm
	{
		/// <summary>
		/// список полей для отметок
		/// </summary>
		public List<TextBox> Marks = new List<TextBox>();
		private readonly string connectionString;
		/// <summary>
		/// конструктор для формы аттетата
		/// </summary>
		/// <param name="ButtonClose">Определяет будел ли видна кнопка закрытия формы</param>
		/// <param name="Num">Номер аттестата</param>
		public Certificate(Visibility ButtonClose, int Num)
		{
			InitializeComponent();
			connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
			try
			{
				string sql = "SELECT Наименование, КоличествоБаллов FROM Шкала";
				SqlConnection connection = new SqlConnection(connectionString);
				SqlCommand command = new SqlCommand(sql, connection);
				connection.Open();
				SqlDataReader reader = command.ExecuteReader();
				while (reader.Read())
				{
					ComboBoxItem item = new ComboBoxItem
					{
						Content = reader.GetString(reader.GetOrdinal("Наименование")),
						Tag = reader.GetInt32(reader.GetOrdinal("КоличествоБаллов"))
					};
					cbScaleType.Items.Add(item);
				}
				cbScaleType.SelectedIndex = 0;
				connection.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			for (int i = 0; i < grMarks.Children.Count; i++)
			{
				if (grMarks.Children[i] is TextBox textBox)
				{
					Marks.Add(textBox);
				}
			}

			int MaxMark = (int)((ComboBoxItem)cbScaleType.SelectedItem).Tag;

			for (int i = 0; i < Marks.Count; i++)
			{
				if (i >= MaxMark)
				{
					Marks[i].IsEnabled = false;
					Marks[i].Text = string.Empty;
				}
				else
				{
					Marks[i].IsEnabled = true;
				}
			}

			btClose.Visibility = ButtonClose;
			tblHeader.Text = "ОБРАЗОВАНИЕ АБИТУРИЕНТА " + (Num / 10 < 1 ? "0" : "") + Num;

			DoubleAnimation animation = new DoubleAnimation
			{
				From = 0,
				To = 670,
				Duration = TimeSpan.FromSeconds(0.2)
			};
			this.BeginAnimation(UserControl.HeightProperty, animation);
		}
		/// <summary>
		/// Обработчик кнопки закрытия формы
		/// </summary>>
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
			bool result = true;
			PLib.CorrectData(tbSeries, ref result);

            int count = (int)((ComboBoxItem)cbScaleType.SelectedItem).Tag;
            for (int j = 0; j < Marks.Count; j++)
            {
                if ((Marks[j].Text == "" && count > j) ||
                (Marks[j].Text != "" && count <= j))
                {
					result = false;
                    Marks[j].Tag = "Error";
                }
            }

            return result;
		}
		/// <summary>
		/// Обработчик для снятия тега Error с поля
		/// </summary>
		private void ClearError(object sender, TextChangedEventArgs e)
		{
			PLib.ClearError(sender);
		}
		/// <summary>
		/// Подсчет суммы отметок аттестата 
		/// </summary>
		private void TextBox_GetMarksSum(object sender, TextChangedEventArgs e)
        {
			if (Marks.Count == 0) return;
            TextBox textBox = (TextBox)sender;
            textBox.Tag = "";

            int MarksCount = 0;
            foreach (TextBox tb in Marks)
            {
                if (tb.IsEnabled == false)
                    break;
                if (int.TryParse(tb.Text, out int x))
                    MarksCount += x;
            }
            tblTotalMarks.Text = "Общее количество отметок: " + MarksCount;
        }
		/// <summary>
		/// Обработчик для полей которые могут принимать тоько числа
		/// </summary>
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !PLib.IsTextAllowed(e.Text);
		}
		/// <summary>
		/// Обработчик изменения шкалы для блокиования неиспользуемых полей отметок
		/// </summary>
		private void ScaleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBoxItem boxItem = (ComboBoxItem)e.AddedItems[0];
			int MaxMark = (int)boxItem.Tag;

			for (int i = 0; i < Marks.Count; i++)
			{
				if (i >= MaxMark)
				{
					Marks[i].IsEnabled = false;
					Marks[i].Text = string.Empty;
					Marks[i].Tag = "";
				}
				else
				{
					Marks[i].IsEnabled = true;
					Marks[i].Text = "0";
				}
			}
		}
		/// <summary>
		/// Обработчик для выделения всего текста при выборе поля отметки
		/// </summary>
		private void TextBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
			(sender as TextBox).SelectAll();
        }
		/// <summary>
		/// Обработчик запрещающий изменения элемента combobox колесиком мыши
		/// </summary>
		private void cbScaleType_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;
		}
	}
}
