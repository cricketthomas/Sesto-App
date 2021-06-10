using System;
using Xamarin.Forms;
using SestoApp.Interfaces;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using SestoApp.ViewModels.Settings;
using SestoApp.Resources;
using SestoApp.Views;
using SestoApp.ViewModels;

namespace SestoApp.Views.Settings
{
    public partial class ConvertAccountPage : ContentPage
    {

        ConvertAccountViewModel viewModel;
        IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();

        public ConvertAccountPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ConvertAccountViewModel();

        }


        async void ConvertAccount_Clicked(System.Object sender, System.EventArgs e)
        {
            if (!Validators.IsValidEmail(viewModel.ConvertEmailAddress))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please enter a valid email address", "OK");
                return;
            }
            try
            {
                await auth.ConvertToEmailAndPasswordAccount(viewModel.ConvertEmailAddress, viewModel.ConvertPassword);
                MessagingCenter.Send(this, "RefreshProfilePage");

                await Navigation.PopAsync();

            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex.Message);
                ShowError();
            }

        }

        private async void ShowError()
        {
            await DisplayAlert("Authentication Failed", "Email or password are incorrect. Try again!", "OK");
        }


        async void CancelButton_Clicked(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Send(this, "RefreshProfilePage");

            await Navigation.PopAsync();
            // MessagingCenter.Send(this, "RefreshProfilePage");

        }


        //protected async override void OnDisappearing()
        //{
        //    //await auth.GetProfile();
        //}


    }
}
