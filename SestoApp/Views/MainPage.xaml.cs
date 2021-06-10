using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using SestoApp.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SestoApp.Views.Auth;
namespace SestoApp.Views
{
    public partial class MainPage : TabbedPage
    {
        IFirebaseAuthentication auth;


        public MainPage()
        {
            auth = DependencyService.Get<IFirebaseAuthentication>();
            InitializeComponent();
        }

        async private void validateAuth()
        {
            if (!auth.IsSignedIn())
            {
                //Application.Current.MainPage = new AuthPage();
                await Navigation.PushModalAsync(new AuthPage());

            }
            else
            {
                await auth.RefreshAuthToken(true);

            }
        }


    }
}
