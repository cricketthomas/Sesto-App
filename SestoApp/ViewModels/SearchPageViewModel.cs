using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SestoApp.Models;
using SestoApp.Services;
using Xamarin.Forms;

namespace SestoApp.ViewModels
{
    public class SearchPageViewModel : BaseViewModel
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        int mileRadius = 1;

        public int MileRadius
        {
            set
            {
                if (mileRadius != value)
                {
                    mileRadius = value;
                    OnPropertyChanged("MileRadius");
                }
            }
            get
            {
                return mileRadius;
            }
        }


        public ICommand PerformSearch => new Command<string>(async (string query) =>
        {
            try
            {
                IsBusy = true;
                SearchResults = await DataService.GetSearchResults(query, mileRadius);
                RenderNoResultsView = true;

            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Uh oh..", "Something went wrong, please try again later.", "Dismiss");
                RenderNoResultsView = false;
            }

            finally
            {
                IsBusy = false;

            }
        });



        private bool renderNoResultsView = false;
        public bool RenderNoResultsView
        {
            get
            {
                return renderNoResultsView;
            }
            set
            {
                renderNoResultsView = value;
                OnPropertyChanged();
            }
        }


        private List<PlacesTextModel> searchResults = DataService.Locations;
        public List<PlacesTextModel> SearchResults
        {
            get
            {
                return searchResults;
            }
            set
            {
                searchResults = value;
                OnPropertyChanged();
            }
        }
    }
}
