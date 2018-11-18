using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPortal.WPF.ViewModels.EventArgumentums
{
    public class ConfirmationMessageEventArgs : System.EventArgs
    {
        public string Message { get; private set; }
        public bool Cancel { get; set; }

        public ConfirmationMessageEventArgs(string message) => Message = message;
    }
}
