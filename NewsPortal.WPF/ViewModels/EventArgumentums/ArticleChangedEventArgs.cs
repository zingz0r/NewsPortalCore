using System;

namespace NewsPortal.WPF.ViewModels.EventArgumentums
{
    public class ArticleChangedEventArgs : EventArgs
    {
        public int ArticleId { get; set; }
    }
}
