﻿using System;
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
	/// Логика взаимодействия для Certificate.xaml
	/// </summary>
	public partial class Certificate : UserControl
	{
		public List<TextBox> Marks = new List<TextBox>();
		private readonly string connectionString;
		public Certificate(Visibility ButtonClose, int Num)
		{
			InitializeComponent();

			btClose.Visibility = ButtonClose;
			tblHeader.Text = "ОБРАЗОВАНИЕ АБИТУРИЕНТА " + (Num / 10 < 1 ? "0" : "") + Num;
			connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
		}

		private void Button_CloseNote(object sender, RoutedEventArgs e)
		{
			Panel panel = this.Parent as Panel;
			panel.Children.Remove(this);
			panel.Tag = (int)panel.Tag - 1;
		}

		public bool Validate()
		{
			if (tbSeries.Text == "")
			{
				tbSeries.Tag = "Error";
				return false;
			}
			else
			{
				tbSeries.Tag = "";
				return true;
			}
		}

		private void ClearError(object sender, TextChangedEventArgs e)
		{
			PLib.ClearError(sender);
		}

		private void TextBox_GetMarksSum(object sender, TextChangedEventArgs e)
		{
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
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !PLib.IsTextAllowed(e.Text);
		}

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
				}
				else
				{
					Marks[i].IsEnabled = true;
				}
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
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
		}
	}
}