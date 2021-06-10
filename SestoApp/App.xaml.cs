using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SestoApp.Services;
using SestoApp.Views;
using SestoApp.Views.Auth;
using Xamarin.Essentials;
using SestoApp.Interfaces;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using SestoApp.Infastructure.Data;
using System.IO;

namespace SestoApp
{
    public partial class App : Xamarin.Forms.Application
    {
        readonly IFirebaseAuthentication auth;

        const int smallWidthResolution = 768;
        const int smallHeightResolution = 1334;

        const int largeWidthResolution = 828;
        const int largeHeightResolution = 1792;

        public App()
        {
            InitializeComponent();
            Sharpnado.MaterialFrame.Initializer.Initialize(loggerEnable: true, debugLogEnable: false);
            DependencyService.Register<MockDataStore>();
            auth = DependencyService.Get<IFirebaseAuthentication>();
            getDeviceSize();
            validateAuth();
        }

        private void validateAuth()
        {
            if (auth.IsSignedIn())
            {
                var navigationPage = new Xamarin.Forms.NavigationPage(new SearchPage());
                navigationPage.On<iOS>().SetPrefersLargeTitles(true);

                MainPage = navigationPage;//new NavigationPage(new SearchPage());

            }
            else
            {
                //var mainPage = new MainPage();
                //mainPage.IsVisible = false;
                //var allPages = mainPage.Children;
                //foreach (Page page in allPages)
                //{
                //    page.IsVisible = false;
                //};
                //await mainPage.Navigation.PushModalAsync(new LoginPage());
                MainPage = new Xamarin.Forms.NavigationPage(new AuthPage());

            }

        }

        private void getDeviceSize()
        {
            if (IsSmallDevice())
            {
                dictionary.MergedDictionaries.Add(SmallDevicesStyle.SharedInstance);
            }
            //else if (IsLargeDevice())
            //{
            //    dictionary.MergedDictionaries.Add(LargeDevicesStyle.SharedInstance);
            //}
            else
            {
                dictionary.MergedDictionaries.Add(GeneralDevicesStyle.SharedInstance);
            }
        }

        private static bool IsSmallDevice()
        {
            // Get Metrics
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            // Width (in pixels)
            var width = mainDisplayInfo.Width;

            // Height (in pixels)
            var height = mainDisplayInfo.Height;
            return (width <= smallWidthResolution && height <= smallHeightResolution);
        }

        private static bool IsLargeDevice()
        {
            // Get Metrics
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            // Width (in pixels)
            var width = mainDisplayInfo.Width;

            // Height (in pixels)
            var height = mainDisplayInfo.Height;
            return (width <= largeWidthResolution && height <= largeHeightResolution);
        }

        static SestoDatabase database;

        // Create the database connection as a singleton.
        public static SestoDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new SestoDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Sesto.db3"));
                }
                return database;
            }
        }


        protected override void OnStart()
        {
            //TODO add logic for token or other edge cases here.
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
