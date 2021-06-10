using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using sesto.api.Infastructure;
using sesto.api.Services.Interfaces;
using sesto.api.Infastructure.Data;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using sesto.api.Exceptions;
using sesto.api.Models;
using System.Collections.Generic;

namespace sesto.api.Services.Repositories
{
    public class PlaceActivityDataRepository : IPlaceActivityDataRepository
    {
        private readonly SestoDbContext _dbContext;
        private readonly ILogger<PlaceActivityDataRepository> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IGooglePlaceRepository _googlePlaceRepository;

        public PlaceActivityDataRepository(SestoDbContext dbContext, IMemoryCache memoryCache, ILogger<PlaceActivityDataRepository> logger, IConfiguration configuration, IGooglePlaceRepository googlePlaceRepository)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
            _logger = logger;
            _configuration = configuration;
            _googlePlaceRepository = googlePlaceRepository;

        }


        /// <summary>
        /// this checks for a cache key by the FirebaseId and PlaceId to stop the user from diluting the response results.
        /// if they have submitted an update for a place within the last 5 minutes, 
        /// </summary>
        /// <param name="placeActivity"></param>
        /// <returns></returns>
        public async Task<PlaceActivity> UpdatePlaceActivity(PlaceActivity placeActivity)
        {
            try
            {
                int invalidateCacheTime = _configuration.GetValue<int>("invalidateCacheTime");
                var cacheKey = $"{placeActivity.FirebaseId}_{placeActivity.PlaceId}";
                _memoryCache.TryGetValue(cacheKey, out bool hasCachedEntry);
                if (!hasCachedEntry)
                {
                    try
                    {
                        var place = await CreatePlaceLocation(placeActivity.PlaceId);

                        var pa = new PlaceActivity
                        {
                            ActivityAttributes = placeActivity.ActivityAttributes,
                            PlaceId = placeActivity.PlaceId,
                            FirebaseId = placeActivity.FirebaseId,
                            HeadCount = placeActivity.HeadCount,
                            WaitTime = placeActivity.WaitTime

                        };
                        //place.CurrentActivity.Add(pa);

                        await _dbContext.Activity.AddAsync(placeActivity);
                        await _dbContext.SaveChangesAsync();

                        _memoryCache.GetOrCreate(cacheKey, (entry) =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(invalidateCacheTime);
                            return true;
                        });
                        return placeActivity;

                    }
                    catch
                    {
                        throw;
                    }


                }

                throw new InvalidActivityUpdateWithinTimespanException(name: invalidateCacheTime.ToString());

            }

            catch (Exception Ex)
            {
                _logger.LogError($"{Ex}, from user: {placeActivity.PlaceId}");
                throw;

            }
        }


        /// <summary>
        /// This creates and returns the google place in the Sesto db.
        /// </summary>
        /// <param name="placeId"></param>
        /// <returns></returns>
        public async Task<PlaceLocationResult> CreatePlaceLocation(string placeId)
        {
            try
            {
                var existingPlace = await _dbContext.Location.FirstOrDefaultAsync(place => place.PlaceId.Equals(placeId));
                var _place = await _googlePlaceRepository.GetPlaceById(placeId);
                if (existingPlace == null)
                {
                    var placeLocation = new PlaceLocationResult { PlaceId = _place.PlaceId };
                    await _dbContext.Location.AddAsync(placeLocation);
                    await _dbContext.SaveChangesAsync();
                }

                return _place;

            }
            catch (Exception Ex)
            {
                _logger.LogError($"{Ex}. {placeId} was unable to be saved.");
                throw new ApplicationException("Unable to save the new place.");
            }
        }



        // TODO: delete
        public async Task<PlaceLocationResult> GetPlaceLocationAndActivity(string placeId)
        {
            try
            {
                var place = await _dbContext.Location.FirstOrDefaultAsync(pp => pp.PlaceId == placeId);

                if (place == null)
                    place = await CreatePlaceLocation(placeId);


                var _placeDetails = await _googlePlaceRepository.GetPlaceById(placeId);

                return _placeDetails;
            }

            catch (Exception Ex)
            {
                _logger.LogError($"{Ex}, from fetching place {placeId}");
                throw new ApplicationException("Unable to retrive the place and current acitivity.");
            }

        }



        public async Task<PlaceLocationResult> AggregatedActivity(string placeId)
        {
            try
            {
                var locationActivity = await _dbContext.Location.Include(pl => pl.CurrentActivity)
                    .ThenInclude(pli => pli.ActivityAttributes)
                    .FirstOrDefaultAsync(pp => pp.PlaceId == placeId);

                var groupedActivity = locationActivity.CurrentActivity.GroupBy(ca => ca.CreatedAt.DayOfWeek)
                    .Select(ga => new DailyAggregatedActivity
                    {
                        DayOfWeek = ga.Key,
                        AverageHeadCount = ga.Select(s => s.HeadCount).Sum() / ga.Count(),
                        AverageWaitTime = ga.Select(s => s.WaitTime).Sum() / ga.Count(),
                    }).ToList();

                var place = await _googlePlaceRepository.GetPlaceById(placeId);
                place.DailyAggregatedActivity = groupedActivity;

                return place;
            }

            catch (Exception Ex)
            {
                _logger.LogError($"{Ex}, from fetching place {placeId}");
                throw new ApplicationException("Unable to retrive the place and current acitivity.");
            }

        }





    }
}
