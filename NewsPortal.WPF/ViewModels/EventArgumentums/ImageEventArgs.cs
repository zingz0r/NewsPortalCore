using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPortal.Data.DTO;
using NewsPortal.Data.Entity;

namespace NewsPortal.WPF.ViewModels.EventArgumentums
{
    class ImageEventArgs : System.EventArgs
    {
        public List<PictureDTO> Pictures { get; set; }

        public ImageEventArgs()
        {
            Pictures = new List<PictureDTO>();
        }
    }
}
