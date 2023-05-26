using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PhotoClumba
{
    public class UserLoginStore
    {
        public string GetLogin()
        {
            var storedLoginPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "StoredLogin.txt");
            string storedLogin = string.Empty;
            if (storedLoginPath == null || !File.Exists(storedLoginPath))
            {

            }
            else
            {
                using (var reader = new StreamReader(storedLoginPath, true))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        storedLogin = line;
                    }
                }
            }
            return storedLogin;
        }

    }
}
