using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sesto.api.Infastructure.Data;

namespace sesto.api.Services.Interfaces
{
    public interface IBookmarkRepository
    {
        Task<ICollection<Bookmark>> GetUserBookmarks(string firebaseId);
        Task DeleteUserBookmarks(string bookmarkId, string firebaseId);
        Task CreateBookmark(Bookmark bookmark);
        Task<Bookmark> CheckBookmark(string firebaseId, string placeId);


    }
}
