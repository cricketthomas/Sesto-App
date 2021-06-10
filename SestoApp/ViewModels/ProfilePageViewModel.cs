using System;
using System.Threading.Tasks;
using SestoApp.Models;
using SestoApp.Services;
using Xamarin.Forms;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Linq;
using SestoApp.Views;
using System.Diagnostics;
using SestoApp.Views.Settings;

namespace SestoApp.ViewModels
{
    public class ProfilePageViewModel : BaseViewModel
    {

        private readonly ILogger _logger;

        public async Task GetProfileInfo()
        {
            try
            {
                Profile = await auth.GetProfile();
            }
            catch (Exception Ex)
            {
                await Application.Current.MainPage.DisplayAlert("Uh oh..", "Something went wrong, please try again later.", "Dismiss");
                _logger.LogInformation($"{Ex}");
            }
            finally
            {
            }
        }

        private Profile profile { get; set; }
        public Profile Profile
        {
            get
            {
                return profile;
            }
            set
            {
                profile = value;
                OnPropertyChanged();
            }
        }




        public ProfilePageViewModel()
        {
            Profile = profile;
            //Title = "Profile";
        }
    }
}