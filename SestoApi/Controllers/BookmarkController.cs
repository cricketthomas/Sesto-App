using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sesto.api.Attributes;
using sesto.api.Exceptions;
using sesto.api.Infastructure.Data;
using sesto.api.Models;
using sesto.api.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sesto.api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    public class BookmarkController : Controller
    {


        private readonly IBookmarkRepository _bookmarksRepository;
        private readonly ILogger<BookmarkController> _logger;
        private readonly IGenericPictrueRepository _genericPictrueRepository;
        public BookmarkController(IBookmarkRepository bookmarksRepository, ILogger<BookmarkController> logger, IGenericPictrueRepository genericPictrueRepository)
        {
            _bookmarksRepository = bookmarksRepository;
            _genericPictrueRepository = genericPictrueRepository;
            _logger = logger;
        }


        // GET bookmarked places by HTTP claims
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Bookmark>>> Get()
        {
            var firebaseId = HttpContext.User.Claims.ToArray().FirstOrDefault(f => f.Type.Equals("firebaseId")).Value;
            if (firebaseId == null)
                return BadRequest("FirebaseId attribute not found in HTTP claims.");
            try
            {
                var bookmarks = await _bookmarksRepository.GetUserBookmarks(firebaseId);
                return Ok(bookmarks);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve bookmarks", ex);
                throw new ApplicationException("There was a problem accessing bookmarks");
            }

        }

        /// <summary>
        /// delete a bookmark via bookmarkId and firebaseId from httpclaims
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        // DELETE api/values/5
        [HttpDelete("{bookmarkId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(string bookmarkId)
        {
            var firebaseId = HttpContext.User.Claims.ToArray().FirstOrDefault(f => f.Type.Equals("firebaseId")).Value;
            if (firebaseId == null)
                return BadRequest("FirebaseId attribute not found in HTTP claims.");
            try
            {
                await _bookmarksRepository.DeleteUserBookmarks(bookmarkId: bookmarkId, firebaseId: firebaseId);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Create([FromBody] BookmarkModel bookmark)
        {
            var firebaseId = HttpContext.User.Claims.ToArray().FirstOrDefault(f => f.Type.Equals("firebaseId")).Value;
            if (firebaseId == null)
                return BadRequest("FirebaseId attribute not found in HTTP claims.");
            try
            {
                var _bookmark = new Bookmark
                {
                    PlaceId = bookmark.PlaceId,
                    PlaceName = bookmark.PlaceName,
                    FormattedAddress = bookmark.FormattedAddress,
                    FirebaseId = firebaseId
                };

                await _bookmarksRepository.CreateBookmark(_bookmark);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("The bookmark was unable to be created", ex);
                throw;
            }


        }




        [HttpGet("validate/{placeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ICollection<Bookmark>>> ValidateBookmark(string placeId)
        {
            var firebaseId = HttpContext.User.Claims.ToArray().FirstOrDefault(f => f.Type.Equals("firebaseId")).Value;
            if (firebaseId == null)
                return BadRequest("FirebaseId attribute not found in HTTP claims.");
            try
            {
                var bookmarks = await _bookmarksRepository.CheckBookmark(firebaseId: firebaseId, placeId: placeId);
                if (bookmarks == null)
                    return NotFound();
                return Ok(bookmarks);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to retrieve bookmarks", ex);
                throw new ApplicationException("There was a problem accessing bookmarks");
            }

        }

        [HttpGet("test")]
        //[RequestRateLimit(Name = "Test Rate limit", Seconds = 30)]
        public dynamic Test(string placeName, string query)
        {
            var x = _genericPictrueRepository.GetEmoji(placeName, query);
            return x;
        }

    }
}
