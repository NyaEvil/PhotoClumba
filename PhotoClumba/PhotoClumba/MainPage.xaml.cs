﻿using MySqlConnector;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Diagnostics;

namespace PhotoClumba
{
    public partial class MainPage : ContentPage
    {
        static List<string> adresses;
        public MainPage()
        {
            InitializeComponent();
            UserLoginStore userLoginStore = new UserLoginStore();
            string storedLogin = userLoginStore.GetLogin();
            try
            {
            if (App.conn.State == ConnectionState.Closed) { App.conn.Open(); }
            if (App.conn.Ping())
            {
                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM users WHERE (Login='{storedLogin}')",App.conn);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    
                }
                else
                {
                    YouveBeenDeleted();
                }
            } else
            {
                NoConnection();
            }
            var titleView = new SearchBar { HeightRequest = 44, WidthRequest = 300, };
            titleView.TextChanged += TitleView_TextChanged;
            titleView.TextColor = Color.White;
            NavigationPage.SetTitleView(this, titleView);
            App.conn.Close();
            CreateAdressList();
            }
            catch (Exception ex)
            {
                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                NoConnection();
            }
        }

        public async void YouveBeenDeleted()
        {
            await DisplayAlert("Ошибка", "Ваши данные были удалены из системы", "ОК");
            App.IsUserLoggedIn = false;
            var LoginInfo = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IsUserLoggedIn.txt");
            using (var writer = File.CreateText(LoginInfo))
            {
                await writer.WriteLineAsync(App.IsUserLoggedIn.ToString());
            }
            Process.GetCurrentProcess().Kill();
        }

        private void TitleView_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(searchText))
            {
                adressList.ItemsSource = adresses;
            }
            else
            {
                List<string> objects = new List<string>();
                foreach (var item in adressList.ItemsSource)
                {
                    objects.Add(item.ToString());
                }
                IEnumerable<string> filtered = objects.Where(item => item.ToLower().Contains(searchText.ToLower()));
                adressList.ItemsSource = filtered;
            }
        }

        private async void CreateAdressList()
        {
            App.conn.Open();
            if (Connectivity.NetworkAccess == NetworkAccess.Internet && App.conn.Ping())
            {
                try
                {
                    string com = "SELECT * FROM adress";
                    if (App.conn.State == ConnectionState.Open) { App.conn.Close(); }
                    App.conn.Open();
                    MySqlCommand cmd = new MySqlCommand(com, App.conn);
                    var reader = cmd.ExecuteReader();
                    List<string> read = new System.Collections.Generic.List<string>();
                    while (reader.Read())
                    {
                        string row = reader.GetString(1);
                        read.Add(row);
                    }
                    adressList.ItemsSource = read;
                    adresses = read;
                    App.conn.Close();
                    reader.Dispose();
                }
                catch (AggregateException ex)
                {
                    if (App.conn.State == ConnectionState.Open)
                        App.conn.Close();
                } 
                catch (MySqlException ex)
                {
                    if (App.conn.State == ConnectionState.Open)
                        App.conn.Close();
                    await DisplayAlert("Ошибка", "SQL "+ex.Message, "ОК");
                    Process.GetCurrentProcess().Kill();
                }
                catch (Exception ex)
                {
                    if (App.conn.State == ConnectionState.Open)
                        App.conn.Close();
                    await DisplayAlert("Ошибка", ex.Message, "OK");
                    Process.GetCurrentProcess().Kill();
                }
            } else
            {
                if (App.conn.State == ConnectionState.Open)
                    App.conn.Close();
                await DisplayAlert("Ошибка", "Проверьте подключение к сети или серверу", "ОК");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private void adressList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Page nextpage = new Objects(adressList.SelectedItem.ToString());
            Navigation.PushAsync(nextpage);
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Внмиание", "Вы точно хотите выйти?", "Да", "Отмена");
            try
            {
                App.conn.Open();
                UserLoginStore store = new UserLoginStore();
                MySqlCommand cmd = new MySqlCommand($"UPDATE users SET IsLogged = 0 WHERE (Login = '{store.GetLogin()}')", App.conn);
                cmd.ExecuteNonQuery();
                App.conn.Close();
                App.IsUserLoggedIn = false;
                var LoginInfo = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IsUserLoggedIn.txt");
                using (var writer = File.CreateText(LoginInfo))
                {
                    await writer.WriteLineAsync(App.IsUserLoggedIn.ToString());
                }
                LoginPage loginPage = new LoginPage();
                Navigation.InsertPageBefore(loginPage, this);
                Navigation.PopAsync();
            } catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                return;
            }
        }
        

        public async void NoConnection()
        {
            await DisplayAlert("Внимание", "Нет подключения к серверу", "ОК");
        }
    }
}
