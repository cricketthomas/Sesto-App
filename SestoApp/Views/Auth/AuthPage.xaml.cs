using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text.Json;
using Xamarin.Essentials;
using SestoApp.Interfaces;
using SestoApp.Resources;
using SestoApp.ViewModels;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SestoApp.Views.Auth
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthPage : ContentPage
    {

        AuthPageViewModel viewModel;
        //IFirebaseAuthentication auth;
        IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();

        public AuthPage()
        {
            InitializeComponent();
            //auth = DependencyService.Get<IFirebaseAuthentication>();
            BindingContext = viewModel = new AuthPageViewModel();

        }

     


        async void NavigateToSignIn_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SignInPage());
        }



        async void LoginAnonymously_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                string token = await auth.LoginAnonymously();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "You have successfully joined anonymously", "OK");
                    var navigationPage = new Xamarin.Forms.NavigationPage(new SearchPage());
                    navigationPage.On<iOS>().SetPrefersLargeTitles(true);
                    Xamarin.Forms.Application.Current.MainPage = navigationPage;
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


        async void NavigateToSignUp_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new CreateAccountPage());
        }


        private async void ShowError()
        {
            await DisplayAlert("Authentication Failed", "Email or password are incorrect. Try again!", "OK");
        }

        async void TermsDisclaimer_Tapped(System.Object sender, System.EventArgs e)
        {
            var browser = new WebView
            {
                Source = "https://cricketthomas.github.io/"
            };
            var uri = new Uri("https://cricketthomas.github.io/");

            await Browser.OpenAsync(uri, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                Flags  = BrowserLaunchFlags.PresentAsFormSheet
            });
            //TODO: make this open in the app or in a modal.
            //  await Launcher.
            // await Navigation.PushModalAsync(await Launcher.OpenAsync(uri));
        }



    }
}
