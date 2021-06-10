using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sesto.api.Infastructure;
using sesto.api.Infastructure.Data;
using sesto.api.Services.Interfaces;

namespace sesto.api.Services.Repositories
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BookmarkRepository> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClient;
        private readonly SestoDbContext _dbContext;
        private readonly ICrunchbaseRepository _crunchbase;
        public BookmarkRepository(ILogger<BookmarkRepository> logger, IConfiguration configuration, IMemoryCache memoryCache, IHttpClientFactory httpClient,

            SestoDbContext dbContext)
        {
            _logger = logger;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _httpClient = httpClient;
            _dbContext = dbContext;

        }

        public async Task CreateBookmark(Bookmark bookmark)
        {
            if (string.IsNullOrWhiteSpace(bookmark.FirebaseId) || string.IsNullOrWhiteSpace(bookmark.PlaceId))
                throw new ApplicationException("The firebaseId and placeId cannot be null.");
            try
            {

                bool bookmarkExists = _dbContext.Bookmark.Any(b => b.PlaceId == bookmark.PlaceId && b.FirebaseId == bookmark.FirebaseId);
                if (bookmarkExists)
                    return;

                var currentUser = await _dbContext.User.SingleOrDefaultAsync(u => u.FirebaseId == bookmark.FirebaseId);
                await _dbContext.Bookmark.AddAsync(bookmark);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("bookmark error", ex);
                throw new System.Data.DataException($"Unable to create bookmark");
            }
        }

        public async Task DeleteUserBookmarks(string bookmarkId, string firebaseId)
        {
            try
            {
                var bookmark = await _dbContext.Bookmark.SingleOrDefaultAsync(b => b.Id == new Guid(bookmarkId) && b.FirebaseId == firebaseId);
                if (bookmark != null)
                {
                    _dbContext.Remove(bookmark);
                    await _dbContext.SaveChangesAsync();
                }
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError("bookmark error", ex);
                throw new System.Data.DataException($"The bookmark {bookmarkId} was not found");
            }
        }


        public async Task<ICollection<Bookmark>> GetUserBookmarks(string firebaseId)
        {
            try
            {
                var _bookmarks = await _dbContext.Bookmark.Where(u => u.FirebaseId == firebaseId).Select(b => new Bookmark
                {
                    Id = b.Id,
                    PlaceId = b.PlaceId,
                    PlaceName = b.PlaceName,
                    FormattedAddress = b.FormattedAddress,
                    FirebaseId = b.FirebaseId,
                    CreatedAt = b.CreatedAt
                }).ToArrayAsync();
                return _bookmarks;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("There was a problem with the database connection");
            }
        }

        public async Task<Bookmark> CheckBookmark(string firebaseId, string placeId)
        {
            try
            {
                var IsBookmarked = await _dbContext.Bookmark.SingleOrDefaultAsync(u => u.FirebaseId == firebaseId && u.PlaceId == placeId);
                return IsBookmarked;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("There was a problem with the database connection");
            }
        }



    }
}
