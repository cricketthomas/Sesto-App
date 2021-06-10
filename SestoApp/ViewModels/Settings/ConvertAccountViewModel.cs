using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SestoApp.Models;
using SestoApp.Services;
using SestoApp.ViewModels;
using SestoApp.Views;
using Xamarin.Forms;

namespace SestoApp.ViewModels.Settings
{
    public class ConvertAccountViewModel : BaseViewModel
    {

        public ConvertAccountViewModel()
        {

        }

        private string convertEmailAddress;
        public string ConvertEmailAddress
        {
            get { return convertEmailAddress; }
            set
            {
                convertEmailAddress = value;
                OnPropertyChanged();
            }
        }

        private string convertPassword;
        public string ConvertPassword
        {
            get { return convertPassword; }
            set
            {
                convertPassword = value;
                OnPropertyChanged();
            }
        }


    }
}
