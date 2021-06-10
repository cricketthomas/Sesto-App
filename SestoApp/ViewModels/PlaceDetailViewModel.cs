using System;
using System.Threading.Tasks;
using SestoApp.Services;
using Xamarin.Forms;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using SestoApp.Infastructure;
using SestoApp.Models;
using Microcharts;
using SkiaSharp;

namespace SestoApp.ViewModels
{
    public class PlaceDetailViewModel : BaseViewModel
    {
        private readonly ILogger _logger;

        public PlacesTextModel Place { get; set; }
        private PlaceLocationResult placeLocation = DataService.LocationsDetails;

        public PlaceLocationResult PlaceLocation
        {
            get
            {
                return placeLocation;
            }
            set
            {
                placeLocation = value;
                OnPropertyChanged();
            }
        }

        private Bookmark bookmark { get; set; }
        public Bookmark Bookmark
        {
            get
            {
                return bookmark;
            }
            set
            {
                bookmark = value;
                OnPropertyChanged();
            }
        }



        public async Task GetFullLocationData()
        {
            try
            {
                PlaceLocation = await DataService.GetGooglePlaceById(Place.PlaceId);
            }
            catch (Exception Ex)
            {
                //await Application.Current.MainPage.DisplayAlert("Uh oh..", "Something went wrong, please try again later.", "Dismiss");
                Debug.WriteLine(Ex);
                _logger.LogInformation($"{Ex}");
            }
            finally
            {
            }
        }


        public async Task GetBookmark()
        {
            string firebaseId = (await auth.GetProfile()).FirebaseId;
            string placeId = Place.PlaceId;

            try
            {
                Bookmark = await App.Database.GetBookmarkAsync(placeId, firebaseId);
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex);
                _logger.LogInformation($"{Ex}");
            }
        }


        public async Task IntitializeData()
        {
            try
            {
                IsBusy = true;
                await GetFullLocationData();
                await GetBookmark();
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex);
                await Application.Current.MainPage.DisplayAlert("Uh oh..", "Something went wrong, please try again later.", "Dismiss");

            }
            finally
            {
                IsBusy = false;

            }
        }







        public PlaceDetailViewModel(PlacesTextModel place = null)
        {
            PlaceLocation = placeLocation;
            Title = place?.Name;
            Place = place;
            Bookmark = bookmark;
        }
    }
}