using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NewsPortal.WPF.Resources
{
    public abstract class ResourceManager
    {
        public static object GetPngImage(Bitmap imageResource)
        {
            var bitmapImage = new BitmapImage();
            try
            {
                using (var memory = new MemoryStream())
                {
                    imageResource.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }

            return bitmapImage;
        }
    }
}
