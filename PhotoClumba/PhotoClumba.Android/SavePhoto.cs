using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PhotoClumba;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration;
using static PhotoClumba.Droid.SavePhoto;

namespace PhotoClumba.Droid
{
    public class SavePhoto : ISavePhoto
    {
        public string SaveFile(string fileName, FileResult fileData)
        {
            string externalStoragePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string clumbaDirectory = Path.Combine(externalStoragePath, "Pictures/PhotoClumba");

            Directory.CreateDirectory(clumbaDirectory);

            string fullPath = Path.Combine(clumbaDirectory, fileName);

            //using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            //{
            //    fileData.CopyTo(fileStream);
            //    MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] {fullPath }, null, null);
            //}
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                using (System.IO.Stream stream = fileData.OpenReadAsync().Result)
                {
                    stream.CopyTo(fileStream);
                }

                MediaScannerConnection.ScanFile(Android.App.Application.Context, new string[] { fullPath }, null, null);
            }
            return fullPath;
        }
    }
}
