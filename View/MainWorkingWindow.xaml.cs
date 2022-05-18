using PriyemnayaKomissiya.Controls;
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
using System.Windows.Media;

namespace PriyemnayaKomissiya.View
{
    public partial class MainWorkingWindow : Window
    {
        PlanPriema curentPlanPriema = null;
        private readonly int userId;
        private int planPriemaColumn = 0;
        private readonly List<Button> planPriemaButtons = new List<Button>();
        private readonly string connectionString;
        List<AbiturientDGItem> abiturients = null;
        public MainWorkingWindow(int idUser, string FIOUser)
        {
            InitializeComponent();
            userId = idUser;
            lUser_FIO.Text = FIOUser;

            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

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
            AddEditFormContacts.Tag = 0;
            addEdifFormAtestati.Tag = 0;
            addEdifFormCT.Tag = 0;
            lbPlanPriema.Content = "ПЛАН ПРИЕМА " + DateTime.Now.Year;
            var date = new StringBuilder(DateTime.Now.ToString("dddd, d MMMM"));
            date[0] = char.ToUpper(date[0]);
            lDate.Content = date.ToString();

            //Заполнение специальностей
            List<string> specialty = DB.Get_SpecialnostiName(false);
            foreach (string name in specialty) {
                TabItem tabItem = new TabItem
                {
                    Style = (Style)FindResource("TabItemStyle"),
                    Header = name
                };
                tabItem.PreviewMouseDown += new MouseButtonEventHandler(TabItem_MouseDown);
                TabControl.Items.Add(tabItem);
            }
            TabControl.SelectedItem = TabControl.Items[0];

            PlaniPriemaLoad(((TabItem)TabControl.SelectedItem).Header.ToString());
        }
        private void MainWorkingWindowForm_SizeChanged(object sender, SizeChangedEventArgs e)
        {
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
            Brush[] colors = { new SolidColorBrush(Color.FromRgb(255,87, 107)), new SolidColorBrush(Color.FromRgb(26, 149, 176)), new SolidColorBrush(Color.FromRgb(68, 166, 212)), new SolidColorBrush(Color.FromRgb(220, 136, 51)), new SolidColorBrush(Color.FromRgb(93, 79, 236)) };
            int i = 0;
            planPriemaButtons.Clear();
            grdAdmissionPlans.Children.Clear();

            List<PlanPriema> AdmissionsPlans = DB.Get_PlaniPriema(specialost, CBFinBudjet.IsChecked, CBFinHozrach.IsChecked, CBObrBaz.IsChecked, CBObrsred.IsChecked, CBFormDnev.IsChecked, CBformZaoch.IsChecked);
            foreach (PlanPriema plan in AdmissionsPlans)
            {
                Button button = new Button()
                {
                    Style = (Style)FindResource("AdmissionPlan"),
                };
                button.Click += Canvas_MouseDown;
                planPriemaButtons.Add(button);
                ButtonAdmissionPlanThemeProperties.SetFundingType(button, plan.NameForm.ToUpper());
                ButtonAdmissionPlanThemeProperties.SetStudyType(button, plan.NameFinance + ". " + plan.NameObrazovaie);
                ButtonAdmissionPlanThemeProperties.SetWritesCount(button, plan.Writes.ToString());
                ButtonAdmissionPlanThemeProperties.SetTickBrush(button, colors[i]);
                grdAdmissionPlans.Children.Add(button);
                i++;
                if (i == 4) i = 0;

                button.Tag = plan;
            }
            planPriemaColumn = 0;
            MainWorkingWindowForm_SizeChanged(null, null);
        }
        private void Canvas_MouseDown(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            curentPlanPriema = (PlanPriema)button.Tag;
            try
            {
                LabelFormaObrazovaniya.Content = ButtonAdmissionPlanThemeProperties.GetFundingType((Button)sender) + ". " + ButtonAdmissionPlanThemeProperties.GetStudyType((Button)sender);
                AbiturientsTableLoad(curentPlanPriema.Id);
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
            curentPlanPriema = DB.Get_PlanPriemaByID(PlanPriemaID);
            addEditForm.Visibility = Visibility.Hidden;
            MainGrid.Visibility = Visibility.Hidden;
            GridDataTable.Visibility = Visibility.Visible;
            Filter.Visibility = Visibility.Hidden;
            
            abiturients = DB.Get_AbiturientList(PlanPriemaID);

            dataDridAbiturients.ItemsSource = abiturients;
            GridCountWrite.Text = abiturients.Count.ToString();
        }

        private void SetExamList()
        {
            if (addEditFormobrazovanie.SelectedItem == null || EditEndButton.Visibility == Visibility.Visible) return;

            string letter;
            int num;
            string additional = "";
            try
            {
                letter = DB.Get_SpecialtyLetter((string)addEditFormspecialnost.SelectedValue);
                num = DB.Get_NextExamList(curentPlanPriema.Id);

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
        { //TODO: Очищать сертификаты цт
            PlanPriema temp = curentPlanPriema.Clone();

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

            List<string> spec = DB.Get_SpecialnostiName(true);
            foreach (string name in spec)
            {
                addEditFormspecialnost.Items.Add(name);
            }

            addEditFormspecialnost.SelectedItem = temp.NameSpec;
            addEditFormobushenie.SelectedItem = temp.NameForm;
            addEditFormFinansirovanie.SelectedItem = temp.NameFinance;
            addEditFormobrazovanie.SelectedItem = temp.NameObrazovaie;

            foreach (TabItem item in TabControlAddEditForm.Items)
                item.Tag = "";
            //контактные данные
            AddEditFormContacts.Children.RemoveRange(0, (int)AddEditFormContacts.Tag);
            AddEditFormContacts.Tag = 0;

            ContactData contact = new ContactData(Visibility.Hidden, 1);
            AddEditFormContacts.Children.Insert(0,contact);
            AddEditFormContacts.Tag = 1;
            //аттестат
            addEdifFormAtestati.Children.RemoveRange(0, (int)addEdifFormAtestati.Tag);
            addEdifFormAtestati.Tag = 0;

            Certificate certificate = new Certificate(Visibility.Hidden, 1);
            addEdifFormAtestati.Children.Insert(0, certificate);
            addEdifFormAtestati.Tag = 1;
        }
        private void AbiturientInfoShow()
        {
            if ((AbiturientDGItem)dataDridAbiturients.SelectedItem == null) return;
            GridDataTable.Visibility = Visibility.Hidden;
            GridInfo.Visibility = Visibility.Visible;
            try
            {
                AbiturientDGItem abiturient = DB.Get_AbiturientFullInfo(((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);

                InfoFIO.Text = abiturient.FIO;
                infoSchool.Text = abiturient.Shool;
                infoYear.Text = abiturient.YearOfGraduation;
                infoDate.Text = abiturient.BirthDate;
                infoLgoti.Text = ((AbiturientDGItem)dataDridAbiturients.SelectedItem).Lgoti.Replace('\n', ' ');
                if (infoLgoti.Text == "") 
                    infoLgotiTB.Visibility = Visibility.Collapsed; 
                else 
                    infoLgotiTB.Visibility = Visibility.Visible;
                infoStati.Text = ((AbiturientDGItem)dataDridAbiturients.SelectedItem).Stati.Replace('\n', ' ');
                if (infoStati.Text == "") 
                    infoStatiTB.Visibility = Visibility.Collapsed; 
                else 
                    infoStatiTB.Visibility = Visibility.Visible;
                infoDateVidoci.Text = abiturient.PassportDateIssued;
                infoSeriya.Text = abiturient.PassportSeries;
                infoPassNum.Text = abiturient.PassportNum;
                infokemvidan.Text = abiturient.PassportIssuedBy;
                infoIdentNum.Text = abiturient.PassportIdentnum;
                infoGrajdanstvo.Text = abiturient.Сitizenship;
                if (abiturient.WorkPlase == "")
                {
                    RowInfoWork.Height = new GridLength(0);
                }
                else
                {
                    infoMestoRaboti.Text = abiturient.WorkPlase;
                    infoDoljnost.Text = abiturient.Position;
                    RowInfoWork.Height = new GridLength(91);
                }
                infoVladelec.Text = abiturient.Vladelec;
                infoRedaktor.Text = abiturient.Editor;
                if (infoRedaktor.Text == "")
                    infoRedaktorTB.Visibility = Visibility.Hidden;
                else 
                    infoRedaktorTB.Visibility = Visibility.Visible;
                infoDateVvoda.Text = abiturient.Date;
                infoDateRedact.Text = abiturient.EditDate;
                if (infoDateRedact.Text == "") 
                    infoDateRedactTB.Visibility = Visibility.Hidden; 
                else 
                    infoDateRedactTB.Visibility = Visibility.Visible;
                InfoShow_Status.Text = abiturient.Status;
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
                
                List<string> spec = DB.Get_SpecialnostiName(true);
                foreach (string name in spec)
                {
                    addEditFormspecialnost.Items.Add(name);
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
                    AddEditFormContacts.Children.RemoveRange(0, (int)AddEditFormContacts.Tag);
                    AddEditFormContacts.Tag = 0;
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaKontakti", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Visibility btClose = (int)AddEditFormContacts.Tag == 0 ? Visibility.Hidden : Visibility.Visible;
                        ContactData contact = new ContactData(btClose, (int)AddEditFormContacts.Tag + 1);
                        AddEditFormContacts.Children.Insert((int)AddEditFormContacts.Tag, contact);
                        AddEditFormContacts.Tag = (int)AddEditFormContacts.Tag + 1;

                        contact.cbContactType.SelectedItem = reader.GetString(2);
                        contact.mtbData.Text = reader.GetString(3);
                        break;

                    }
                    if((int)AddEditFormContacts.Tag == 0)
                    {
                        ContactData contact = new ContactData(Visibility.Hidden, 1);
                        AddEditFormContacts.Children.Insert(0, contact);
                        AddEditFormContacts.Tag = 1;
                    }
                    connection.Close();
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message); 
                }//контактные данные
                try
                {
                    addEdifFormAtestati.Children.RemoveRange(0, (int)addEdifFormAtestati.Tag);
                    addEdifFormAtestati.Tag = 0;
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaAttestat", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Visibility btClose = (int)addEdifFormAtestati.Tag == 0 ? Visibility.Hidden : Visibility.Visible;
                        Certificate certificate = new Certificate(btClose, (int)addEdifFormAtestati.Tag + 1);
                        addEdifFormAtestati.Children.Insert((int)addEdifFormAtestati.Tag, certificate);
                        addEdifFormAtestati.Tag = (int)addEdifFormAtestati.Tag + 1;

                        certificate.tbSeries.Text = reader.GetString(reader.GetOrdinal("Num"));
                        certificate.cbScaleType.SelectedItem = reader.GetString(reader.GetOrdinal("Scale"));
                        for(int i = 0; i < certificate.Marks.Count; i++)
                        {
                            if (reader[reader.GetOrdinal("n" + (i + 1))] == DBNull.Value)
                                break;
                            certificate.Marks[i].Text = reader.GetInt32(reader.GetOrdinal("n"+(i+1))).ToString();
                        }
                    }
                    reader.Close();
                    if ((int)addEdifFormAtestati.Tag == 0)
                    {
                        Certificate certificate = new Certificate(Visibility.Hidden, 1);
                        addEdifFormAtestati.Children.Insert(0, certificate);
                        addEdifFormAtestati.Tag = 1;
                    }
                    connection.Close();
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show(ex.Message); 
                }//Аттестаты
                try
                {
                    addEdifFormCT.Children.RemoveRange(0, (int)addEdifFormCT.Tag);
                    addEdifFormCT.Tag = 0;
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand("Get_AbiturientaSertificati", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@abiturient", ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Visibility btClose = (int)addEdifFormCT.Tag == 0 ? Visibility.Hidden : Visibility.Visible;
                        CtCertificate ct = new CtCertificate((int)addEdifFormCT.Tag + 1);
                        addEdifFormCT.Children.Insert((int)addEdifFormCT.Tag, ct);
                        addEdifFormCT.Tag = (int)addEdifFormCT.Tag + 1;

                        ct.tbSeries.Text = reader.GetString(reader.GetOrdinal("num"));
                        string disciplin = reader.GetString(reader.GetOrdinal("Дисциплина"));
                        bool hasDisc = false;
                        foreach (ComboBoxItem item in ct.cbDisciplin.Items)
                        {
                            if(item.Content.ToString() == disciplin)
                            {
                                hasDisc = true;
                                ct.cbDisciplin.SelectedItem = item;
                                return;
                            }
                        }
                        if(hasDisc == false)
                        {
                            ComboBoxItem item = new ComboBoxItem()
                            {
                                Content = disciplin
                            };
                            ct.cbDisciplin.Items.Add(item);
                            ct.cbDisciplin.SelectedItem = item;
                        }
                        ct.cbDisciplin.SelectedItem = 
                        ct.mtbYear.Text = reader.GetInt32(reader.GetOrdinal("ГодПрохождения")).ToString();
                        ct.tbScore.Text = reader.GetInt32(reader.GetOrdinal("Балл")).ToString();
                        break;

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
        private void Abiturient_IssueDocuments(object sender, RoutedEventArgs e)
        {
            if ((AbiturientDGItem)dataDridAbiturients.SelectedItem == null) return;
            if (MessageBox.Show($"Отметить данную запись как отозванно?\n\n  {((AbiturientDGItem)dataDridAbiturients.SelectedItem).FIO}", "Выдать документы", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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

                    AbiturientsTableLoad(curentPlanPriema.Id);
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
                    AbiturientsTableLoad(curentPlanPriema.Id);
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

                if (MessageBox.Show($"Отметить данные записи как отозванно?\n\n {delItemsName}", "Выдать документы", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                        AbiturientsTableLoad(curentPlanPriema.Id);
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ContactData contact = new ContactData(Visibility.Visible, (int)AddEditFormContacts.Tag + 1);
            AddEditFormContacts.Children.Insert((int)AddEditFormContacts.Tag, contact);
            AddEditFormContacts.Tag = (int)AddEditFormContacts.Tag + 1;
        }//добавление нового контакта

        private void Button_NewAtestat(object sender, RoutedEventArgs e)
        {
            Certificate certificate = new Certificate(Visibility.Visible, (int)addEdifFormAtestati.Tag + 1);
            addEdifFormAtestati.Children.Insert((int)addEdifFormAtestati.Tag, certificate);
            addEdifFormAtestati.Tag = (int)addEdifFormAtestati.Tag + 1;
        }//добавление нового аттестата

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
            e.Handled = !PLib.IsTextAllowed(e.Text);
        }

        private void ButtonNewSertificatCT(object sender, RoutedEventArgs e)
        {
            CtCertificate ct = new CtCertificate((int)addEdifFormCT.Tag + 1);
            addEdifFormCT.Children.Insert((int)addEdifFormCT.Tag, ct);
            addEdifFormCT.Tag = (int)addEdifFormCT.Tag + 1;
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
            bool correct = (string)dateOfBirth.Tag != "Error" && (string)addEditFormGraduationYear.Tag != "Error"; ;
            TextBoxIsCorrect(addEditFormExamList, ref correct);
            TextBoxIsCorrect(addEditFormSurename, ref correct);
            TextBoxIsCorrect(addEditFormName, ref correct);
            TextBoxIsCorrect(addEditFormOtchestvo, ref correct);
            TextBoxIsCorrect(AddFormGrajdanstvo, ref correct);
            TextBoxIsCorrect(addEditFormShool, ref correct);
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
            if (PLib.FormIsCorrect<ContactData>(AddEditFormContacts))
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }

        }

        private void Button_NextStep_3(object sender, RoutedEventArgs e)
        {
            if (PLib.FormIsCorrect<Certificate>(addEdifFormAtestati))
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                    TabControlAddEditForm.SelectedIndex++;
                if(((TabItem)TabControlAddEditForm.SelectedItem).IsEnabled == false)
                    TabControlAddEditForm.SelectedIndex++;
            }
        }
        private void Button_NextStep_4(object sender, RoutedEventArgs e)
        {
            if (PLib.FormIsCorrect<CtCertificate>(addEdifFormCT))
            {
                ((TabItem)TabControlAddEditForm.SelectedItem).Tag = "True";
                TabControlAddEditForm.SelectedIndex++;
            }
        }

        //Добавление
        private void Button_AddEnd(object sender, RoutedEventArgs e)
        {
            if (!EnterIsCorrect()){
                return;
            }
            int AbiturientID = DB.InsertAbiturientMainData(addEditFormSurename.Text,
                addEditFormName.Text,
                addEditFormOtchestvo.Text,
                addEditFormShool.Text,
                addEditFormGraduationYear.Text,
                AddFormChekBoxGrajdanstvo.IsChecked == true,
                AddFormGrajdanstvo.Text,
                addEditFormObshejitie.IsChecked == true,
                curentPlanPriema.Id,
                textBoxWorkPlace.Text,
                textBoxDoljnost.Text,
                addEditForm_CheckBox_DetiSiroti.IsChecked == true,
                addEditForm_CheckBox_Dogovor.IsChecked == true,
                userId,
                addEditFormExamList.Text);  //Основные данные
            
            for (int i = 0; i < (int)AddEditFormContacts.Tag; i++)
            {
                if (AddEditFormContacts.Children[i] is ContactData contactData)
                {
                    DB.InsertContactData(contactData, AbiturientID);
                }
            } //Контактные данные
            
            for (int i = 0; i < (int)addEdifFormAtestati.Tag; i++)
            {
                if (addEdifFormAtestati.Children[i] is Certificate certificate)
                {
                    DB.InsertCertificate(certificate, AbiturientID);
                }
            } //Образование
            
            for (int i = 0; i < (int)addEdifFormCT.Tag; i++)
            {
                if (addEdifFormCT.Children[i] is CtCertificate ct)
                {
                    DB.InsertCtCertificate(ct, AbiturientID);
                }
            } //Сертификаты ЦТ

            DB.InsertPasportData(AbiturientID, PassportDateVidachi.Text, dateOfBirth.Text, PassportSeriya.Text, PassportNomer.Text, PassportVidan.Text, PassportIdentNum.Text); //Паспортные данные
                                                                                                                                                                                
            for (int i = 0; i < 2; i++)
            {
                StackPanel stackPanel = (StackPanel)Stati.Children[i];
                for (int j = 0; j < 3; j++)
                {
                    CheckBox checkBox = (CheckBox)stackPanel.Children[j];
                    if (checkBox.IsChecked == true)
                    {
                        DB.InsertArticles(AbiturientID, (string)checkBox.Content);
                    }
                }
            }//Статьи
            AbiturientsTableLoad(curentPlanPriema.Id);
        }

        //завершение редактирования
        private void Button_EditEnd(object sender, RoutedEventArgs e)
        {
            if (!EnterIsCorrect())
            {
                return;
            }
            int AbiturientID = ((AbiturientDGItem)dataDridAbiturients.SelectedItem).ID;
            DB.UpdateAbiturientMainData(AbiturientID,
                addEditFormSurename.Text,
                addEditFormName.Text,
                addEditFormOtchestvo.Text,
                addEditFormShool.Text,
                addEditFormGraduationYear.Text,
                AddFormChekBoxGrajdanstvo.IsChecked == true,
                AddFormGrajdanstvo.Text,
                addEditFormObshejitie.IsChecked == true,
                curentPlanPriema.Id,
                textBoxWorkPlace.Text,
                textBoxDoljnost.Text,
                addEditForm_CheckBox_DetiSiroti.IsChecked == true,
                addEditForm_CheckBox_Dogovor.IsChecked == true,
                userId,
                addEditFormExamList.Text);
            //Основные данные 

            DB.DeleteAllAbiturientDataInTable(AbiturientID, "КонтактныеДанные");
            for (int i = 0; i < (int)AddEditFormContacts.Tag; i++)
            {
                if (AddEditFormContacts.Children[i] is ContactData contactData)
                {
                    DB.InsertContactData(contactData, AbiturientID);
                }
            } //Контактные данные* ?

            DB.DeleteAllAbiturientDataInTable(AbiturientID, "Атестат");
            for (int i = 0; i < (int)addEdifFormAtestati.Tag; i++)
            {
                if (addEdifFormAtestati.Children[i] is Certificate certificate)
                {
                    DB.InsertCertificate(certificate, AbiturientID);
                }
            } //Образование* ?

            DB.DeleteAllAbiturientDataInTable(AbiturientID, "СертификатЦТ");
            for (int i = 0; i < (int)addEdifFormCT.Tag; i++)
            {
                if (addEdifFormCT.Children[i] is CtCertificate ct)
                {
                    DB.InsertCtCertificate(ct, AbiturientID);
                }
            } //Сертификаты ЦТ* ?

            DB.UpdatePasportData(AbiturientID, PassportDateVidachi.Text, dateOfBirth.Text, PassportSeriya.Text, PassportNomer.Text, PassportVidan.Text, PassportIdentNum.Text);
            //Паспортные данные*

            DB.DeleteAllAbiturientDataInTable(AbiturientID, "СтатьиАбитуриента");

            for (int i = 0; i < 2; i++)
            {
                StackPanel stackPanel = (StackPanel)Stati.Children[i];
                for (int j = 0; j < 3; j++)
                {
                    CheckBox checkBox = (CheckBox)stackPanel.Children[j];
                    if (checkBox.IsChecked == true)
                    {
                        DB.InsertArticles(AbiturientID, (string)checkBox.Content);
                    }
                }
            }//Статьи* ?
            AbiturientsTableLoad(curentPlanPriema.Id);
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
                SqlCommand command = new SqlCommand("Get_PlanPriemaID", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@speciality", addEditFormspecialnost.SelectedItem);
                command.Parameters.AddWithValue("@formOfEducation", addEditFormobushenie.SelectedItem);
                command.Parameters.AddWithValue("@financing", addEditFormFinansirovanie.SelectedItem);
                command.Parameters.AddWithValue("@education", addEditFormobrazovanie.SelectedItem);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                curentPlanPriema = DB.Get_PlanPriemaByID(reader.GetInt32(0));
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
                SqlCommand command = new SqlCommand($"SELECT ЦТ FROM ПланПриема WHERE IDПланПриема = {curentPlanPriema.Id}", connection);
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
                        if (PLib.FormIsCorrect<ContactData>(AddEditFormContacts)) ((TabItem)TabControlAddEditForm.Items[1]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[1]).Tag = "";
                        break;

                    case 2:
                        if (PLib.FormIsCorrect<Certificate>(addEdifFormAtestati)) ((TabItem)TabControlAddEditForm.Items[2]).Tag = "True";
                        else ((TabItem)TabControlAddEditForm.Items[2]).Tag = "";
                        break;

                    case 3:
                        if (PLib.FormIsCorrect<CtCertificate>(addEdifFormCT)) ((TabItem)TabControlAddEditForm.Items[3]).Tag = "True";
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
            AbiturientsTableLoad(curentPlanPriema.Id);
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
                if (tabItem.Tag.ToString() != "True" && tabItem.IsEnabled == true)
                {
                    TabControlAddEditForm.SelectedItem = tabItem;
                    return false;
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

        private void PassportIdentNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).Tag = PassportIdentNum.Text.Length == 14 ? "" : "Error";
        }

        private void SetStartPosition(object sender, TextCompositionEventArgs e)
        {
            PLib.SetStartPosition(sender);
        }

        private void ClearError(object sender, TextChangedEventArgs e)
        {
            PLib.ClearError(sender);
        }

        private void Abiturient_Delete(object sender, RoutedEventArgs e)
        {
            AbiturientDGItem abiturient = (AbiturientDGItem)dataDridAbiturients.SelectedItem;
            MessageBoxResult acceptDeletion = MessageBox.Show("Удалить выбранную запись?\n"+ abiturient.FIO, "Удаление", MessageBoxButton.YesNo);
            if (acceptDeletion == MessageBoxResult.Yes)
            {
                DB.DeleteAllAbiturientDataInTable(abiturient.ID, "Абитуриент");
                AbiturientsTableLoad(curentPlanPriema.Id);
                GridInfo.Visibility = Visibility.Hidden;
            }
        }
    }
} 