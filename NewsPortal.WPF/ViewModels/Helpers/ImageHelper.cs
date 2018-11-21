using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace NewsPortal.WPF.ViewModels.Helpers
{
    public static class ImageHelper
    {
        public static byte[] OpenAndResize(string path, int height)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path);
            image.DecodePixelHeight = height;
            image.EndInit();

            PngBitmapEncoder encoder = new PngBitmapEncoder(); 
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }
        public static bool IsValidImage(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(path);
                image.EndInit();

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));

                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    stream.ToArray();
                    Image.FromStream(stream);
                }
                
            }
            catch 
            {
                return false;
            }
            return true;
        }
    }
}
