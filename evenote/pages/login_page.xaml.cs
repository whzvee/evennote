﻿using System;
using System.Collections.Generic;
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
using DataBaseAPI;
using System.Text.RegularExpressions;
using System.IO;

namespace evenote.pages
{
    /// <summary>
    /// Interaction logic for login_page.xaml
    /// </summary>
    public partial class login_page : Page
    {

        public login_page()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (Evennote.OfflineMode)
            {
                if (!Directory.Exists(String.Format("C:\\Users\\{0}\\Documents\\evennote\\{1}\\", Environment.UserName, login.Text))) return;
                Evennote.SetUserDirectory(login.Text);

                //Считываем с диска существующие заметки
                Notebook.LoadNotes();

                (Application.Current.MainWindow as MainWindow).ChangePage("pages/menu_page.xaml");
                return;
            }
            string pass = password.Password;
            try
            {
                //Если авторизация успешна то..
                if (Evennote.Authorization(login.Text, password.Password))
                {
                    //Пересылаем окно на страницу меню приложения
                    (Application.Current.MainWindow as MainWindow).ChangePage("pages/menu_page.xaml");

                    //Создаем пользовательскую директорию
                    Evennote.SetUserDirectory(login.Text);

                    //Считываем с диска существующие заметки
                    Notebook.LoadNotes();

                    Evennote.user.online = true;

                    //Сохраняем логин пароль для автовхода
                    if (checkBox.IsChecked.Value)
                        Evennote.WriteConfigFile(login.Text + " " + pass, "3v3nn0t3");
                }
                else
                {
                    MessageBox.Show("Login or password incorrect.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void signUp_button_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).ChangePage("pages/regist_page.xaml");
        }

        //Регулярное выражение на пароль и логин
        private void login_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^A-Za-z0-9_]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        //Проверяем поставленные чекбоксы (если remember me == true то авторизируем пользователя)
        private void Page_Initialized(object sender, EventArgs e)
        {
            if (!File.Exists(Evennote.ConfigFile))
            {
                if (Evennote.AutoLogin)
                    Evennote.SetConfigurateFile();
                return;
            }
            else
            {
                checkBox.IsChecked = true;
                string config = Evennote.ReadConfigFile("3v3nn0t3");
                if (config.Equals("")) return;
                try
                {
                    if (Evennote.Authorization(config.Split(' ').First(), config.Split(' ').Last()))
                    {
                        (Application.Current.MainWindow as MainWindow).ChangePage("pages/menu_page.xaml");

                        Evennote.SetUserDirectory(config.Split(' ').First());

                        Evennote.user.online = true;
                        //Считываем с диска существующие заметки
                        Notebook.LoadNotes();
                    }
                    else
                    {
                        MessageBox.Show("Login or password incorrect.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Evennote.AutoLogin = false;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            Evennote.AutoLogin = true;
        }

        private void checkBoxOffline_Unchecked(object sender, RoutedEventArgs e)
        {
            Evennote.OfflineMode = false;
            signUp_button.IsEnabled = true;
        }

        private void checkBoxOffline_Checked(object sender, RoutedEventArgs e)
        {
            Evennote.OfflineMode = true;
            signUp_button.IsEnabled = false;
        }


        //Просто методы для удобства интерфейса 
        private void login_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "login")
                (sender as TextBox).Text = "";
        }

        private void login_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
                (sender as TextBox).Text = "login";
        }

        private void password_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password == "")
                (sender as PasswordBox).Password = "password";
        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordBox).Password == "password")
                (sender as PasswordBox).Password = "";
        }
    }
}