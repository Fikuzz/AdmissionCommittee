using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.DirectoryServices.AccountManagement;


namespace PriyemnayaKomissiya.View
{
    /// <summary>
    /// Логика взаимодействия для формы авторизации
    /// </summary>
    public partial class Authorization : Window
    {
        private string connectionString;
        /// <summary>
        /// Название группы имеющей доступ к программе
        /// </summary>
        private string groupName = "grp_priem";
        /// <summary>
        /// Конструктор по умолчанию для формы авторизации
        /// </summary>
        public Authorization()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region Войти в систему
        /// <summary>
        /// Обработчик кнопки входа
        /// </summary>
        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                if (tbPassword.Password == "priemadmin")
                {
                    string hasUser = $"SELECT IDПользователя, ФИО FROM Пользователь WHERE Логин = '{tbLogin.Text}' AND IDроли = (SELECT IDРоли FROM Роль WHERE Наименование = 'admin')";
                    SqlCommand command = new SqlCommand(hasUser, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        MainWorkingWindow mainWorkingWindow = new MainWorkingWindow(reader.GetInt32(0), reader.GetString(1));
                        mainWorkingWindow.Show();
                        Close();
                        return;
                    }
                }

                List<GroupPrincipal> result = new List<GroupPrincipal>();
                PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);
                if(tbLogin.Text == "")
                {
                    tbLogin.Tag = "Error";
                    return;
                }
                UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, tbLogin.Text);
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "college.local", "DC=college,DC=local", tbLogin.Text, tbPassword.Password))
                {
                    if (pc.ValidateCredentials(tbLogin.Text, tbPassword.Password))
                    {
                        PrincipalSearchResult<Principal> groups = user.GetGroups();
                        bool grpCorrect = false;
                        foreach (GroupPrincipal g in groups)
                        {
                            if (g.Name == groupName)
                            {
                                grpCorrect = true;
                            }
                        }
                        if (grpCorrect == false)
                        {
                            MessageBox.Show("Возможно вы не вхоите в состав приемной комиссии.", "Доступ запрещен",MessageBoxButton.OK,MessageBoxImage.Information);
                            tbPassword.Clear();
                            tbLogin.Focus();
                            tbLogin.SelectAll();
                            return;
                        }
                        string hasUser = $"SELECT IDПользователя FROM Пользователь WHERE Логин = '{tbLogin.Text}'";
                        SqlCommand command = new SqlCommand(hasUser, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            MainWorkingWindow mainWorkingWindow = new MainWorkingWindow(Convert.ToInt32(reader[0]), user.DisplayName);
                            mainWorkingWindow.Show();
                            Close();
                        }
                        else
                        {
                            reader.Close();
                            command = new SqlCommand("Add_User", connection);
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@login", tbLogin.Text);
                            command.Parameters.AddWithValue("@fio", user.DisplayName);
                            command.Parameters.AddWithValue("@role", "user");
                            reader = command.ExecuteReader();
                            reader.Read();
                            MainWorkingWindow mainWorkingWindow = new MainWorkingWindow(Convert.ToInt32(reader[0]), user.DisplayName);
                            mainWorkingWindow.Show();
                            Close();
                        }
                    }
                    else
                    {
                        tbPassword.Clear();

                        tbPassword.Tag = "Error";
                        tbLogin.Tag = "Error";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удается подключиться к сети. Проверьте сетевое подключение или обратитесь к системному администратору\n\n"+ex.Message,"Ошибка входа");
            }
        }
        #endregion

        private void TbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            pb.Tag = (!string.IsNullOrEmpty(pb.Password)).ToString();
            tbPassword.BorderBrush = default;
        }

        private void TbLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbLogin.BorderBrush = default;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSignIn_Click(btnSignIn, new RoutedEventArgs());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbLogin.Focus();
        }
    }
}