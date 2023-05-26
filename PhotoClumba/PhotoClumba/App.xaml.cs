using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MySqlConnector;

namespace PhotoClumba
{
    public partial class App : Application
    {
        public static MySqlConnection conn = new MySqlConnection("server=malyshw5.beget.tech; uid=malyshw5_test; pwd=test123!; database=malyshw5_test; Connection Timeout=20");
        public static bool IsUserLoggedIn { get; set; }
        public static string login;
        public static string password;
        public App()
        {
            InitializeComponent();
            Xamarin.Forms.Application.Current.UserAppTheme = OSAppTheme.Dark;
            var IsUserLoggedFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "IsUserLoggedIn.txt");
            string LoginState = string.Empty;
            if (IsUserLoggedFile == null || !File.Exists(IsUserLoggedFile))
            {

            }
            else
            {
                using (var reader = new StreamReader(IsUserLoggedFile, true))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        LoginState = line;
                    }
                }
            }
            if (LoginState != string.Empty)
            {
                IsUserLoggedIn = Convert.ToBoolean(LoginState);
            } else { IsUserLoggedIn = false; }
            if (!IsUserLoggedIn)
            {
                MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                MainPage = new NavigationPage(new PhotoClumba.MainPage());
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
