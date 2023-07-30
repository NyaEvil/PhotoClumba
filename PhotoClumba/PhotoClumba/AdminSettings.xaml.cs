using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhotoClumba
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminSettings : ContentPage
    {
        public AdminSettings()
        {
            InitializeComponent();
            FTPAdress.Text = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPAdress.txt"), Encoding.Default);
            FTPUser.Text = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPUser.txt"), Encoding.Default);
            FTPPassword.Text = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPPassword.txt"), Encoding.Default);

            SQLAdress.Text = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLAdress.txt"), Encoding.Default);
            SQLUser.Text = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLUser.txt"), Encoding.Default);
            SQLpwd.Text = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLpwd.txt"), Encoding.Default);
            SQLDatabase.Text = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLDatabase.txt"), Encoding.Default);
            AdminAttention();
        }

        private async void AdminAttention()
        {
            await DisplayAlert("Внимание!", "Данное окно предназначено только для работы системного администратора!", "ОК");
        }

        private async void AcceptButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPAdress.txt"), FTPAdress.Text, Encoding.Default);
                File.WriteAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPUser.txt"), FTPUser.Text, Encoding.Default);
                File.WriteAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPPassword.txt"), FTPPassword.Text, Encoding.Default);

                File.WriteAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLAdress.txt"), SQLAdress.Text, Encoding.Default);
                File.WriteAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLDatabase.txt"), SQLDatabase.Text, Encoding.Default);
                File.WriteAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLUser.txt"), SQLUser.Text, Encoding.Default);
                File.WriteAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLpwd.txt"), SQLpwd.Text, Encoding.Default);

                await DisplayAlert("Успех", "Настройки сохранены, перезапустите приложение, чтобы они вошли в силу", "ОК");

            } catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
            }
        }
    }
}