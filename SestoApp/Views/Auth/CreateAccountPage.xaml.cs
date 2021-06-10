using System;
using Xamarin.Forms;
using SestoApp.Interfaces;
using SestoApp.ViewModels;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using SestoApp.Resources;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
namespace SestoApp.Views.Auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAccountPage : ContentPage
    {

        CreateAccountViewModel viewModel;
        IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();

        public CreateAccountPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new CreateAccountViewModel();


        }


        async void SignUpButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (!Validators.IsValidEmail(viewModel.UserEmailAddress))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please enter a valid email address", "OK");
                return;
            }

            var isMatchingPassword = viewModel.ConfirmUserPassword.Equals(viewModel.UserPassword);
            if (!isMatchingPassword)
            {
                await App.Current.MainPage.DisplayAlert("Mismatching Passwords", "Your password do not match", "OK");
                return;
            }

            if (!Validators.IsValidPassword(viewModel.UserPassword))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Your password must be at least 6 characters long", "OK");
                return;
            }
               

            try
            {
                var token = await auth.CreateUserAndSignIn(viewModel.UserEmailAddress, viewModel.UserPassword);
                await App.Current.MainPage.DisplayAlert("Alert", "You have successfully signed up", "OK");
                var navigationPage = new Xamarin.Forms.NavigationPage(new SearchPage());
                navigationPage.On<iOS>().SetPrefersLargeTitles(true);
                Xamarin.Forms.Application.Current.MainPage = navigationPage;

            }

            catch (Exception Ex)
            {
                Debug.WriteLine(Ex);
                await App.Current.MainPage.DisplayAlert("Error", Ex.Message, "OK");
            }
        }



        private async void ShowError()
        {
            await DisplayAlert("Authentication Failed", "Email or password are incorrect. Try again!", "OK");
        }



        async void CancelButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }


        //protected async override void OnDisappearing()
        //{
        //    //await auth.GetProfile();
        //}

    }
}
