using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Runtime.InteropServices.ComTypes;
using Xamarin.Forms.PlatformConfiguration;
using System.Net;
using MySqlConnector;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using FFImageLoading.Forms;

namespace PhotoClumba
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectedObjectPage : ContentPage
    {
        public Array objects;
        public string adress;
        public int photoCount = 0;
        public string lastDate;
        public static Button SendButton2 = new Button();

        public SelectedObjectPage()
        {
            InitializeComponent();
            SendButton2 = SendButton;
            DashedBorderButton dashed = new DashedBorderButton();
            dashed.buttonColor.Clicked += AddButton_Clicked;
            GridImage.Children.Add(dashed);
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = $"xamarin.{DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss")}.png"
                });
                UserLoginStore userLoginStore = new UserLoginStore();
                lastDate = DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss").ToString();
                photo.FileName = $"{lastDate}_{userLoginStore.GetLogin()}.png";
                string fileName = photo.FileName;
                ISavePhoto savePhotoService = DependencyService.Get<ISavePhoto>();
                string actualPath = savePhotoService.SaveFile(fileName, photo);
                System.IO.File.Delete(photo.FullPath);

                //Image img = new Image();
                CachedImage img = new CachedImage();
                img.BitmapOptimizations = true;
                img.DownsampleToViewSize = true;
                img.Source = ImageSource.FromFile(actualPath);
                int curcol = GridImage.ColumnDefinitions.Count - 1;

                GridImage.Children.RemoveAt(curcol);
                GridImage.Children.Add(img, curcol, 0);
                GridImage.ColumnDefinitions.Add(new ColumnDefinition());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //await DisplayAlert("Ошибка", ex.Message, "OK");
                GridImage.Children.RemoveAt(GridImage.ColumnDefinitions.Count-1);
            }
            finally
            {
                DashedBorderButton dashed = new DashedBorderButton();
                dashed.buttonColor.Clicked += AddButton_Clicked;
                int curcol = GridImage.ColumnDefinitions.Count - 1;
                GridImage.Children.Add(dashed, curcol, 0);
                photoCount++;
            }
        }

        private async void Send_Clicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Внмиание", "Загрузка начнется по нажатию кнопки ОК", "ОК", "Отмена");
            if (result)
            {
                if (!GridImage.Children.OfType<CachedImage>().Any())
                {
                    await DisplayAlert("Ошибка", "Сделайте хотя бы одно фото", "ОК");
                    return;
                }
                ActivityIndicator activityIndicator = new ActivityIndicator();
                activityIndicator.Color = Color.Yellow;
                activityIndicator.IsRunning = true;
                MainStack.Children.Remove(SendButton);
                MainStack.Children.Add(activityIndicator);
                await Task.Delay(2000);
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    try
                    {
                        UserLoginStore user = new UserLoginStore();
                        int prop = Convert.ToInt32(Прополото.IsChecked);
                        int polito = Convert.ToInt32(Полито.IsChecked);
                        int flow = Convert.ToInt32(НаличиеЦветов.IsChecked);
                        string ftpuser = System.IO.File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPUser.txt"));
                        string ftppassword = System.IO.File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPPassword.txt"));
                        FTPClient fTPClient = new FTPClient(ftpuser, ftppassword);
                        foreach (CachedImage image in GridImage.Children.OfType<CachedImage>())
                        {
                            try
                            {
                                string localPath = image.Source.ToString();
                                localPath = localPath.Substring(6);
                                string[] strings = localPath.Split(new char[] { '/' });
                                string filename = strings[strings.Length - 1];
                                SendButton.BackgroundColor = Color.Green;
                                fTPClient.UploadFile(localPath, filename);
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Ошибка", "FTP " + ex.Message, "OK");
                                MainStack.Children.Remove(activityIndicator);
                                MainStack.Children.Add(SendButton2);
                                return;
                                //string localPath = image.Source.ToString();
                                //localPath = localPath.Substring(6);
                                //string[] strings = localPath.Split(new char[] { '/' });
                                //string filename = strings[strings.Length - 1];
                                //await DisplayAlert("Ошибка", ex.Message, "ОК");
                                //fTPClient.DeleteFile(filename);
                                //break;
                            }
                        }
                        foreach (var item in objects)
                        {
                            int id = -1;
                            try
                            {
                                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                                App.conn.Open();
                            }
                            catch (Exception ex)
                            {
                                await DisplayAlert("Ошибка", "SQL "+ex.Message, "OK");
                                MainStack.Children.Remove(activityIndicator);
                                MainStack.Children.Add(SendButton2);
                                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                                return;
                            }
                            if (App.conn.Ping())
                            {
                                try
                                {
                                        string com = $"INSERT INTO reports (дата,пользователь,прополото,полито,цветы,комментарий,adress,объект) VALUES ('{lastDate}','{user.GetLogin()}','{prop}','{polito}', '{flow}', '{comment.Text}', '{adress}', '{item.ToString()}')";
                                        MySqlCommand cmd = new MySqlCommand(com, App.conn);
                                        cmd.ExecuteNonQuery();

                                        com = $"SELECT id FROM reports WHERE (дата='{lastDate}' AND пользователь = '{user.GetLogin()}') ORDER BY id DESC";
                                        cmd = new MySqlCommand(com, App.conn);
                                        var reader = cmd.ExecuteReader();
                                        if (reader.Read())
                                        {
                                            id = reader.GetInt32(0);
                                        }
                                        reader.Close();
                                        App.conn.Close();

                                        foreach (CachedImage image in GridImage.Children.OfType<CachedImage>())
                                        {
                                            App.conn.Open();
                                            string localPath = image.Source.ToString();
                                            localPath = localPath.Substring(6);
                                            string[] strings = localPath.Split(new char[] { '/' });
                                            string filename = strings[strings.Length - 1];
                                            com = $"INSERT INTO photos (reportid, photo) VALUES ('{id}','{filename}')";
                                            cmd = new MySqlCommand(com, App.conn);
                                            cmd.ExecuteNonQuery();
                                            App.conn.Close();
                                        }
                                }
                                catch (Exception ex)
                                {
                                    await DisplayAlert("Ошибка", "SQL " + ex.Message, "OK");
                                    MainStack.Children.Remove(activityIndicator);
                                    MainStack.Children.Add(SendButton2);
                                    if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                                    return;
                                }
                            }
                            else
                            {
                                await DisplayAlert("Ошибка", "Нет подключения к серверу", "ОК");
                                MainStack.Children.Remove(activityIndicator);
                                MainStack.Children.Add(SendButton2);
                                if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                                return;
                            }
                            if (App.conn.State == ConnectionState.Open) { App.conn.Close(); }

                        }

                        FtpWebResponse response = (FtpWebResponse)fTPClient.ftpresponse;
                        if (response.StatusCode == FtpStatusCode.ClosingData)
                        {
                            await DisplayAlert("Успех!", "Передача успешна", "ОК");
                            response.Dispose();
                            MainStack.Children.Remove(activityIndicator);
                            MainStack.Children.Add(SendButton2);
                            await Navigation.PopAsync();
                        }
                    }
                    catch (MySqlException ex)
                    {
                        await DisplayAlert("Ошибка", "MySQL Потеряно подключеие к серверу", "ОК");
                        MainStack.Children.Remove(activityIndicator);
                        MainStack.Children.Add(SendButton2);
                        if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                        return;
                    }
                    catch (System.AggregateException ex)
                    {
                        await DisplayAlert("Ошибка", "Строка 282" + ex.Message, "OK");
                        MainStack.Children.Remove(activityIndicator);
                        MainStack.Children.Add(SendButton2);
                        if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                        return;
                    }
                    catch (System.InvalidOperationException ex)
                    {
                        await DisplayAlert("Ошибка", "Строка 282" + ex.Message, "OK");
                        MainStack.Children.Remove(activityIndicator);
                        MainStack.Children.Add(SendButton2);
                        if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                        return;
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Ошибка", ex.Message, "OK");
                        MainStack.Children.Remove(activityIndicator);
                        MainStack.Children.Add(SendButton2);
                        if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                        return;
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Нет подключения к интернету", "ОК");
                    MainStack.Children.Remove(activityIndicator);
                    MainStack.Children.Add(SendButton2);
                    if (App.conn.State == ConnectionState.Open || App.conn.State == ConnectionState.Connecting) { App.conn.Close(); }
                    return;
                }
            }
        }
    
    }

}