using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;

namespace PhotoClumba
{
    public static class ImageConverter
    {
        public static async Task<byte[]> ConvertToBinary(ImageSource imageSource)
        {
            byte[] imageData = null;

            if (imageSource is FileImageSource fileImageSource)
            {
                string filePath = fileImageSource.File;
                imageData = File.ReadAllBytes(filePath);
            }
            else if (imageSource is StreamImageSource streamImageSource)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Stream imageStream = await streamImageSource.Stream(CancellationToken.None);
                    if (imageStream != null)
                    {
                        await imageStream.CopyToAsync(ms);
                        imageData = ms.ToArray();
                    }
                }
            }
            return imageData;
        }
    }
}
