using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using SestoApp.Infastructure;
using SestoApp.Models;
using SestoApp.Resources;
using SestoApp.Services;
using SestoApp.Views;
using Xamarin.Forms;

namespace SestoApp.ViewModels
{
    public class BookmarksPageViewModel : BaseViewModel
    {


        public BookmarksPageViewModel()
        {
            Title = "Bookmarks";
            Bookmarks = bookmarks;

            MessagingCenter.Subscribe<PlaceDetailPage, Bookmark>(this, "PopBookmarkFromCollection", async (obj, bookmark) =>
            {
                var _bookmark = bookmark as Bookmark;
                PopBookmark(_bookmark);
            });
        }

        private readonly ILogger _logger;

        private ObservableCollection<Bookmark> bookmarks = new ObservableCollection<Bookmark>();
        //private ObservableCollection<Bookmark> bookmarks { get; set; }
        public ObservableCollection<Bookmark> Bookmarks
        {
            get
            {
                return bookmarks;
            }
            set
            {
                bookmarks = value;
                OnPropertyChanged();
            }
        }


        public class BookmarkComparer : IEqualityComparer<Bookmark>
        {
            public bool Equals(Bookmark x, Bookmark y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;
                return x.PlaceId == y.PlaceId && x.Id == y.Id;
            }
            public int GetHashCode(Bookmark bookmark)
            {
                if (Object.ReferenceEquals(bookmark, null)) return 0;
                int hashProductName = bookmark.PlaceName == null ? 0 : bookmark.PlaceName.GetHashCode();
                int hashProductCode = bookmark.PlaceName.GetHashCode();
                return hashProductName ^ hashProductCode;
            }
        }


        public async Task GetAllBookmarks()
        {
            string firebaseId = (await auth.GetProfile()).FirebaseId;

            try
            {
                var _bookmarks = await App.Database.GetBookmarksAsync(firebaseId);
                //var intersect = bookmarks.Intersect(_bookmarks, new BookmarkComparer()).ToArray();
                var allBookmarks = bookmarks.Concat(_bookmarks).ToArray();

                if (_bookmarks != null)
                {
                    foreach (var bookmark in allBookmarks)
                    {
                        var inCollection = bookmarks.Contains<Bookmark>(bookmark, new BookmarkComparer());
                        if (!inCollection)
                            bookmarks.Add(bookmark);

                        var notInDb = !_bookmarks.Contains<Bookmark>(bookmark, new BookmarkComparer());
                        Debug.WriteIf(notInDb, bookmark.PlaceName);
                        if (notInDb && bookmarks.Contains<Bookmark>(bookmark, new BookmarkComparer())) bookmarks.Remove(bookmark);

                    }
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex);
                _logger.LogInformation($"{Ex}");
            }
        }

        public void PopBookmark(Bookmark bookmark)
        {
            var inCollection = bookmarks.Contains<Bookmark>(bookmark, new BookmarkComparer());
            if (inCollection)
                bookmarks.Remove(bookmark);
        }


        public async Task IntitializeData()
        {
            try
            {
                IsBusy = true;
                await GetAllBookmarks();
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
    }
}
