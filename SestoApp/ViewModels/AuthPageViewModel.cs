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
    public class AuthPageViewModel : BaseViewModel
    {

        public AuthPageViewModel()
        {
        }



        private List<Instructions> _instructions = new List<Instructions>

        {
             new Instructions()
             {
                 Title = "Tally",
                 Subtitle = "View and report on current activity, of any place.",
                 ImageSource = ImageSource.FromResource("SestoApp.Assets.Images.undraw.destination.png", typeof(ImageResourceExtension))
             },

             new Instructions()
             {
                 Title = "Search",
                 Subtitle = "Search for and view user generated activity reports, wait times, line length, place capacity restrictions, and other conditions all from one place!",
                 ImageSource = ImageSource.FromResource("SestoApp.Assets.Images.undraw.map.png", typeof(ImageResourceExtension))
             },

             new Instructions()
             {
                 Title = "Reporting",
                 Subtitle = "View and Report popular location (restaurants, stores, parks, etc.) capacity, average wait time, queue/line length and daily activity to avoid crowds and long waiting times",
                 ImageSource = ImageSource.FromResource("SestoApp.Assets.Images.undraw.right_direction.png", typeof(ImageResourceExtension))
             },
             new Instructions()
             {
                 Title = "Joining",
                 Subtitle = "Skip the sign up and start using the app now, or create an account (this can be done later too) and add activity data about whats going on or view it before you decided to go to a place!",
                 ImageSource = ImageSource.FromResource("SestoApp.Assets.Images.undraw.access_account.png", typeof(ImageResourceExtension))
             }

    };


        public List<Instructions> Instructions
        {
            get { return _instructions; }
            set
            {
                _instructions = value;
                OnPropertyChanged();
            }
        }



    }
}
