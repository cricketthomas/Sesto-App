using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SestoApp.Infastructure;
using System;
using SestoApp.Models;

namespace SestoApp.Infastructure.Data
{
    public class SestoDatabase
    {
        readonly SQLiteAsyncConnection database;

        public SestoDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Bookmark>().Wait();
        }

        public async Task<IEnumerable<Bookmark>> GetBookmarksAsync(string firebaseId)
        {
            var bookmarks = await database.Table<Bookmark>().Where(b => b.FirebaseId == firebaseId).ToListAsync();
            return bookmarks;
        }

        public async Task<Bookmark> GetBookmarkAsync(string placeId, string firebaseId)
        {
            var bookmark = await database.Table<Bookmark>().Where(b => b.PlaceId.Equals(placeId) && b.FirebaseId.Equals(firebaseId)).FirstOrDefaultAsync();
            return bookmark;
        }

        public async Task SaveBookmarkAsync(PlaceLocationResult place, string firebaseId)
        {
            var bookmark = await database.Table<Bookmark>().Where(b => b.PlaceId.Equals(place.PlaceId) && b.FirebaseId.Equals(firebaseId)).FirstOrDefaultAsync();
            if (bookmark == null)
            {
                await database.InsertAsync(new Bookmark
                {
                    PlaceId = place.PlaceId,
                    PlaceName = place.Name,
                    FormattedAddress = place.FormattedAddress,
                    //PhotoUrl = place.p,
                    FirebaseId = firebaseId,
                    CreatedAt = DateTime.Now
                });
            }
            return;
        }

        // TODO: maybe you can use await???
        public async Task<int> DeleteBookmarkAsync(Bookmark bookmark)
        {
            return await database.DeleteAsync(bookmark);
        }


    }
}