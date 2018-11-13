﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NewsPortal.WPF.ViewModels.EventArgumentums;

namespace NewsPortal.WPF.ViewModels.BaseViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<MessageEventArgs> MessageApplication;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        protected void OnMessageApplication(string message)
        {
            MessageApplication?.Invoke(this, new MessageEventArgs(message));
        }
    }
}
