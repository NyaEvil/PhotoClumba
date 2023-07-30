using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PhotoClumba
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Objects : ContentPage
	{
        public CollectionView objectsList = new CollectionView();
        public Objects (string title)
		{
            InitializeComponent();
            objectsList.BackgroundColor = Color.FromHex("#424242");
            Title = title;
            CreateObjectList();
		}

        protected override void OnAppearing()
        {
            objectsList.SelectedItems.Clear();
        }

        private async void CreateObjectList()
        {
            App.conn.Open();
            if (App.conn.Ping())
            {
                App.conn.Close();
                try
                {
                    string com = $"SELECT objects FROM objects WHERE (adress ='{Title}')";
                    App.conn.Open();
                    MySqlCommand cmd = new MySqlCommand(com, App.conn);
                    var reader = cmd.ExecuteReader();
                    List<string> read = new System.Collections.Generic.List<string>();
                    while (reader.Read())
                    {
                        string row = reader.GetString(0);
                        read.Add(row);
                    }

                    objectsList.SelectionMode = SelectionMode.Multiple;
                    objectsList.ItemsSource = read;
                    objectsList.ItemTemplate = new DataTemplate(() =>
                    {
                        var label = new Label();
                        label.VerticalOptions = LayoutOptions.Center;
                        label.HorizontalOptions = LayoutOptions.FillAndExpand;
                        label.Margin = new Thickness(0, 0, 0, 10);
                        label.Padding = 10;
                        label.FontSize = 25;
                        label.HeightRequest = 45;
                        label.TextColor = Color.LightGray;
                        label.SetBinding(Label.TextProperty, ".");

                        return label;
                    });
                    ObjectStack.Children.Add(objectsList);

                    Button button = new Button();
                    button.Text = "Выбрать";
                    button.Clicked += Button_Clicked;
                    ObjectStack.Children.Add(button);

                    App.conn.Close();
                    reader.Dispose();
                }
                catch (AggregateException ex)
                {
                    App.conn.Close();
                    await DisplayAlert("Ошибка", ex.Message, "OK");
                    return;
                }
                catch (MySqlException ex)
                {
                    await DisplayAlert("Ошибка", "Потеряно соединение с сервером", "ОК");
                    Process.GetCurrentProcess().Kill();
                }
            } else
            {
                await DisplayAlert("Ошибка", "Потеряно соединение с сервером", "ОК");
                Process.GetCurrentProcess().Kill();
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            UserLoginStore userLoginStore = new UserLoginStore();
            App.conn.Open();
            MySqlCommand cmd = new MySqlCommand($"SELECT IsAdmin FROM users WHERE Login='{userLoginStore.GetLogin()}'", App.conn);
            var reader = cmd.ExecuteReader();
            reader.Read();
            bool IsAdmin = reader.GetBoolean(0);
            reader.Close();
            App.conn.Close();
            if (objectsList.SelectedItems.Count!=0)
            {
                if (!IsAdmin)
                {
                    SelectedObjectPage objectPage = new SelectedObjectPage();
                    objectPage.objects = objectsList.SelectedItems.ToArray();
                    objectPage.adress = Title;
                    Navigation.PushAsync(objectPage);
                } else
                {
                    ManualReport manualReport = new ManualReport();
                    manualReport.objects = objectsList.SelectedItems.ToArray();
                    manualReport.adress = Title;
                    Navigation.PushAsync(manualReport);
                }
            } else
            {
                DisplayAlert("Сообщение", "Выберите хотя бы один объект", "ОК");
            }
        }

    }
}