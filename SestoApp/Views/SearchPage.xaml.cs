using System;
using SestoApp.ViewModels;
using Xamarin.Forms;
using SestoApp.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Linq;
using System.Collections.Generic;

namespace SestoApp.Views
{
    public partial class SearchPage : ContentPage
    {

        SearchPageViewModel viewModel;

        public SearchPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new SearchPageViewModel();
        }
        private void PackagesScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var transY = Convert.ToInt32(SearchBarView.TranslationY);
            if (transY == 0 &&
                e.VerticalDelta > 15)
            {
                var trans = SearchBarView.Height + SearchBarView.Margin.Top;
                var safeInsets = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                //Debug.WriteLine($"up {SearchBarView.Margin.Bottom}");
                //SearchBarView.Margin = new Thickness(0, 20, 15, 5);
                // Start both animations concurrently
                Task.WhenAll(
                    SearchBarView.TranslateTo(0, -(trans + safeInsets.Top), 200, Easing.CubicIn),
                    SearchBarView.FadeTo(0.25, 200));
            }
            else if (transY != 0 &&
                     e.VerticalDelta < 0 &&
                     Math.Abs(e.VerticalDelta) > 10)
            {

                //Debug.WriteLine($"down {SearchBarView.Margin.Bottom}");

                Task.WhenAll(
                    SearchBarView.TranslateTo(0, 0, 200, Easing.CubicOut),
                    SearchBarView.FadeTo(1, 200));
            }
        }



        async void OnItemSelected(object sender, EventArgs args)
        {
            var layout = (BindableObject)sender;
            var item = (PlacesTextModel)layout.BindingContext;
            Debug.WriteLine(item.Name);

            await Navigation.PushAsync(new PlaceDetailPage(new PlaceDetailViewModel(item)));
        }

        //async void AddItem_Clicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushModalAsync(new NewItemPage());
        //}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //    if (viewModel.SearchResults.Count == 0)
            //        viewModel.IsBusy = true;
        }

        void BookmarksIcon_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new BookmarksPage());
        }

        void ProfileIcon_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ProfilePage());

        }

    }
}