using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.WPF.ViewModels.EventArgumentums
{
    class ImageEventArgs : System.EventArgs
    {
        public byte[] LargeImageData { get; set; }
        public byte[] SmallImageData { get; set; }
    }
}
