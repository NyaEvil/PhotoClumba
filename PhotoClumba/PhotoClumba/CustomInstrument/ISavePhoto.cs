using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Essentials;

namespace PhotoClumba
{
    public interface ISavePhoto
    {
        string SaveFile(string fileName, FileResult fileData);
    }
}
