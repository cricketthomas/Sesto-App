using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text.Json;
using Xamarin.Essentials;
using SestoApp.Interfaces;
using SestoApp.ViewModels;
using Xamarin.Forms.Xaml;
using static SestoApp.Views.MainPage;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using SestoApp.Resources;

namespace SestoApp.Views.Auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInPage : ContentPage
    {

        SignInPageViewModel viewModel;
        IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();

        public SignInPage()
        {
            //On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);

            InitializeComponent();
            BindingContext = viewModel = new SignInPageViewModel();

        }


        async void SignInButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (!Validators.IsValidEmail(viewModel.UserEmailAddress))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please enter a valid email address", "OK");
                return;
            }

            try
            {
                string token = await auth.LoginWithEmailAndPassword(viewModel.UserEmailAddress, viewModel.UserPassword);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "You have successfully signed in", "OK");
                    //NavigationPage.SetHasNavigationBar(this, false);
                    //NavigationPage.SetHasBackButton(this, false);
                    var navigationPage = new Xamarin.Forms.NavigationPage(new SearchPage());
                    navigationPage.On<iOS>().SetPrefersLargeTitles(true);
                    Xamarin.Forms.Application.Current.MainPage = navigationPage;//new Xamarin.Forms.NavigationPage (new SearchPage());

                }
                else
                {
                    ShowError();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {

            }
        }



        private async void ShowError()
        {
            await DisplayAlert("Authentication Failed", "Email or password are incorrect. Try again!", "OK");
        }

  
        async void ForgotPassword_Clicked(System.Object sender, System.EventArgs e)
        {
            var email = await App.Current.MainPage.DisplayPromptAsync("Reset Password", "Enter the email you used to create your account");
            if (!string.IsNullOrWhiteSpace(email)) {
                if (!Validators.IsValidEmail(email))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Please make sure that is a valid email address", "OK");
                    return;
                }
                auth.ResetPasswordForUser(email);
                await App.Current.MainPage.DisplayAlert("Request Received!", "If an account exists with that email, you will recieve reset password instructions.", "OK");
                return;
            }
        }


        async void NavigateToSignUp_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new CreateAccountPage());
        }


    }
}
