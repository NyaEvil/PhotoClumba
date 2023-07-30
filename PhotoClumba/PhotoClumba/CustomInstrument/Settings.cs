using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;

namespace PhotoClumba.CustomInstrument
{
    public static class Settings
    {
        public static void InitSettings()
        {
            if (!File.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLAdress.txt")) && !File.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLUser.txt"))
                && !File.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLpwd.txt")) && !File.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLDatabase.txt"))
                && !File.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPAdress.txt")) 
                && !File.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPUser.txt"))
                && !File.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPPaswword.txt")))
            {
                var file = File.Create(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLDatabase.txt"));
                file.Close();
                file = File.Create(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLAdress.txt"));
                file.Close();
                file = File.Create(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLUser.txt"));
                file.Close();
                file = File.Create(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLpwd.txt"));
                file.Close();
                file = File.Create(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPAdress.txt"));
                file.Close();
                file = File.Create(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPUser.txt"));
                file.Close();
                file = File.Create(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FTPPassword.txt"));
                file.Close();
            }
        }

        public static string GetDataString()
        {
                string server = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLAdress.txt"), Encoding.Default);
                string uid = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLUser.txt"), Encoding.Default);
                string pwd = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLpwd.txt"), Encoding.Default);
                string database = File.ReadAllText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SQLDatabase.txt"), Encoding.Default);
                string result = $"server={server}; uid={uid}; pwd={pwd}; database={database}";
                return result;
        }
    }
}
