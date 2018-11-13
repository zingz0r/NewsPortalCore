namespace NewsPortal.WPF.ViewModels.EventArgumentums
{
    public class MessageEventArgs : System.EventArgs
    {
        public string Message { get; private set; }

        public MessageEventArgs(string message) => Message = message;
    }
}
