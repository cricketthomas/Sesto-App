using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microcharts;
using SestoApp.Interfaces;
using SestoApp.Models;
using SestoApp.ViewModels;
using SestoApp.Views.Modals;
using SkiaSharp;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace SestoApp.Views
{
    public partial class PlaceDetailPage : ContentPage
    {
        readonly PlaceDetailViewModel viewModel;
        readonly IFirebaseAuthentication auth;
        public async Task<string> FirebaseIdAsync() => (await auth.GetProfile()).FirebaseId;

        public PlaceDetailPage(PlaceDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = this.viewModel = viewModel;
            auth = DependencyService.Get<IFirebaseAuthentication>();
            ValidateBookmarkIcons();
            //BindingContext = viewModel = new PlaceDetailViewModel();
        }

        public async void ValidateBookmarkIcons()
        {
            this.ToolbarItems.Clear();

            string firebaseId = (await auth.GetProfile()).FirebaseId;
            var bookmark = await App.Database.GetBookmarkAsync(viewModel.Place.PlaceId, firebaseId);

            ToolbarItem BookmarkOutline = new() { IconImageSource = ImageSource.FromFile("bookmark.png"), Order = ToolbarItemOrder.Primary, Priority = 0 };
            ToolbarItem BookmarkFilled = new() { IconImageSource = ImageSource.FromFile("bookmarkfilled.png"), Order = ToolbarItemOrder.Primary, Priority = 0 };
            BookmarkFilled.Clicked += BookmarkFilled_Clicked;
            BookmarkOutline.Clicked += BookmarkOutline_Clicked;

            if (bookmark != null)
            {
                ToolbarItems.Remove(BookmarkOutline);
                ToolbarItems.Add(BookmarkFilled);
            }
            else
            {
                ToolbarItems.Add(BookmarkOutline);
            }
        }


        async void BookmarkFilled_Clicked(System.Object sender, System.EventArgs e)
        {
            var bookmark = await App.Database.GetBookmarkAsync(viewModel.Place.PlaceId, await FirebaseIdAsync());
            if (bookmark != null)
            {
                await App.Database.DeleteBookmarkAsync(bookmark);
                MessagingCenter.Send(this, "PopBookmarkFromCollection", bookmark);
            }


            ValidateBookmarkIcons();
        }




        async void BookmarkOutline_Clicked(System.Object sender, System.EventArgs e)
        {
            var place = new PlaceLocationResult
            {
                Name = viewModel.Place.Name,
                PlaceId = viewModel.Place.PlaceId,
                GenericPhoto = viewModel.Place.PhotoUrl,

                FormattedAddress = viewModel.Place.FormattedAddress

            };
            await App.Database.SaveBookmarkAsync(place, await FirebaseIdAsync());
            ValidateBookmarkIcons();
            var toastOptions = new ToastOptions
            {
                BackgroundColor = Color.Transparent,

                Duration = TimeSpan.FromSeconds(2),

                MessageOptions = {
                    Message = "saved!",
                    Foreground = Color.Accent,
                    //Font = Font.OfSize("FARegular", 15),

                }
            };


            await this.DisplayToastAsync(toastOptions);
        }



        async void AddCurrentActivity_Clicked(System.Object sender, System.EventArgs e)
        {
            //  await  Navigation.PushModalAsync(new NavigationPage(new ReportActivityPage()));
            //await Navigation.PushModalAsync(new ReportActivityPage());
            await Navigation.PushModalAsync(new ReportActivityPage(new ViewModels.Modals.ReportActivityViewModel(viewModel.Place)));
        }



        public PlaceDetailPage()
        {
            InitializeComponent();
            BindingContext = new PlaceDetailViewModel();
        }


        private static readonly ChartEntry[] entries = new[]
        {
            new ChartEntry(212)
            {
                Label = Enum.GetName(typeof(DayOfWeek),1).ToString(),
                ValueLabel = "112",
                Color = SKColor.Parse("#2c3e50")
            },
            new ChartEntry(248)
            {
                Label = Enum.GetName(typeof(DayOfWeek),2).ToString(),
                ValueLabel = "648",
                Color = SKColor.Parse("#77d065")
            },
            new ChartEntry(128)
            {
                Label = Enum.GetName(typeof(DayOfWeek),3).ToString(),
                ValueLabel = "428",
                Color = SKColor.Parse("#b455b6")
            },
            new ChartEntry(514)
            {
                Label = Enum.GetName(typeof(DayOfWeek),4).ToString(),
                ValueLabel = "214",
                Color = SKColor.Parse("#3498db")
            },
              new ChartEntry(514)
            {
                Label = Enum.GetName(typeof(DayOfWeek),5).ToString(),
                ValueLabel = "214",
                Color = SKColor.Parse("#3498db")
            }
        };


        public PointChart pointChart = new PointChart
        {
            Entries = entries,
            Typeface = SKTypeface.Default,
            PointSize = 40,
            BackgroundColor = SKColors.Transparent,
            IsAnimated = true,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 25

            //Margin = 50

        };

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.IntitializeData();

            chartView.Chart = pointChart;

        }


    }
}
