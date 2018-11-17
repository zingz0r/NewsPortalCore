using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.WPF.ViewModels.EventArgumentums
{
    public class ArticleChangedEventArgs : EventArgs
    {
        public int ArticleId { get; set; }
    }
}
