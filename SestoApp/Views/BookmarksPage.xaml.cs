using System;
using System.Collections.Generic;
using System.Diagnostics;
using SestoApp.Infastructure;
using SestoApp.Interfaces;
using SestoApp.Models;
using SestoApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SestoApp.Views
{
    public partial class BookmarksPage : ContentPage
    {
        readonly BookmarksPageViewModel viewModel;

        public BookmarksPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new BookmarksPageViewModel();
        }



        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.IntitializeData();
        }

        async void BookmarkItem_Tapped(System.Object sender, System.EventArgs e)
        {
            var layout = (BindableObject)sender;
            var item = (Bookmark)layout.BindingContext;

            var place = new PlacesTextModel
            {
                PlaceId = item.PlaceId,
                FormattedAddress = item.FormattedAddress,
                Name = item.PlaceName,
            };
            Debug.WriteLine(item.PlaceName);

            await Navigation.PushAsync(new PlaceDetailPage(new PlaceDetailViewModel(place)));

        }
    }
}
