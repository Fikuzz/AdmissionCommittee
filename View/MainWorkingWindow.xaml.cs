using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PriyemnayaKomissiya.View
{
    public partial class MainWorkingWindow : Window
    {
        int PlanPriemaID = 0; //mda
        int currentPlanPriemaID = 0;
        private readonly int userId;
        private int planPriemaColumn = 0;
        private readonly List<Canvas> planPriemaButtons = new List<Canvas>();
        private readonly string connectionString;
        List<AbiturientDGItem> abiturients = null;
        public MainWorkingWindow(int idUser, string FIOUser)
        {
            InitializeComponent();
            userId = idUser;
            lUser_FIO.Text = FIOUser;
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            {
                planPriemaButtons.Add(BlockPlana1);
                planPriemaButtons.Add(BlockPlana2);
                planPriemaButtons.Add(BlockPlana3);
                planPriemaButtons.Add(BlockPlana4);
                planPriemaButtons.Add(BlockPlana5);
                planPriemaButtons.Add(BlockPlana6);
                planPriemaButtons.Add(BlockPlana7);
                planPriemaButtons.Add(BlockPlana8);
                planPriemaButtons.Add(BlockPlana9);
            }
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = System.IO.Path.GetDirectoryName(location);
            try {
                using (StreamReader sr = new StreamReader(path + "/config"))
                {
                    string Str;
                    while ((Str = sr.ReadLine()) != null)
                    {
                        string[] settingStr = Str.Split(' ');
                        switch (settingStr[0])
                        {
                            case "Filter:":
                                int j = 1;
                                for (int i = 0; i < Filter.Children.Count; i++)
                                {
                                    if (Filter.Children[i].GetType().ToString() == "System.Windows.Controls.CheckBox")
                                    {
                                        try
                                        {
                                            ((CheckBox)Filter.Children[i]).IsChecked = Convert.ToBoolean(settingStr[j]);
                                            j++;
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                break;
                            case "WindowState:":
                                this.WindowState = (WindowState)Enum.Parse(typeof(WindowState), settingStr[1]);
                                break;
                            case "WindowWidth:":
                                this.Width = Convert.ToDouble(settingStr[1]);
                                break;
                            case "WindowHeight:":
                                this.Height = Convert.ToDouble(settingStr[1]);
                                break;
                        }
                    }
                }
            }
            catch { }
        }
        public MainWorkingWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            {
                planPriemaButtons.Add(BlockPlana1);
                planPriemaButtons.Add(BlockPlana2);
                planPriemaButtons.Add(BlockPlana3);
                planPriemaButtons.Add(BlockPlana4);
                planPriemaButtons.Add(BlockPlana5);
                planPriemaButtons.Add(BlockPlana6);
                planPriemaButtons.Add(BlockPlana7);
                planPriemaButtons.Add(BlockPlana8);
                planPriemaButtons.Add(BlockPlana9);
            }
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = System.IO.Path.GetDirectoryName(location);

            using (StreamReader sr = new StreamReader(path + "/config"))
            {
                string Str;
                while ((Str = sr.ReadLine()) != null)
                {
                    string[] settingStr = Str.Split(' ');
                    switch (settingStr[0])
                    {
                        case "Filter:":
                            int j = 1;
                            for (int i = 0; i < Filter.Children.Count; i++)
                            {
                                if (Filter.Children[i].GetType().ToString() == "System.Windows.Controls.CheckBox")
                                {
                                    try
                                    {
                                        ((CheckBox)Filter.Children[i]).IsChecked = Convert.ToBoolean(settingStr[j]);
                                        j++;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            break;
                        case "WindowState:":
                            this.WindowState = (WindowState)Enum.Parse(typeof(WindowState), settingStr[1]);
                            break;
                        case "WindowWidth:":
                            this.Width = Convert.ToDouble(settingStr[1]);
                            break;
                        case "WindowHeight:":
                            this.Height = Convert.ToDouble(settingStr[1]);
                            break;
                    }
                }
            }
        }
        private void MainWorkingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            lbPlanPriema.Content = "ПЛАН ПРИЕМА " + DateTime.Now.Year;
            var date = new StringBuilder(DateTime.Now.ToString("dddd, d MMMM"));
            date[0] = char.ToUpper(date[0]);
            lDate.Content = date.ToString();

            //Заполнение специальностей
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_SpecialnostiName", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@useFilter", 0);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    TabItem tabItem = new TabItem
                    {
                        Style = (Style)FindResource("TabItemStyle"),
                        Header = reader[0]
                    };
                    tabItem.PreviewMouseDown += new MouseButtonEventHandler(TabItem_MouseDown);
                    TabControl.Items.Add(tabItem);
                }
                connection.Close();
                TabControl.SelectedItem = TabControl.Items[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            PlaniPriemaLoad(((TabItem)TabControl.SelectedItem).Header.ToString());
        }
        private void MainWorkingWindowForm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if(this.WindowState == WindowState.Maximized || this.Width > 1400) dataDridAbiturients.Columns[5].Visibility = Visibility.Visible;
            //else dataDridAbiturients.Columns[5].Visibility = Visibility.Hidden;

            //ScrollAddMain.Height = this.Height-300;
            //if (this.Width < 1400)
            //    ScrollAddMain.Width = this.Width-300;
            if (this.WindowState == WindowState.Maximized)
            {
                ButtonPos(4);
            }
            else if (this.Width < 1300)
            {
                ButtonPos(2);
            }
            else if (this.Width < 1600)
            {
                ButtonPos(3);
            }
            else
            {
                ButtonPos(4);
            }
        }
        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            addEditForm.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Visible;
            GridDataTable.Visibility = Visibility.Hidden;
            Filter.Visibility = Visibility.Visible;
            GridInfo.Visibility = Visibility.Hidden;
            PlaniPriemaLoad(((TabItem)sender).Header.ToString());
            TabControl.SelectedItem = sender as TabItem;
        } //нажатие на TabItem специальности
        private void MainWorkingWindowForm_Closed(object sender, EventArgs e) //Запись положения фильтров в файл
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var path = System.IO.Path.GetDirectoryName(location);

            using (StreamWriter sw = new StreamWriter(path + "/config"))
            {
                sw.Write("Filter: ");
                for (int i = 0; i < Filter.Children.Count; i++)
                {
                    if (Filter.Children[i].GetType().ToString() == "System.Windows.Controls.CheckBox")
                        sw.Write(((CheckBox)Filter.Children[i]).IsChecked + " ");
                }
                sw.WriteLine("\nWindowState: " + (int)this.WindowState);
                sw.WriteLine("WindowWidth: " + (double)this.Width);
                sw.WriteLine("WindowHeight: " + (double)this.Height);
            }
        }
        private void ClearData<T>(T obj) where T : Panel
        {
            foreach (object control in obj.Children)
            {
                if (control.GetType() == typeof(CheckBox))
                    ((CheckBox)control).IsChecked = false;
                if (control.GetType() == typeof(TextBox))
                {
                    ((TextBox)control).Text = String.Empty;
                    ((TextBox)control).Tag = String.Empty;
                }
                if (control.GetType() == typeof(Xceed.Wpf.Toolkit.MaskedTextBox))
                    ((Xceed.Wpf.Toolkit.MaskedTextBox)control).Text = String.Empty;
                if (control.GetType() == typeof(ComboBox))
                    ((ComboBox)control).SelectedIndex = 0;
                if (control.GetType() == typeof(StackPanel))
                {
                    if (((StackPanel)control).Tag != null && ((StackPanel)control).Tag.ToString() == "HIddenField")
                        ((StackPanel)control).Visibility = Visibility.Collapsed;
                    ClearData<StackPanel>((StackPanel)control);
                }
                if (control.GetType() == typeof(Grid))
                {
                    ClearData<Grid>((Grid)control);
                }
            }
        } //очистка текстовых полей чекбоксов и тд
        #region Форма планов приема
        private void PlaniPriemaLoad(string specialost)
        {
            foreach (Canvas canvas in planPriemaButtons)
            {
                canvas.Visibility = Visibility.Hidden;
            }

            int buttInd = 0;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_PlaniPriema", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@specialost", specialost);
                command.Parameters.AddWithValue("@budjet", CBFinBudjet.IsChecked == true ? "Бюджет" : "");
                command.Parameters.AddWithValue("@hozrash", CBFinHozrach.IsChecked == true ? "Хозрасчет" : "");
                command.Parameters.AddWithValue("@bazovoe", CBObrBaz.IsChecked == true ? "На основе базового образования" : "");
                command.Parameters.AddWithValue("@srednee", CBObrsred.IsChecked == true ? "На основе среднего образования" : "");
                command.Parameters.AddWithValue("@dnevnaya", CBFormDnev.IsChecked == true ? "Дневная" : "");
                command.Parameters.AddWithValue("@zaochnaya", CBformZaoch.IsChecked == true ? "Заочная" : "");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Canvas canvas = planPriemaButtons[buttInd];
                    canvas.Tag = reader[5];
                    canvas.Visibility = Visibility.Visible;
                    canvas.Children[2].SetValue(TextBlock.TextProperty, reader[3].ToString().ToUpper());
                    canvas.Children[3].SetValue(TextBlock.TextProperty, reader[2].ToString().ToUpper() + ". " + reader[4]);
                    canvas.Children[5].SetValue(TextBlock.TextProperty, reader[6].ToString());
                    canvas.Children[2].SetValue(TextBlock.TagProperty, reader[3].ToString() + ". " + reader[2].ToString() + ". " + reader[4].ToString());
                    buttInd++;
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                LabelFormaObrazovaniya.Content = ((Canvas)sender).Children[2].GetValue(TagProperty);
                currentPlanPriemaID = Convert.ToInt32(((Canvas)sender).Tag);
                AbiturientsTableLoad(currentPlanPriemaID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ButtonPos(int col) //изменение позиций кнопок под размер экрана
        {
            if (planPriemaColumn == col) return;
            int buttons = 0;
            int row = 1;
            while (buttons < planPriemaButtons.Count)
            {
                for (int i = 1; i <= col && buttons < planPriemaButtons.Count; i++)
                {
                    planPriemaButtons[buttons].SetValue(Grid.RowProperty, row);
                    planPriemaButtons[buttons].SetValue(Grid.ColumnProperty, i);
                    buttons++;
                }
                row++;
            }
            planPriemaColumn = col;
        }
        private void Filter_Click(object sender, RoutedEventArgs e) //Изменение фльтра
        {
            PlaniPriemaLoad(((TabItem)TabControl.SelectedItem).Header.ToString());
        }
        #endregion
        #region Список абитуриентов
        private void AbiturientsTableLoad(int PlanPriemaID)
        {
            addEditForm.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Hidden;
            GridDataTable.Visibility = Visibility.Visible;
            Filter.Visibility = Visibility.Hidden;
            try
            {
                abiturients = new List<AbiturientDGItem>();
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientList", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PlanPriema", PlanPriemaID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int num = 1;
                while (reader.Read())
                {
                    string vladelec = reader[4].ToString();
                    try
                    {
                        vladelec = vladelec.Split(' ')[0] + ' ' + vladelec.Split(' ')[1][0] + ". " + vladelec.Split(' ')[2][0] + ".";
                    }
                    catch { }
                    AbiturientDGItem abiturient = new AbiturientDGItem(num, (int)reader[0], reader[1].ToString(), vladelec, DateTime.Parse(reader[5].ToString()).ToShortDateString())
                    {
                        Lgoti = ""
                    };
                    if (Convert.ToBoolean(reader[2]) == true) { abiturient.Lgoti += "Cирота"; }
                    if (Convert.ToBoolean(reader[3]) == true) { abiturient.Lgoti += (abiturient.Lgoti.Length == 0 ? "" : "\n") + "Договор"; }
                    if (Convert.ToBoolean(reader[6]) == true) { abiturient.Status = "Зачислен"; }
                    else if (Convert.ToBoolean(reader[7]) == true) { abiturient.Status = "Документы выданы"; }
                    else abiturient.Status = "Документы приняты";

                    

                    abiturients.Add(abiturient);SqlConnection connection1 = new SqlConnection(connectionString);
                    SqlCommand command1 = new SqlCommand("Get_StatiAbiturienta", connection1);
                    command1.CommandType = CommandType.StoredProcedure;
                    command1.Parameters.AddWithValue("@abiturient", reader[0]);
                    connection1.Open();
                    SqlDataReader reader1 = command1.ExecuteReader();
                    string statyi = "";
                    while (reader1.Read())
                    {
                        statyi += reader1[0] + " ";
                    }
                    connection1.Close();
                    abiturient.Stati = statyi;
                    num++;
                }
                dataDridAbiturients.ItemsSource = abiturients;
                GridCountWrite.Text = abiturients.Count.ToString();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetExamList()
        {
            if (addEditFormobrazovanie.SelectedItem == null || EditEndButton.Visibility == Visibility.Visible) return;

            string letter;
            int num;
            string additional = "";
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sql1 = $"SELECT Буква FROM Специальность WHERE Наименование = '{addEditFormspecialnost.SelectedValue}'";
                SqlCommand command = new SqlCommand(sql1, connection);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                    letter = reader[0].ToString(); ;
                reader.Close();

                command = new SqlCommand("NextExamList", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("id", PlanPriemaID);
                reader = command.ExecuteReader();
                reader.Read();
                if (reader[0] == DBNull.Value) 
                    num = 1;
                else
                    num = Convert.ToInt32(reader[0]);
                reader.Close();
                connection.Close();
                if (addEditFormobushenie.SelectedValue.ToString() == "Заочная")
                    additional = "зб";
                else if (addEditFormFinansirovanie.SelectedValue.ToString() == "Хозрасчет")
                    additional = "х/р";
                else if (addEditFormobrazovanie.SelectedValue != null && addEditFormobrazovanie.SelectedValue.ToString() == "На основе среднего образования")
                    additional = "с";
                addEditFormExamList.Text = letter + num + additional;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Номер экзаменационного листа");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e) //открытие формы добавления
        {
            AddEndButton.Visibility = Visibility.Visible;
            EditEndButton.Visibility = Visibility.Collapsed;
            GridDataTable.Visibility = Visibility.Hidden;
            addEditForm.Visibility = Visibility.Visible;
            TabControlAddEditForm.SelectedIndex = 0;
            ClearData<StackPanel>(AddEditMainData);
            ClearData<StackPanel>(AddEditFormContacts);
            ClearData<StackPanel>(addEdifFormAtestati);
            ClearData<StackPanel>(addEdifFormCT);
            ClearData<StackPanel>(AddEditFormPassport);

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_SpecialnostiName", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@useFilter", 1);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                addEditFormspecialnost.Items.Clear();
                while (reader.Read())
                {
                    addEditFormspecialnost.Items.Add(reader[0]);
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            addEditFormspecialnost.SelectedIndex = TabControl.SelectedIndex;
            string[] dataFormiObucheniya = LabelFormaObrazovaniya.Content.ToString().Split('.');
            addEditFormobushenie.SelectedItem = dataFormiObucheniya[0];
            addEditFormFinansirovanie.SelectedItem = dataFormiObucheniya[1].Substring(1);
            addEditFormobrazovanie.SelectedItem = dataFormiObucheniya[2].Substring(1);

            foreach (TabItem item in TabControlAddEditForm.Items)
                item.Tag = "";

            try
            {
                string sql = "SELECT Наименование FROM ТипКонтакта";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                ContactDataType.Items.Clear();
                while (reader.Read())
                {
                    ContactDataType.Items.Add(reader.GetString(0));
                }
                ContactDataType.SelectedIndex = 0;
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                string sql = "SELECT Наименование, КоличествоБаллов FROM Шкала";
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                ScaleType.Items.Clear();
                while (reader.Read())
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = reader.GetString(0),
                        Tag = reader.GetInt32(1)
                    };
                    ScaleType.Items.Add(item);
                }
                ScaleType.SelectedIndex = 0;
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AbiturientInfoShow()
        {
            if ((AbiturientDGItem)dataDridAbiturients.SelectedItem == null) return;
            GridDataTable.Visibility = Visibility.Hidden;
            GridInfo.Visibility = Visibility.Visible;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaFullInfo", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();

                InfoFIO.Text = reader[0].ToString();
                infoSchool.Text = reader[1].ToString();
                infoYear.Text = reader[2].ToString();
                infoDate.Text = reader[3].ToString() != "" ? DateTime.Parse(reader[3].ToString()).ToString("D") : "-";
                infoLgoti.Text = ((AbiturientDGItem)dataDridAbiturients.SelectedItem).Lgoti.Replace('\n', ' ');
                if (infoLgoti.Text == "") infoLgotiTB.Visibility = Visibility.Collapsed; else infoLgotiTB.Visibility = Visibility.Visible;
                infoStati.Text = ((AbiturientDGItem)dataDridAbiturients.SelectedItem).Stati.Replace('\n', ' ');
                if (infoStati.Text == "") infoStatiTB.Visibility = Visibility.Collapsed; else infoStatiTB.Visibility = Visibility.Visible;
                infoDateVidoci.Text = reader[4].ToString() != "" ? DateTime.Parse(reader[4].ToString()).ToString("D") : "-";
                infoSeriya.Text = reader[5].ToString();
                infoPassNum.Text = reader[6].ToString();
                infokemvidan.Text = reader[7].ToString();
                infoIdentNum.Text = reader[8].ToString();
                infoGrajdanstvo.Text = reader[9].ToString();
                if (reader[10].ToString() == "")
                {
                    RowInfoWork.Height = new GridLength(0);
                }
                else
                {
                    infoMestoRaboti.Text = reader[10].ToString();
                    infoDoljnost.Text = reader[11].ToString();
                    RowInfoWork.Height = new GridLength(91);
                }
                infoVladelec.Text = reader[12].ToString();
                infoRedaktor.Text = reader[13].ToString();
                if (infoRedaktor.Text == "") infoRedaktorTB.Visibility = Visibility.Hidden; else infoRedaktorTB.Visibility = Visibility.Visible;
                infoDateVvoda.Text = reader[14].ToString();
                infoDateRedact.Text = reader[15].ToString();
                if (infoDateRedact.Text == "") infoDateRedactTB.Visibility = Visibility.Hidden; else infoDateRedactTB.Visibility = Visibility.Visible;
                if ((bool)reader[16]) InfoShow_Status.Text = "Зачислен"; else if ((bool)reader[17]) InfoShow_Status.Text = "Отозвано"; else InfoShow_Status.Text = "Принято";
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try//Аттестаты
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                dataAdapter.Fill(dataTable);
                AtestatGrid.ItemsSource = dataTable.DefaultView;
                connection.Close();
                atestatCount.Text = AtestatGrid.Items.Count.ToString();

                for (int i = 1; i < AtestatGrid.Columns.Count - 2; i++)
                {
                    bool isNull = true;
                    for (int j = 0; j < AtestatGrid.Items.Count; j++)
                    {
                        if (((DataRowView)AtestatGrid.Items[j])[i].ToString() != "")
                        {
                            isNull = false;
                        }
                    }
                    if (isNull) AtestatGrid.Columns[i].Visibility = Visibility.Hidden;
                    else AtestatGrid.Columns[i].Visibility = Visibility.Visible;
                }//скрытие неиспользуемых столбцов
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try//цт
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaSertificati", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                dataAdapter.Fill(dataTable);
                SertificatiCTGrid.ItemsSource = dataTable.DefaultView;
                connection.Close();
                sertificatCount.Text = SertificatiCTGrid.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try//Контактные данные
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_AbiturientaKontakti", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                connection.Open();
                dataAdapter.Fill(dataTable);
                kontaktnieDannieGrid.ItemsSource = dataTable.DefaultView;
                connection.Close();
                contactsCount.Text = kontaktnieDannieGrid.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } //открытие информации об абитуриенте
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)//информация о абитуиенте
        {
            AbiturientInfoShow();
        }
        private void Image_AtestatRedakt(object sender, MouseButtonEventArgs e)
        {
            Image_MouseUp_1(sender, e);
            TabControlAddEditForm.SelectedIndex = 2;
        }
        private void Image_KontaktsRedakt(object sender, MouseButtonEventArgs e)
        {
            Image_MouseUp_1(sender, e);
            TabControlAddEditForm.SelectedIndex = 1;
        }
        private void Image_CTRedakt(object sender, MouseButtonEventArgs e)
        {
            Image_MouseUp_1(sender, e);
            TabControlAddEditForm.SelectedIndex = 3;
        }
        private void Image_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            GridInfo.Visibility = Visibility.Hidden;
            AddEndButton.Visibility = Visibility.Collapsed;
            EditEndButton.Visibility = Visibility.Visible;
            if ((AbiturientDGItem)dataDridAbiturients.SelectedItem != null)
            {
                //очистка старых данных
                GridDataTable.Visibility = Visibility.Hidden;
                addEditForm.Visibility = Visibility.Visible;
                TabControlAddEditForm.SelectedIndex = 0;
                foreach (TabItem item in TabControlAddEditForm.Items)
                    item.Tag = "True";
                ClearData<StackPanel>(AddEditMainData);
                ClearData<StackPanel>(AddEditFormContacts);
                ClearData<StackPanel>(addEdifFormAtestati);
                ClearData<StackPanel>(addEdifFormCT);
                ClearData<StackPanel>(AddEditFormPassport);
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Get_SpecialnostiName", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@useFilter", 1);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    addEditFormspecialnost.Items.Clear();
                    while (reader.Read())
                    {
                        addEditFormspecialnost.Items.Add(reader[0]);
                    }
                    reader.Close();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                addEditFormspecialnost.SelectedIndex = TabControl.SelectedIndex;
                string[] dataFormiObucheniya = LabelFormaObrazovaniya.Content.ToString().Split('.');
                addEditFormobushenie.SelectedItem = dataFormiObucheniya[0];
                addEditFormFinansirovanie.SelectedItem = dataFormiObucheniya[1].Substring(1);
                addEditFormobrazovanie.SelectedItem = dataFormiObucheniya[2].Substring(1);
                //Запись данных
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Get_AbiturientMainInfo", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    reader.Read();
                    addEditFormSurename.Text = reader[0].ToString();
                    addEditFormName.Text = reader[1].ToString();
                    addEditFormOtchestvo.Text = reader[2].ToString();
                    addEditFormShool.Text = reader[3].ToString();
                    addEditFormGraduationYear.Text = reader[4].ToString();
                    dateOfBirth.Text = reader[5].ToString().Split(' ')[0];
                    PassportDateVidachi.Text = reader[6].ToString().Split(' ')[0];
                    PassportSeriya.Text = reader[7].ToString();
                    PassportNomer.Text = reader[8].ToString();
                    PassportVidan.Text = reader[9].ToString();
                    PassportIdentNum.Text = reader[10].ToString();
                    AddFormGrajdanstvo.Text = reader[11].ToString();
                    textBoxWorkPlace.Text = reader[12].ToString();
                    textBoxDoljnost.Text = reader[13].ToString();
                    addEditFormObshejitie.IsChecked = reader[14].ToString() == "True";
                    addEditForm_CheckBox_DetiSiroti.IsChecked = reader[15].ToString() == "True";
                    addEditForm_CheckBox_Dogovor.IsChecked = reader[16].ToString() == "True";
                    addEditFormExamList.Text = reader[17].ToString();
                    reader.Close();

                    SqlConnection con = new SqlConnection(connectionString);
                    con.Open();
                    for (int k = 0; k < 2; k++)
                    {
                        StackPanel panel = Stati.Children[k] as StackPanel;
                        for (int j = 0; j < 3; j++)
                        {
                            CheckBox checkBox = panel.Children[j] as CheckBox;
                            SqlCommand command1 = new SqlCommand("HasStatya", con);
                            command1.CommandType = CommandType.StoredProcedure;
                            command1.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                            command1.Parameters.AddWithValue("@statya", checkBox.Content);
                            SqlDataReader reader1 = command1.ExecuteReader();
                            if (reader1.HasRows)
                            {
                                checkBox.IsChecked = true;
                            }
                            reader1.Close();
                        }
                    }
                    con.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }//основные данные и паспортные данные
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaKontakti", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                    SqlDataReader reader = command.ExecuteReader();
                    int lastPoint = 0;
                    while (reader.Read())
                    {
                        for (int i = lastPoint; i < AddEditFormContacts.Children.Count; i++)
                        {
                            string Tag = ((StackPanel)AddEditFormContacts.Children[i]).Tag.ToString();
                            if (Tag == "VisibleField" || Tag == "HIddenField")
                            {
                                try
                                {
                                    ComboBox comboBox = (ComboBox)((StackPanel)AddEditFormContacts.Children[i]).Children[3];
                                    string sql = "SELECT Наименование FROM ТипКонтакта";
                                    SqlConnection connection1 = new SqlConnection(connectionString);
                                    SqlCommand command1 = new SqlCommand(sql, connection1);
                                    connection1.Open();
                                    SqlDataReader reader1 = command1.ExecuteReader();
                                    comboBox.Items.Clear();
                                    while (reader1.Read())
                                        comboBox.Items.Add(reader1[0]);
                                    comboBox.SelectedIndex = 0;
                                    connection1.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                                StackPanel stackPanel = AddEditFormContacts.Children[i] as StackPanel;
                                stackPanel.Visibility = Visibility.Visible;
                                ((ComboBox)stackPanel.Children[3]).SelectedItem = reader[2].ToString();
                                ((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).Text = reader[3].ToString();
                                lastPoint = i + 1;
                                break;
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }//контактные данные
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                    SqlDataReader reader = command.ExecuteReader();
                    int lastPoint = 0;

                    bool haveAttestat = false;
                    while (reader.Read())
                    {
                        for (int i = lastPoint; i < addEdifFormAtestati.Children.Count; i++)
                        {
                            string AttestatTag = ((StackPanel)addEdifFormAtestati.Children[i]).Tag.ToString();
                            if (AttestatTag == "VisibleField" || AttestatTag == "HIddenField")
                            {
                                haveAttestat = true;
                                try
                                {
                                    ComboBox comboBox = (ComboBox)((StackPanel)addEdifFormAtestati.Children[i]).Children[8];
                                    string sql1 = "SELECT Наименование, КоличествоБаллов FROM Шкала";
                                    SqlConnection connection1 = new SqlConnection(connectionString);
                                    SqlCommand command1 = new SqlCommand(sql1, connection1);
                                    connection1.Open();
                                    SqlDataReader reader1 = command1.ExecuteReader();
                                    comboBox.Items.Clear();
                                    while (reader1.Read())
                                    {
                                        ComboBoxItem boxItem = new ComboBoxItem();
                                        boxItem.Content = reader1.GetString(0);
                                        boxItem.Tag = reader1.GetInt32(1);
                                        comboBox.Items.Add(boxItem);
                                    }
                                    reader1.Close();
                                    addEdifFormAtestati.Height += 450;
                                    comboBox.SelectedIndex = 0;
                                    connection1.Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                                StackPanel stackPanel = addEdifFormAtestati.Children[i] as StackPanel;
                                stackPanel.Visibility = Visibility.Visible;

                                ((TextBox)stackPanel.Children[3]).Text = reader[1].ToString();

                                Grid grid = stackPanel.Children[4] as Grid;
                                for (int j = 4; j < 32; j += 2)
                                {
                                    ((TextBox)grid.Children[j]).Text = reader[j / 2].ToString();
                                }
                                ((ComboBox)stackPanel.Children[8]).SelectedItem = reader[17].ToString();
                                lastPoint = i + 1;
                                break;
                            }
                        }
                    }
                    reader.Close();
                    if (haveAttestat == false)
                    {
                        try
                        {
                            string sql = "SELECT Наименование, КоличествоБаллов FROM Шкала";
                            SqlCommand command2 = new SqlCommand(sql, connection);
                            SqlDataReader reader2 = command2.ExecuteReader();
                            ScaleType.Items.Clear();
                            while (reader2.Read())
                            {
                                ComboBoxItem item = new ComboBoxItem()
                                {
                                    Content = reader2.GetString(0),
                                    Tag = reader2.GetInt32(1)
                                };
                                ScaleType.Items.Add(item);
                            }
                            ScaleType.SelectedIndex = 0;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message); 
                }//Аттестаты
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaSertificati", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        for (int i = 0; i < addEdifFormCT.Children.Count; i++)
                        {
                            if (addEdifFormCT.Children[i].Visibility == Visibility.Collapsed)
                            {
                                Grid grid = ((StackPanel)addEdifFormCT.Children[i]).Children[2] as Grid;

                                ((TextBox)grid.Children[1]).Text = reader[1].ToString();
                                ((ComboBox)grid.Children[5]).SelectedItem = reader[2].ToString();
                                ((Xceed.Wpf.Toolkit.MaskedTextBox)grid.Children[3]).Text = reader[3].ToString();
                                ((TextBox)grid.Children[7]).Text = reader[4].ToString();
                                addEdifFormCT.Height += 257;
                                addEdifFormCT.Children[i].Visibility = Visibility.Visible;
                                break;
                            }
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }//сертификаты цт
            }
        } //нажатие кнопки редактирования
        private void Image_MouseUp_2(object sender, MouseButtonEventArgs e)
        {
            if ((AbiturientDGItem)dataDridAbiturients.SelectedItem == null) return;
            if (e.ChangedButton == MouseButton.Left)
            {
                Image image = sender as Image;
                ContextMenu contextMenu = image.ContextMenu;
                contextMenu.PlacementTarget = image;
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
        }//открытие контекстного меню
        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<AbiturientDGItem> newabiturients = new List<AbiturientDGItem>();

            newabiturients = abiturients.FindAll(x => Regex.IsMatch(x.FIO.ToLower(), $@"{textBoxSearch.Text.ToLower()}"));

            dataDridAbiturients.ItemsSource = newabiturients;
        }//поиск в таблице абитуриентов
        private void Abiturient_Delete(object sender, RoutedEventArgs e)
        {
            if ((AbiturientDGItem)dataDridAbiturients.SelectedItem == null) return;
            if (MessageBox.Show($"Отметить данную запись как отозванно?\n\n  {((AbiturientDGItem)dataDridAbiturients.SelectedItem).FIO}", "Забрать документы", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Del_AbiturientMarks", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    AbiturientsTableLoad(currentPlanPriemaID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }//Удаление
        private void Abiturient_SetStatus(object sender, RoutedEventArgs e)
        {
            if ((AbiturientDGItem)dataDridAbiturients.SelectedItem == null) return;
            try
            {
                foreach (AbiturientDGItem abiturient in dataDridAbiturients.SelectedItems)
                {
                    string[] stat = ((MenuItem)sender).Tag.ToString().Split(',');
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    string sql = $"UPDATE Абитуриент SET Удалено = {stat[0]}, АбитуриентЗачислен = {stat[1]} WHERE IDАбитуриента = {abiturient.ID}";
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                }
                if (GridDataTable.Visibility == Visibility.Visible)
                {
                    AbiturientsTableLoad(currentPlanPriemaID);
                }
                else AbiturientInfoShow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//Усановка статуса абитуриента
        private void Table_PressDelete(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (dataDridAbiturients.SelectedItems.Count == 0) return;
                string delItemsName = "";
                int i = 0;
                {
                    AbiturientDGItem abiturient;
                    do
                    {
                        delItemsName += $"{((AbiturientDGItem)dataDridAbiturients.SelectedItems[i]).FIO}\n ";
                        abiturient = dataDridAbiturients.SelectedItems[i] as AbiturientDGItem;
                        i++;
                    } while (i < 3 && i < dataDridAbiturients.SelectedItems.Count && abiturient != null);
                    if (dataDridAbiturients.SelectedItems.Count - 3 > 0)
                        delItemsName += $"И еще {dataDridAbiturients.SelectedItems.Count - 3} запись(-и)";
                }

                if (MessageBox.Show($"Отметить данные записи как отозванно?\n\n {delItemsName}", "Забрать документы", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        foreach (AbiturientDGItem abiturient in dataDridAbiturients.SelectedItems)
                        {
                            SqlConnection connection = new SqlConnection(connectionString);
                            SqlCommand command = new SqlCommand("Del_AbiturientMarks", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@abiturient", abiturient.ID);
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        AbiturientsTableLoad(currentPlanPriemaID);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            if(e.Key == Key.Enter)
            {
                List<AbiturientDGItem> list = new List<AbiturientDGItem>();
                if (dataDridAbiturients.SelectedItems.Count > 1) 
                {
                    foreach (AbiturientDGItem item in dataDridAbiturients.SelectedItems)
                    {
                        list.Add(item);
                    }
                }

            }
        }//Удаление на кнопку Del
        private void DataDridAbiturients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AbiturientInfoShow();
        }//открытие информации абитуриена по двойному клику
        #endregion
        #region Форма добавления/редактирования
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                AddFormGrajdanstvo.Text = "Белорусское";
            }
        }
        private void AddFormGrajdanstvo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text != "")
            {
                ((TextBox)sender).Tag = "";
            }
            if (((TextBox)sender).Text == "Белорусское")
                AddFormChekBoxGrajdanstvo.IsChecked = true;
            else
                AddFormChekBoxGrajdanstvo.IsChecked = false;
        }
        private void DateOfBirth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DateTime.TryParse(((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Text, out _))
            {
                ((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Tag = "";
            }
            else
                ((Xceed.Wpf.Toolkit.MaskedTextBox)sender).Tag = "Error";
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < AddEditFormContacts.Children.Count; i++)
            {
                if (AddEditFormContacts.Children[i].Visibility == Visibility.Collapsed)
                {
                    try
                    {
                        ComboBox comboBox = (ComboBox)((StackPanel)AddEditFormContacts.Children[i]).Children[3];
                        string sql = "SELECT Наименование FROM ТипКонтакта";
                        SqlConnection connection = new SqlConnection(connectionString);
                        SqlCommand command = new SqlCommand(sql, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        comboBox.Items.Clear();
                        while (reader.Read())
                            comboBox.Items.Add(reader[0]);
                        comboBox.SelectedIndex = 0;
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    AddEditFormContacts.Children[i].Visibility = Visibility.Visible;
                    break;
                }
            }
        }//добавление нового контакта

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

        private void Button_NewAtestat(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < addEdifFormAtestati.Children.Count; i++)
            {
                if (addEdifFormAtestati.Children[i].Visibility == Visibility.Collapsed)
                {
                    try
                    {
                        ComboBox comboBox = (ComboBox)((StackPanel)addEdifFormAtestati.Children[i]).Children[8];
                        string sql = "SELECT Наименование, КоличествоБаллов FROM Шкала";
                        SqlConnection connection = new SqlConnection(connectionString);
                        SqlCommand command = new SqlCommand(sql, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        comboBox.Items.Clear();
                        while (reader.Read())
                        {
                            ComboBoxItem boxItem = new ComboBoxItem()
                            {
                                Content = reader.GetString(0),
                                Tag = reader.GetInt32(1)
                            };
                            comboBox.Items.Add(boxItem);
                        }
                        comboBox.SelectedIndex = 0;
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    addEdifFormAtestati.Height += 450;
                    addEdifFormAtestati.Children[i].Visibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void Tb_IdentNuber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9a-zA-Z]+");
            bool isMatch = regex.IsMatch(e.Text);
            ttpIdentNum.PlacementTarget = (UIElement)sender;
            ttpIdentNum.IsOpen = !isMatch;
            e.Handled = !isMatch;
        }

        private void Tb_SeriyaPasporta_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[a-zA-Z]+$");
            bool isMatch = regex.IsMatch(e.Text);
            ttpSerya.PlacementTarget = (UIElement)sender;
            ttpSerya.IsOpen = !isMatch;
            e.Handled = !isMatch;
        }
        private void PassportSeriya_TextInput(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int selStart = tb.SelectionStart;
            tb.Text = tb.Text.ToUpper();
            tb.SelectionStart = selStart;
            tb.Tag = "";
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static readonly Regex _regex = new Regex("[^0-9]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void ButtonNewSertificatCT(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < addEdifFormCT.Children.Count; i++)
            {
                if (addEdifFormCT.Children[i].Visibility == Visibility.Collapsed)
                {
                    addEdifFormCT.Height += 257;
                    addEdifFormCT.Children[i].Visibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void Button_PrewPage(object sender, RoutedEventArgs e)
        {
            TabControlAddEditForm.SelectedIndex -= 1;
            if (((TabItem)TabControlAddEditForm.SelectedItem).IsEnabled == false)
                TabControlAddEditForm.SelectedIndex--;
        }

        private void Button_CloseNote(object sender, RoutedEventArgs e)
        {
            ((StackPanel)((Grid)((Button)sender).Parent).Parent).Visibility = Visibility.Collapsed;
        }
        private void TextBoxIsCorrect(TextBox textBox, ref bool correct)
        {
            if (textBox.Text == "" || Convert.ToString(textBox.Tag) == "Error")
            {
                textBox.Tag = "Error";
                correct = false;
            }
            else
            {
                textBox.Tag = "";
            }
        }
        private void Button_NextStep_1(object sender, RoutedEventArgs e)
        {
            if (Correct_1())
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }
            else
            {
                ((TabItem)TabControlAddEditForm.Items[0]).Tag = "";
                ScrollAddMain.ScrollToVerticalOffset(0);
                MessageBox.Show("Некоторые даные были введены некорректно!");
            }
        }
        private bool Correct_1()
        {
            bool correct = true;
            TextBoxIsCorrect(addEditFormSurename, ref correct);
            TextBoxIsCorrect(addEditFormName, ref correct);
            TextBoxIsCorrect(addEditFormOtchestvo, ref correct);
            TextBoxIsCorrect(AddFormGrajdanstvo, ref correct);
            TextBoxIsCorrect(addEditFormShool, ref correct);
            correct = (string)dateOfBirth.Tag != "Error" && (string)addEditFormGraduationYear.Tag != "Error";
            if (textBoxWorkPlace.Text != "" && textBoxDoljnost.Text == "")
            {
                correct = false;
                textBoxDoljnost.Tag = "Error";
            }
            if (textBoxWorkPlace.Text == "" && textBoxDoljnost.Text != "")
            {
                correct = false;
                textBoxWorkPlace.Tag = "Error";
            }
            return correct;
        }
        private void Button_NextStep_2(object sender, RoutedEventArgs e)
        {
            if (Correct_2())
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }

        }
        private bool Correct_2()
        {
            bool correct = true;
            for (int i = 0; i < AddEditFormContacts.Children.Count; i++)
            {
                if (AddEditFormContacts.Children[i].Visibility == Visibility.Visible)
                {
                    if (!(AddEditFormContacts.Children[i] is StackPanel stackPanel)) break;
                    if (((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).IsMaskCompleted == false || ((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).Text == "")
                    {
                        correct = false;
                        ((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).Tag = "Error";
                    }
                    else
                        ((Xceed.Wpf.Toolkit.MaskedTextBox)stackPanel.Children[5]).Tag = "";
                }
            }
            return correct;
        }
        private void Button_NextStep_3(object sender, RoutedEventArgs e)
        {
            if (Correct_3())
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                    TabControlAddEditForm.SelectedIndex++;
                if(((TabItem)TabControlAddEditForm.SelectedItem).IsEnabled == false)
                    TabControlAddEditForm.SelectedIndex++;
            }
        }
        private bool Correct_3()
        {
            bool correct = true;
            for (int i = 0; i < addEdifFormAtestati.Children.Count; i++)
            {
                if (addEdifFormAtestati.Children[i].Visibility == Visibility.Visible)
                {
                    if (!(addEdifFormAtestati.Children[i] is StackPanel stackPanel)) break;
                    if (((TextBox)stackPanel.Children[3]).Text == "")
                    {
                        correct = false;
                        ((TextBox)stackPanel.Children[3]).Tag = "Error";
                    }
                    else
                        ((TextBox)stackPanel.Children[3]).Tag = "";
                }
            }
            return correct;
        }
        private void Button_NextStep_4(object sender, RoutedEventArgs e)
        {
            if (Correct_4())
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }
        }
        private bool Correct_4()
        {
            bool correct = true;
            for (int i = 0; i < addEdifFormCT.Children.Count; i++)
            {
                if (addEdifFormCT.Children[i].Visibility == Visibility.Visible)
                {
                    if (addEdifFormCT.Children[i] as StackPanel == null) break;
                    Grid grid = ((StackPanel)addEdifFormCT.Children[i]).Children[2] as Grid;
                    TextBoxIsCorrect((TextBox)grid.Children[1], ref correct);
                    TextBoxIsCorrect((TextBox)grid.Children[7], ref correct);
                    if (((Xceed.Wpf.Toolkit.MaskedTextBox)grid.Children[3]).IsMaskCompleted == false)
                    {
                        ((Xceed.Wpf.Toolkit.MaskedTextBox)grid.Children[3]).Tag = "Error";
                        correct = false;
                    }
                    else
                        ((Xceed.Wpf.Toolkit.MaskedTextBox)grid.Children[3]).Tag = "";
                }
            }
            return correct;
        }

        //Добавление
        private void Button_AddEnd(object sender, RoutedEventArgs e)
        {
            if(!EnterIsCorrect())return;
            int AbiturientID = 0;
            //Основные данные
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Add_Abiturient", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@surename", addEditFormSurename.Text);
                command.Parameters.AddWithValue("@name", addEditFormName.Text);
                command.Parameters.AddWithValue("@otchestvo", addEditFormOtchestvo.Text);
                command.Parameters.AddWithValue("@shool", addEditFormShool.Text);
                command.Parameters.AddWithValue("@graduationYear", addEditFormGraduationYear.Text);
                command.Parameters.AddWithValue("@grajdanstvoRB", AddFormChekBoxGrajdanstvo.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@grajdanstvo", AddFormGrajdanstvo.Text);
                command.Parameters.AddWithValue("@obshejitie", addEditFormObshejitie.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@planPriema", PlanPriemaID);
                command.Parameters.AddWithValue("@workPlace", textBoxWorkPlace.Text);
                command.Parameters.AddWithValue("@doljnost", textBoxDoljnost.Text);
                command.Parameters.AddWithValue("@sirota", addEditForm_CheckBox_DetiSiroti.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@dogovor", addEditForm_CheckBox_Dogovor.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@user", userId);
                command.Parameters.AddWithValue("@ExamList", addEditFormExamList.Text);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                AbiturientID = Convert.ToInt32(reader[0]);
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Основные данные");
            }
            //Контактные данные
            try
            {
                for (int i = 0; i < AddEditFormContacts.Children.Count - 1; i++)
                {
                    if (AddEditFormContacts.Children[i].Visibility == Visibility.Visible && (AddEditFormContacts.Children[i] as StackPanel) != null)
                    {
                        StackPanel stackPanel = AddEditFormContacts.Children[i] as StackPanel;
                        SqlConnection connection = new SqlConnection(connectionString);
                        SqlCommand command = new SqlCommand("Add_ContctData", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@abiturient", AbiturientID);
                        command.Parameters.AddWithValue("@svedeniya", ((TextBox)stackPanel.Children[5]).Text.Replace("_", string.Empty));
                        command.Parameters.AddWithValue("@contactType", ((ComboBox)stackPanel.Children[3]).SelectedItem);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Контактные данные");
            }
            //Образование
            try
            {
                for (int i = 0; i < addEdifFormAtestati.Children.Count - 1; i++)
                {
                    if (addEdifFormAtestati.Children[i].Visibility == Visibility.Visible && (addEdifFormAtestati.Children[i] as StackPanel) != null)
                    {
                        SqlConnection connection = new SqlConnection(connectionString);

                        StackPanel stackPanel = addEdifFormAtestati.Children[i] as StackPanel;

                        List<int> marks = new List<int>();
                        Grid grid = stackPanel.Children[4] as Grid;
                        connection.Open();
                        for (int j = 4; j < 32; j += 2)
                        {
                            if (((TextBox)grid.Children[j]).Text != "")
                            {
                                marks.Add(Convert.ToInt16(((TextBox)grid.Children[j]).Text));
                            }
                            else break;
                        }

                        double sum = 0;
                        int col = 0;
                        for (int j = 0; j < marks.Count; j++)
                        {
                            sum += (marks[j]) * (j + 1);
                            col += marks[j];
                        }
                        double markAvg = sum / col;

                        SqlCommand command = new SqlCommand("Add_Atestat", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@abiturient", AbiturientID);
                        ComboBoxItem item = (ComboBoxItem)((ComboBox)stackPanel.Children[8]).SelectedItem;
                        command.Parameters.AddWithValue("@scaleName", item.Content);
                        command.Parameters.AddWithValue("@attestatSeries", ((TextBox)stackPanel.Children[3]).Text);
                        command.Parameters.AddWithValue("@avgMarks", markAvg.ToString().Replace(',', '.'));
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int AtestatID = (int)reader[0];
                        reader.Close();
                        for (int j = 0; j < marks.Count; j++)
                        {
                            command = new SqlCommand("Add_Mark", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@mark", j + 1);
                            command.Parameters.AddWithValue("@colvo", marks[j]);
                            command.Parameters.AddWithValue("@attestat", AtestatID);
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Образование");
            }
            //Сертификаты ЦТ
            try
            {
                for (int i = 0; i < addEdifFormCT.Children.Count - 1; i++)
                {
                    if (addEdifFormCT.Children[i].Visibility == Visibility.Visible && (addEdifFormCT.Children[i] as StackPanel) != null)
                    {
                        Grid grid = (Grid)(addEdifFormCT.Children[i] as StackPanel).Children[2];
                        SqlConnection connection = new SqlConnection(connectionString);
                        SqlCommand command = new SqlCommand("Add_Sertificat", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@sertificat", AbiturientID);
                        command.Parameters.AddWithValue("@disciplin", ((ComboBoxItem)((ComboBox)grid.Children[5]).SelectedItem).Content);
                        command.Parameters.AddWithValue("@mark", ((TextBox)grid.Children[7]).Text);
                        command.Parameters.AddWithValue("@decMark", (Convert.ToDouble(((TextBox)grid.Children[7]).Text) / 10).ToString().Replace(',', '.'));
                        command.Parameters.AddWithValue("@year", ((TextBox)grid.Children[3]).Text);
                        command.Parameters.AddWithValue("@serialNum", ((TextBox)grid.Children[1]).Text);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Сертификаты ЦТ");
            }
            //Паспортные данные
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Add_PassportData", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                command.Parameters.AddWithValue("@dateIssue", PassportDateVidachi.Text);
                command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth.Text);
                command.Parameters.AddWithValue("@series", PassportSeriya.Text);
                command.Parameters.AddWithValue("@PasspornNum", PassportNomer.Text);
                command.Parameters.AddWithValue("@name", PassportVidan.Text);
                command.Parameters.AddWithValue("@identNum", PassportIdentNum.Text);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Паспортные данные");
            }
            //Статьи
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                for (int i = 0; i < 2; i++)
                {
                    StackPanel stackPanel = (StackPanel)Stati.Children[i];
                    for (int j = 0; j < 3; j++)
                    {
                        CheckBox checkBox = (CheckBox)stackPanel.Children[j];
                        if (checkBox.IsChecked == true)
                        {
                            string sql1 = $"SELECT IDСтатьи FROM Статьи WHERE ПолноеНаименование LIKE N'{checkBox.Content}'";
                            SqlCommand command = new SqlCommand(sql1, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            command = new SqlCommand("Add_Stati", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@abiturient", AbiturientID);
                            command.Parameters.AddWithValue("@statya", reader[0]);
                            reader.Close();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Статьи");
            } 
            AbiturientsTableLoad(currentPlanPriemaID);
        }
        
        //завершение редактирования
        private void Button_EditEnd(object sender, RoutedEventArgs e)
        {
            if (!EnterIsCorrect()) return;
            int AbiturientID = ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID;
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Update_MainData", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@surename", addEditFormSurename.Text);
                command.Parameters.AddWithValue("@name", addEditFormName.Text);
                command.Parameters.AddWithValue("@otchestvo", addEditFormOtchestvo.Text);
                command.Parameters.AddWithValue("@shool", addEditFormShool.Text);
                command.Parameters.AddWithValue("@graduationYear", addEditFormGraduationYear.Text);
                command.Parameters.AddWithValue("@grajdaninRB", AddFormChekBoxGrajdanstvo.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@grajdanstvo", AddFormGrajdanstvo.Text);
                command.Parameters.AddWithValue("@obshejitie", addEditFormObshejitie.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@planPriema", PlanPriemaID);
                command.Parameters.AddWithValue("@workPlase", textBoxWorkPlace.Text);
                command.Parameters.AddWithValue("@doljnost", textBoxDoljnost.Text);
                command.Parameters.AddWithValue("@sirota", addEditForm_CheckBox_DetiSiroti.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@dogovor", addEditForm_CheckBox_Dogovor.IsChecked == true ? 1 : 0);
                command.Parameters.AddWithValue("@redaktor", userId);
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                command.Parameters.AddWithValue("@ExamList", addEditFormExamList.Text);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Основные данные");
            }//Основные данные +-
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqldel = $"DELETE FROM КонтактныеДанные WHERE IDАбитуриента = {AbiturientID}";
                SqlCommand del = new SqlCommand(sqldel, connection);
                del.ExecuteNonQuery();
                for (int i = 0; i < AddEditFormContacts.Children.Count - 1; i++)
                {
                    if (AddEditFormContacts.Children[i].Visibility == Visibility.Visible && (AddEditFormContacts.Children[i] as StackPanel) != null)
                    {
                        StackPanel stackPanel = AddEditFormContacts.Children[i] as StackPanel;
                        
                        SqlCommand command = new SqlCommand("Add_ContctData", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@abiturient", AbiturientID);
                        command.Parameters.AddWithValue("@svedeniya", ((TextBox)stackPanel.Children[5]).Text.Replace("_", string.Empty));
                        command.Parameters.AddWithValue("@contactType", ((ComboBox)stackPanel.Children[3]).SelectedItem);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Контактные данные");
            }//Контактные данные* ?
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqldel = $"DELETE FROM Атестат WHERE IDАбитуриента = {AbiturientID}";
                SqlCommand del = new SqlCommand(sqldel, connection);
                del.ExecuteNonQuery();
                for (int i = 0; i < addEdifFormAtestati.Children.Count - 1; i++)
                {
                    if (addEdifFormAtestati.Children[i].Visibility == Visibility.Visible && (addEdifFormAtestati.Children[i] as StackPanel) != null)
                    {
                        StackPanel stackPanel = addEdifFormAtestati.Children[i] as StackPanel;

                        List<int> marks = new List<int>();
                        List<int> marksDec = new List<int>();
                        Grid grid = stackPanel.Children[4] as Grid;

                        for (int j = 4; j < 32; j += 2)
                        {
                            if (((TextBox)grid.Children[j]).Text != "")
                            {
                                marks.Add(Convert.ToInt16(((TextBox)grid.Children[j]).Text));
                            }
                            else
                                break;
                        }

                        double sum = 0;
                        int col = 0;
                        for (int j = 0; j < marks.Count; j++)
                        {
                            sum += (marks[j]) * (j + 1);
                            col += marks[j];
                        }
                        double markAvg = sum / col;

                        SqlCommand command = new SqlCommand("Add_Atestat", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@abiturient", AbiturientID);
                        ComboBoxItem item = (ComboBoxItem)((ComboBox)stackPanel.Children[8]).SelectedItem;
                        command.Parameters.AddWithValue("@scaleName", item.Content);
                        command.Parameters.AddWithValue("@attestatSeries", ((TextBox)stackPanel.Children[3]).Text);
                        command.Parameters.AddWithValue("@avgMarks", Math.Round(markAvg,2));
                        SqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        int AtestatID = (int)reader[0];
                        reader.Close();
                        for (int j = 0; j < marks.Count; j++)
                        {
                            command = new SqlCommand("Add_Mark", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@attestat", AtestatID);
                            command.Parameters.AddWithValue("@mark", j + 1);
                            command.Parameters.AddWithValue("@colvo", marks[j]);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Образование");
            }//Образование* ?
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqldel = $"DELETE FROM СертификатЦТ WHERE IDАбитуриента = {AbiturientID}";
                SqlCommand del = new SqlCommand(sqldel, connection);
                del.ExecuteNonQuery();
                for (int i = 0; i < addEdifFormCT.Children.Count - 1; i++)
                {
                    if (addEdifFormCT.Children[i].Visibility == Visibility.Visible && (addEdifFormCT.Children[i] as StackPanel) != null)
                    {
                        Grid grid = (Grid)(addEdifFormCT.Children[i] as StackPanel).Children[2];

                        SqlCommand command = new SqlCommand("Add_Sertificat", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@sertificat", AbiturientID);
                        command.Parameters.AddWithValue("@disciplin", ((ComboBoxItem)((ComboBox)grid.Children[5]).SelectedItem).Content);
                        command.Parameters.AddWithValue("@mark", ((TextBox)grid.Children[7]).Text);
                        command.Parameters.AddWithValue("@decMark", (Convert.ToDouble(((TextBox)grid.Children[7]).Text) / 10).ToString().Replace(',', '.'));
                        command.Parameters.AddWithValue("@year", ((TextBox)grid.Children[3]).Text);
                        command.Parameters.AddWithValue("@serialNum", ((TextBox)grid.Children[1]).Text);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Сертификаты ЦТ");
            }//Сертификаты ЦТ* ?
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Update_PasportData", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@dateVidachi", PassportDateVidachi.Text);
                command.Parameters.AddWithValue("@dateOfBirth", dateOfBirth.Text);
                command.Parameters.AddWithValue("@seriya", PassportSeriya.Text);
                command.Parameters.AddWithValue("@pasportNum", PassportNomer.Text);
                command.Parameters.AddWithValue("@vidan", PassportVidan.Text);
                command.Parameters.AddWithValue("@identNum", PassportIdentNum.Text);
                command.Parameters.AddWithValue("@abiturient", AbiturientID);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Паспортные данные");
            }//Паспортные данные*
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqldel = $"DELETE FROM СтатьиАбитуриента WHERE IDАбитуриента = {AbiturientID}";
                SqlCommand del = new SqlCommand(sqldel, connection);
                del.ExecuteNonQuery();
                for (int i = 0; i < 2; i++)
                {
                    StackPanel stackPanel = (StackPanel)Stati.Children[i];
                    for (int j = 0; j < 3; j++)
                    {
                        CheckBox checkBox = (CheckBox)stackPanel.Children[j];
                        if (checkBox.IsChecked == true)
                        {
                            string sql1 = $"SELECT IDСтатьи FROM Статьи WHERE ПолноеНаименование LIKE N'{checkBox.Content}'";
                            SqlCommand command = new SqlCommand(sql1, connection);
                            SqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            
                            command = new SqlCommand("Add_Stati", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@abiturient", AbiturientID);
                            command.Parameters.AddWithValue("@statya", reader[0]);
                            reader.Close();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Статьи");
            }//Статьи* ?
            AbiturientsTableLoad(currentPlanPriemaID);
        }

        //заполнение ComboBoks для формы добавленя и редактирования
        private void AddEditFormspecialnost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormspecialnost.SelectedItem == null) return;
            try
            {
                string sql1 = $"SELECT DISTINCT ФормаОбучения.Наименование FROM ПланПриема JOIN Специальность ON(ПланПриема.IDСпециальности = Специальность.IDСпециальность) JOIN ФормаОбучения ON (ПланПриема.IDФормаОбучения = ФормаОбучения.IDФормаОбучения)  WHERE Специальность.Наименование LIKE N'{((ComboBox)sender).SelectedItem}'";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql1, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                addEditFormobushenie.Items.Clear();
                while (reader.Read())
                {
                    addEditFormobushenie.Items.Add(reader[0]);
                }
                reader.Close();
                connection.Close();
                addEditFormobushenie.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddEditFormobushenie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormobushenie.SelectedItem == null) return;
            try
            {
                string sql1 = $"SELECT DISTINCT Финансирование.Наименование FROM ПланПриема JOIN Специальность ON(ПланПриема.IDСпециальности = Специальность.IDСпециальность) JOIN ФормаОбучения ON (ПланПриема.IDФормаОбучения = ФормаОбучения.IDФормаОбучения) JOIN Финансирование ON (ПланПриема.IDФинансирования = Финансирование.IDФинансирования) WHERE Специальность.Наименование LIKE N'{addEditFormspecialnost.SelectedItem}' AND ФормаОбучения.Наименование LIKE N'{addEditFormobushenie.SelectedItem}'";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql1, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                addEditFormFinansirovanie.Items.Clear();
                while (reader.Read())
                {
                    addEditFormFinansirovanie.Items.Add(reader[0]);
                }
                reader.Close();
                connection.Close();
                addEditFormFinansirovanie.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddEditFormFinansirovanie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addEditFormFinansirovanie.SelectedItem == null) return;
            try
            {
                string sql1 = $"SELECT DISTINCT ФормаОбучения.Образование FROM ПланПриема JOIN Специальность ON(ПланПриема.IDСпециальности = Специальность.IDСпециальность) JOIN ФормаОбучения ON (ПланПриема.IDФормаОбучения = ФормаОбучения.IDФормаОбучения) JOIN Финансирование ON (ПланПриема.IDФинансирования = Финансирование.IDФинансирования) WHERE Специальность.Наименование LIKE N'{addEditFormspecialnost.SelectedItem}' AND ФормаОбучения.Наименование LIKE N'{addEditFormobushenie.SelectedItem}' AND Финансирование.Наименование LIKE N'{addEditFormFinansirovanie.SelectedItem}'";

                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql1, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                addEditFormobrazovanie.Items.Clear();
                while (reader.Read())
                {
                    addEditFormobrazovanie.Items.Add(reader[0]);
                }
                reader.Close();
                connection.Close();
                addEditFormobrazovanie.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand("Get_PlanPriemaID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@speciality", addEditFormspecialnost.SelectedItem);
                command.Parameters.AddWithValue("@formOfEducation", addEditFormobushenie.SelectedItem);
                command.Parameters.AddWithValue("@financing", addEditFormFinansirovanie.SelectedItem);
                command.Parameters.AddWithValue("@education", addEditFormobrazovanie.SelectedItem);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                PlanPriemaID = Convert.ToInt32(reader[0]);
                SetExamList();
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand($"SELECT ЦТ FROM ПланПриема WHERE IDПланПриема = {PlanPriemaID}", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                TabItemSertificat.IsEnabled = Convert.ToBoolean(reader[0]);
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MaskedTB_IsComplited(object sender, TextChangedEventArgs e)
        {
            Xceed.Wpf.Toolkit.MaskedTextBox maskedText = sender as Xceed.Wpf.Toolkit.MaskedTextBox;
            if (maskedText.IsMaskCompleted)
                maskedText.Tag = "";
            else
                maskedText.Tag = "Error";

        }
        private void TabControlAddEditForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControlAddEditForm.SelectedItem != null && e.RemovedItems.Count != 0)
            {
                if (!(e.RemovedItems[0] is TabItem tabItem)) return;
                switch (TabControlAddEditForm.Items.IndexOf(tabItem))
                {
                    case 0:
                        if (Correct_1()) ((TabItem)TabControlAddEditForm.Items[0]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[0]).Tag = "";
                        break;

                    case 1:
                        if (Correct_2()) ((TabItem)TabControlAddEditForm.Items[1]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[1]).Tag = "";
                        break;

                    case 2:
                        if (Correct_3()) ((TabItem)TabControlAddEditForm.Items[2]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[2]).Tag = "";
                        break;

                    case 3:
                        if (Correct_4()) ((TabItem)TabControlAddEditForm.Items[3]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[3]).Tag = "";
                        break;
                }
            }
        }
        #endregion
        #region Просмотр подробной информации
        private void Image_BackToAbiturients(object sender, MouseButtonEventArgs e)
        {
            GridInfo.Visibility = Visibility.Hidden;
            GridDataTable.Visibility = Visibility.Visible;
            AbiturientsTableLoad(currentPlanPriemaID);
        }
        private void MenuItem_DeleteAtestat(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить атестат?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    string sql = $"DELETE FROM Атестат WHERE IDАтестата = {((DataRowView)AtestatGrid.SelectedItem)[0]}";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception) { }
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                    
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    connection.Open();
                    dataAdapter.Fill(dataTable);
                    AtestatGrid.ItemsSource = dataTable.DefaultView;
                    connection.Close();
                    atestatCount.Text = AtestatGrid.Items.Count.ToString();

                    for (int i = 1; i < AtestatGrid.Columns.Count - 1; i++)
                    {
                        bool isNull = true;
                        for (int j = 0; j < AtestatGrid.Items.Count; j++)
                        {
                            if (((DataRowView)AtestatGrid.Items[j])[i].ToString() != "")
                            {
                                isNull = false;
                            }
                        }
                        if (isNull) AtestatGrid.Columns[i].Visibility = Visibility.Hidden;
                        else AtestatGrid.Columns[i].Visibility = Visibility.Visible;
                    }//скрытие неиспользуемых столбцов
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void MenuItem_DeleteCT(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить сертификат ЦТ?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    string sql = $"DELETE FROM СертификатЦТ WHERE IDСертификатаЦТ = {((DataRowView)SertificatiCTGrid.SelectedItem)[0]}";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                try//цт
                {
                    string sql = $"SELECT IDСертификатаЦТ, НомерСерии as num, Дисциплина, ГодПрохождения, Балл, ДесятибальноеЗначение FROM СертификатЦТ WHERE IDАбитуриента = {((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID}";
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    connection.Open();
                    dataAdapter.Fill(dataTable);
                    SertificatiCTGrid.ItemsSource = dataTable.DefaultView;
                    connection.Close();
                    sertificatCount.Text = SertificatiCTGrid.Items.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
        private void MenuItem_DeleteContact(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить контакт?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    string sql = $"DELETE FROM КонтактныеДанные WHERE IDКонтактныеДанные = {((DataRowView)kontaktnieDannieGrid.SelectedItem)[0]}";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                try//Контактные данные
                {
                    string sql = $"SELECT IDКонтактныеДанные, ROW_NUMbER() OVER(ORDER BY IDКонтактныеДанные) as Num, (SELECT Наименование FROM ТипКонтакта WHERE КонтактныеДанные.IDТипКонтакта = ТипКонтакта.IDТипКонтакта) as [ТипКонтакта], Сведения FROM  КонтактныеДанные WHERE IDАбитуриента = {((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID}";
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    connection.Open();
                    dataAdapter.Fill(dataTable);
                    kontaktnieDannieGrid.ItemsSource = dataTable.DefaultView;
                    connection.Close();
                    contactsCount.Text = kontaktnieDannieGrid.Items.Count.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        private bool EnterIsCorrect()
        {
            //проверка заполнения паспортных данных
            bool correct = (string)PassportDateVidachi.Tag != "Error";
            TextBoxIsCorrect(PassportSeriya, ref correct);
            TextBoxIsCorrect(PassportNomer, ref correct);
            TextBoxIsCorrect(PassportVidan, ref correct);
            TextBoxIsCorrect(PassportIdentNum, ref correct);
            if (correct)
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
            }
            //проверка корректности всех вкладок
            foreach (TabItem tabItem in TabControlAddEditForm.Items)
            {
                if (tabItem.Tag.ToString() != "True")
                {
                    TabControlAddEditForm.SelectedItem = tabItem;
                    return false;
                }
            }

            for (int i = 0; i < addEdifFormAtestati.Children.Count - 1; i++)
            {
                if (addEdifFormAtestati.Children[i].Visibility == Visibility.Visible && (addEdifFormAtestati.Children[i] as StackPanel) != null)
                {
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    StackPanel stackPanel = addEdifFormAtestati.Children[i] as StackPanel;
                    Grid grid = stackPanel.Children[4] as Grid;
                    for (int j = 4; j < 32; j += 2)
                    {
                        ((TextBox)grid.Children[j]).Tag = "";
                    }
                    bool error = false;
                    try
                    {
                        SqlCommand comm = new SqlCommand($"SELECT КоличествоБаллов FROM Шкала WHERE Наименование = '{((ComboBox)stackPanel.Children[8]).SelectedItem}'", connection);
                        SqlDataReader reader = comm.ExecuteReader();
                        reader.Read();
                        int count = Convert.ToInt16(reader[0]);
                        reader.Close();
                        for (int j = 4; j < 32; j += 2)
                        {
                            if ((((TextBox)grid.Children[j]).Text == "" && count * 2 + 4 > j) ||
                                (((TextBox)grid.Children[j]).Text != "" && count * 2 + 4 <= j))
                            {
                                error = true;
                                ((TextBox)grid.Children[j]).Tag = "Error";
                            }
                        }
                    }
                    catch { }

                    if (error)
                    {
                        TabControlAddEditForm.SelectedIndex = 2;
                        return false;
                    }
                }
            }
            return true;
            //проверка на корректность ввода оценок
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GridDataTable.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Visible;
            Filter.Visibility = Visibility.Visible;
        }

        private void InUpperLetter(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = ((TextBox)sender);
            if (textBox.Text.Length == 0)
            {
                textBox.Text = e.Text.ToUpper();
                textBox.SelectionStart = 1;
                e.Handled = true;
            }
        }

        private void TextBlock_Exit(object sender, MouseButtonEventArgs e)
        {
            Authorization authorization = new Authorization();
            this.Close();
            authorization.Show();
        }

        private void ScaleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem boxItem = (ComboBoxItem)e.AddedItems[0];
            int MaxMark = (int)boxItem.Tag;

            Grid grid = (Grid)((StackPanel)((ComboBox)sender).Parent).Children[4];
            for(int i = 3; i < grid.Children.Count; i += 2)
            {
                if((i-3)/2 >= MaxMark)
                {
                    TextBox textBox = ((TextBox)grid.Children[i + 1]);
                    textBox.IsEnabled = false;
                    textBox.Text = string.Empty;
                }
                else
                {
                    ((TextBox)grid.Children[i + 1]).IsEnabled = true;
                }
            }
        }

        private void TextBox_GetMarksSum(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Tag = "";
            Grid grid = (Grid)textBox.Parent;
            int MarksCount = 0;
            for (int i = 4; i < grid.Children.Count; i += 2)
            {
                if (((TextBox)grid.Children[i]).IsEnabled == false)
                    break;
                int x = 0;
                if(Int32.TryParse(((TextBox)grid.Children[i]).Text, out x))
                    MarksCount += x;
            }
            TextBlock textBlock = (TextBlock)((StackPanel)grid.Parent).Children[5];
            textBlock.Text = "Общее количество отметок: " + MarksCount;
        }

        private void PassportIdentNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).Tag = PassportIdentNum.Text.Length == 14 ? "" : "Error";
        }

        private void SetStartPosition(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            char[] arr = textBox.Text.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == '_')
                {
                    textBox.SelectionStart = i;
                    return;
                }
            }
        }

        private void ClearError(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).Tag = "";
        }
    }
} 