using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    //  [Authorize]
    [Route("api/[controller]")]
    public class PlacesController : Controller
    {


        private readonly IGooglePlaceRepository _googlePlaceRepository;
        private readonly IPlaceActivityDataRepository _placeActivityDataRepository;
        private readonly ILogger<PlacesController> _logger;

        public PlacesController(IGooglePlaceRepository googlePlaceRepository,
            IPlaceActivityDataRepository placeActivityDataRepository, ILogger<PlacesController> logger)
        {
            _logger = logger;
            _googlePlaceRepository = googlePlaceRepository;
            _placeActivityDataRepository = placeActivityDataRepository;
        }


        /// <summary>
        /// Search locations by name, and lat, long and mile radius only if lat & long are present.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="mileRadius"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [RequestRateLimit(Name = "GooglePlaces", Seconds = 1)]
        public async Task<ActionResult<IEnumerable<PlacesTextModel>>> SearchLocations(string query, double latitude = 0, double longitude = 0, int mileRadius = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest();
            try
            {
                var searchResults = await _googlePlaceRepository.TextSearchPlacesAsync(query, latitude, longitude, mileRadius);
                if (searchResults == null)
                    return NotFound();

                return Ok(searchResults);
            }
            catch (Exception Ex)
            {
                _logger.LogError($"Error fetching google places: \n {Ex}");
                throw;
            }


        }

        /// <summary>
        /// Get place by google placeId for additional details
        /// </summary>
        /// <param name="placeId"></param>
        /// <returns></returns>
        [HttpGet("details/{placeId}")]
        public async Task<ActionResult<PlaceLocationResult>> GetPlaceById(string placeId)
        {
            if (string.IsNullOrWhiteSpace(placeId))
                return BadRequest();

            try
            {
                var searchResults = await _googlePlaceRepository.GetPlaceById(placeId);
                if (searchResults == null)
                    return NotFound();

                return searchResults;

            }
            catch (Exception Ex)
            {
                _logger.LogError($"{Ex} was thrown by {placeId}");
                throw;
            }
        }


        /// <summary>
        /// Get place activity
        /// </summary>
        /// <param name="placeId"></param>
        /// <returns></returns>
        [HttpGet("activity/{placeId}")]
        public async Task<ActionResult<PlaceLocationResult>> GetPlaceActivity(string placeId)
        {
            if (string.IsNullOrWhiteSpace(placeId))
                return BadRequest();

            try
            {
                var searchResults = await _placeActivityDataRepository.GetPlaceLocationAndActivity(placeId);
                if (searchResults == null)
                    return NotFound();

                return searchResults;

            }
            catch (Exception Ex)
            {
                _logger.LogError($"{Ex} was thrown by {placeId}");
                throw;
            }
        }



        /// <summary>
        /// save place activity data
        /// </summary>
        /// <param name="placeId"></param>
        /// <param name="placeActivity"></param>
        /// <returns></returns>
        [HttpPut("activity/{placeId}")]
        public async Task<ActionResult> Post(string placeId, [FromBody] PlaceActivity activity)
        {
            if (string.IsNullOrWhiteSpace(activity.PlaceId) && placeId != activity.PlaceId)
                return BadRequest();

            var firebaseId = HttpContext.User.Claims.ToArray().FirstOrDefault(f => f.Type.Equals("firebaseId")).Value;
            if (activity.FirebaseId == null && firebaseId != null)
                activity.FirebaseId = firebaseId;
            try
            {
                //var placeActivity = new PlaceActivity
                //{
                //    PlaceId = activity.PlaceId,
                //    FirebaseId = firebaseId,
                //    ActivityAttributes = activity.ActivityAttributes,
                //    HeadCount = activity.HeadCount,
                //    WaitTime = activity.WaitTime
                //};

                var savedActivity = await _placeActivityDataRepository.UpdatePlaceActivity(activity);
                return Ok();
            }
            catch (InvalidActivityUpdateWithinTimespanException updateEx)
            {
                _logger.LogError($"{updateEx} was thrown by {activity.FirebaseId}");
                return Conflict("The user submmited a response very recently. Try again later.");

            }
            catch (Exception Ex)
            {
                _logger.LogError($"{Ex} was thrown by {activity.PlaceId}");
                throw;
            }
        }

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        [HttpGet("aggregate/{placeId}")]
        public async Task<ActionResult<IList<PlaceLocationResult>>> GetActivityAggregation(string placeId)
        {
            if (string.IsNullOrWhiteSpace(placeId))
                return BadRequest();
            try
            {
                return Ok(await _placeActivityDataRepository.AggregatedActivity(placeId));
            }
            catch (Exception Ex)
            {
                _logger.LogError($"{Ex} was thrown by trying to fetch activity for {placeId}");
                throw;
            }
        }



        //[HttpGet("details/{placeId}")]
        //public async Task<ActionResult<IList<AggregatedActivity>>> GetPlaceMetadata(string placeId)
        //{
        //    if (string.IsNullOrWhiteSpace(placeId))
        //        return BadRequest();
        //    try
        //    {
        //        return Ok();
        //    }
        //    catch (Exception Ex)
        //    {
        //        _logger.LogError($"{Ex}");
        //        throw;
        //    }
        //}





    }
}
