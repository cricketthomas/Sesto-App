using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SestoApp.Interfaces;
using SestoApp.ViewModels;
using SestoApp.Views.Settings;
using SestoApp.Views.Auth;

using Xamarin.Forms;
using System.Windows.Input;
using System.Diagnostics;

namespace SestoApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        readonly IFirebaseAuthentication auth;
        readonly ProfilePageViewModel viewModel;

        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ProfilePageViewModel();
            auth = DependencyService.Get<IFirebaseAuthentication>();
            // start with a blank settings table to render properly for different providers of users. 
            SettingsTable.Clear();
            ReRenderProfile();
            MessagingCenter.Subscribe<ConvertAccountPage>(this, "RefreshProfilePage", async (sender) =>
            {
                Debug.WriteLine("RefreshProfilePage");
                ReRenderProfile();
            });

        }



        async void LogoutIcon_Clicked(System.Object sender, System.EventArgs e)
        {
            if (auth.IsSignedIn())
            {
                bool answer = await DisplayAlert("Are you sure you want to logout?", "Anonymous users can't recover their account once logged out", "Continue", "Cancel");
                if (answer)
                {
                    await auth.SignOut();
                    //await Navigation.PopToRootAsync(); //works
                    await Navigation.PushModalAsync(new NavigationPage(new AuthPage()));

                }
            }
        }


        async private void ReRenderProfile()
        {
            try
            {
                viewModel.IsBusy = true;

                await viewModel.GetProfileInfo();

                SettingsTable.Clear();

                if (viewModel.Profile.IsAnonymous)
                    SettingsTable.Add(AnonAccountSettings);

                if (!viewModel.Profile.IsAnonymous)
                    SettingsTable.Add(AccountSettings);
            }
            finally
            {
                viewModel.IsBusy = false;
            }

        }


        async void UpdateUserName_Tapped(System.Object sender, System.EventArgs e)
        {
            var initialDisplayName = viewModel.Profile.DisplayName;
            string result = await DisplayPromptAsync("Display Name", "Enter a name to be shown on your profile:", initialValue: initialDisplayName, maxLength: 200);
            if (result != null && !result.Equals(initialDisplayName))
            {

                await auth.UpdateFirebaseDisplayName(string.IsNullOrWhiteSpace(result) ? null : result);
                await viewModel.GetProfileInfo();
            }

        }

        async void ResetPassword_Tapped(System.Object sender, System.EventArgs e)
        {
            var sendAlert = await DisplayAlert("Request Reset Email", "An email will be sent to the address associated with this account", "Confirm", "Cancel");
            if (sendAlert)
            {
                try
                {
                    auth.ResetPasswordForUser();
                    await DisplayAlert("Email Sent!", "You should receive an email shortly with password reset instructions.", "Dismiss");
                }
                catch (Exception Ex)
                {
                    await DisplayAlert("Uh oh!", "Something went wrong.", "Dismiss");

                }
            }

        }

        async void ConvertAccount_Tapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ConvertAccountPage());
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

    }
}
