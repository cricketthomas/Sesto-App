using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SestoApp.Models;
using SestoApp.Services;
using Xamarin.Forms;

namespace SestoApp.ViewModels
{
    public class CreateAccountViewModel : BaseViewModel
    {

        public CreateAccountViewModel()
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

        private string confirmUserPassword;
        public string ConfirmUserPassword
        {
            get { return confirmUserPassword; }
            set
            {
                confirmUserPassword = value;
                OnPropertyChanged();
            }
        }


    }
}
