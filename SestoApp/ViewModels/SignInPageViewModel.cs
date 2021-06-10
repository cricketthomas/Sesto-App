using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SestoApp.Models;
using SestoApp.Resources;
using SestoApp.Services;
using Xamarin.Forms;

namespace SestoApp.ViewModels
{
    public class SignInPageViewModel : BaseViewModel
    {

        public SignInPageViewModel()
        {
        }

        private string userEmailAddress;
        public string UserEmailAddress
        {
            get { return userEmailAddress; }
            set
            {
                userEmailAddress = value;
                OnPropertyChanged();
            }
        }

        private string userPassword;

        public string UserPassword
        {
            get { return userPassword; }
            set
            {
                userPassword = value;
                OnPropertyChanged();
            }
        }

    }
}
