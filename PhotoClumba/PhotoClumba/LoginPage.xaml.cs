using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Data;

namespace PhotoClumba
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void AuthButton(object sender, EventArgs e)
        { try
            {
                App.conn.Open();
                if (Connectivity.NetworkAccess == NetworkAccess.Internet && App.conn.Ping())
                {
                    AuthBut.IsEnabled = false;
                    string com = $"SELECT IsLogged FROM users WHERE (Login='{AuthLogin.Text}' AND Password='{AuthPassword.Text}')";
                    MySqlCommand cmd = new MySqlCommand(com, App.conn);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        bool IsLogged = reader.GetBoolean(0);
                        if (IsLogged)
                        {
                            await DisplayAlert("Неудача", "Такой пользователь уже в сети", "OK");
                            App.conn.Close();
                        }
                        else
                        {
                            if (App.conn.State == System.Data.ConnectionState.Open) { App.conn.Close(); }
                            App.conn.Open();
                            await DisplayAlert("Успех", "Вы успешно авторизированы", "ОК");
                            App.IsUserLoggedIn = true;
                            var LoginInfo = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IsUserLoggedIn.txt");
                            using (var writer = File.CreateText(LoginInfo))
                            {
                                await writer.WriteLineAsync(App.IsUserLoggedIn.ToString());
                            }
                            var storedLogin = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "StoredLogin.txt");
                            using (var writer = File.CreateText(storedLogin))
                            {
                                await writer.WriteLineAsync(AuthLogin.Text);
                            }
                            UserLoginStore userLoginStore = new UserLoginStore();
                            com = $"UPDATE users SET IsLogged = 1 WHERE (Login='{userLoginStore.GetLogin()}')";
                            cmd = new MySqlCommand(com, App.conn); 
                            cmd.ExecuteNonQuery();
                            com = $"UPDATE users SET version = '{VersionTracking.CurrentVersion}' WHERE (Login = '{userLoginStore.GetLogin()}')";
                            cmd = new MySqlCommand(com, App.conn);
                            cmd.ExecuteNonQuery();
                            App.conn.Close();
                            Navigation.InsertPageBefore(new MainPage(), this);
                            await Navigation.PopAsync();
                        }
                    }
                    else
                    {
                        await DisplayAlert("Неудача", "Неверный логин или пароль", "ОК");
                        if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка подключения", "Нет подключения к интернету или серверу", "ОК");
                    if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                    AuthLogin.Text = null;
                    AuthPassword.Text = null;
                }
            }
            catch (AggregateException)
            {
                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                return;
            }
            catch (MySqlException ex)
            {
                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                await DisplayAlert("Ошибка", "Проверьте подключение к серверу" + ex.Message, "ОК");
            }
            catch (System.NullReferenceException)
            {
                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                await DisplayAlert("Ошибка", "Обратитесь к системному администратору за настройкой", "ОК");
            }
            catch (Exception ex)
            {
                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                AuthBut.IsEnabled = true;
            }
        }

        private void Logo_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AdminSettings());
        }
    }
}