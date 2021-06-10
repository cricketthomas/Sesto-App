using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Places.Details.Request;
using GoogleApi.Entities.Places.Details.Response;
using GoogleApi.Entities.Places.Search.Find.Request;
using GoogleApi.Entities.Places.Search.Find.Request.Enums;
using GoogleApi.Entities.Places.Search.Find.Response;
using GoogleApi.Entities.Places.Search.Text.Request;
using GoogleApi.Entities.Places.Search.Text.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using sesto.api.Infastructure;
using sesto.api.Infastructure.Data;
using sesto.api.Models;
using sesto.api.Services.Interfaces;

namespace sesto.api.Services.Repositories
{
    public class GooglePlaceRepository : IGooglePlaceRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GooglePlaceRepository> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClient;
        public readonly IGenericPictrueRepository _genericPictureRepo;

        public GooglePlaceRepository(ILogger<GooglePlaceRepository> logger, IConfiguration configuration, IMemoryCache memoryCache, IHttpClientFactory httpClient, IGenericPictrueRepository genericPictureRepo)
        {
            _logger = logger;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _httpClient = httpClient;
            _genericPictureRepo = genericPictureRepo;

        }


        public HttpClient client => _httpClient.CreateClient();
        public string ImageBaseAddress => _configuration["GooglePlaceImageBaseURL"];
        public string apiKey => _configuration["GoogleAPIKey"];


        public async Task<PlacesFindSearchResponse> GetPlaceAsync(string input, double latitude, double longitude, int mileRadius)
        {
            var apiKey = _configuration["GoogleAPIKey"];
            var ImageBaseAddress = _configuration["GooglePlaceImageBaseURL"];
            var request = new PlacesFindSearchRequest()
            {
                Key = apiKey,
                Fields = FieldTypes.Basic,
                Radius = mileRadius,
                Location = new Location(latitude, longitude),
                Input = input,
            };

            var results = await GooglePlaces.FindSearch.QueryAsync(request);


            return results;
        }


        private async Task<Uri> GetCachedGooglePlaceImageURL(string placeName, int imageWidth, string photoRef)
        {
            return await _memoryCache.GetOrCreateAsync($"{placeName}", async (ICacheEntry f) =>
            {
                f.SlidingExpiration = TimeSpan.FromHours(1);
                var imageURL = (await client.GetAsync($"{ImageBaseAddress}maxwidth={imageWidth}&photoreference={photoRef}&sensor=false&key={apiKey}")).RequestMessage.RequestUri;
                return imageURL;
            });
        }


        public async Task<IEnumerable<PlacesTextModel>> TextSearchPlacesAsync(string input, double latitude, double longitude, int mileRadius)
        {
            var cacheKey = input.Trim().Replace(" ", "_") + $"_{latitude}_{longitude}_{mileRadius}";

            var _placesResults = await _memoryCache.GetOrCreateAsync(cacheKey, async (f) =>
            {
                f.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);


                var request = new PlacesTextSearchRequest()
                {
                    Key = apiKey,
                    Radius = mileRadius,
                    Location = new Location(latitude, longitude),
                    Query = input,
                };

                var searchResponse = await GooglePlaces.TextSearch.QueryAsync(request);
                if (searchResponse.Status == Status.ZeroResults)
                    return null;

                var places = searchResponse.Results.Select(async r =>
                {
                    //var domain = await _crunchbase.GetDomain(r.Name);
                    //var imageWidth = r.Photos.First().Width;
                    //var photoRef = r.Photos.First().PhotoReference;
                    //var ImageUrl = await GetCachedGooglePlaceImageURL(r.Name, imageWidth, photoRef);
                    var placeType = r.Types.FirstOrDefault().ToString().ToLowerInvariant().Replace("_", " ");
                    var slug = _genericPictureRepo.GetEmoji(r.Name, placeType).Character ?? string.Empty;


                    return new PlacesTextModel
                    {
                        PlaceId = r.PlaceId,
                        Name = r.Name,
                        FormattedAddress = r.FormattedAddress,
                        BusinessStatus = r.BusinessStatus,
                        IsCurrentlyOpen = r.OpeningHours == null ? null : r.OpeningHours.OpenNow,
                        Rating = r.Rating,
                        Location = r.Geometry.Location,
                        //PhotoUrl = ImageUrl.ToString(),
                        Slug = slug,
                        //PhotoUrl =  domain != null ? $"https://logo.clearbit.com/{domain}" : null,
                        // TODO:
                        // this can will not working without aatribution.
                        // create a service that fetches images curated by undraw and sercies them up based on the location type. 
                        Type = r.Types.FirstOrDefault(),
                        Types = r.Types.Select(s => s.Value.ToString()).ToArray()

                    };

                });
                return Task.WhenAll(places).Result;
            });

            return _placesResults;

        }

        public async Task<PlaceLocationResult> GetPlaceById(string placeId)
        {

            return await _memoryCache.GetOrCreateAsync($"google_{placeId}", async (arg) =>
            {
                arg.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
                var result = (await GooglePlaces.Details.QueryAsync(new PlacesDetailsRequest
                {
                    Key = apiKey,
                    PlaceId = placeId
                }));

                if (result.Status == Status.ZeroResults)
                    return null;

                //var imageWidth = result.Photos.FirstOrDefault().Width;
                //var photoRef = result.Photos.FirstOrDefault().PhotoReference;
                //var imageURL = (await httpClient.GetAsync($"{ImageBaseAddress}maxwidth={imageWidth}&photoreference={photoRef}&sensor=false&key={apiKey}")).RequestMessage.RequestUri;
                //var logoUrl = "https://logo.clearbit.com/" + await _crunchbase.GetDomain(result.Result.Name);

                var types = result.Result.Types.Select(t => Enum.GetName(typeof(PlaceLocationType), t)).ToArray();
                var genericPhoto = await _genericPictureRepo.GetFirstPexelImage(types);

                return new PlaceLocationResult
                {
                    PlaceId = result.Result.PlaceId,
                    Name = result.Result.Name,
                    FormattedAddress = result.Result.FormattedAddress,
                    Website = result.Result.Website,
                    FormattedPhoneNumber = result.Result.FormattedPhoneNumber,
                    Rating = result.Result.Rating,
                    BusinessStatus = result.Result.BusinessStatus,

                    OpeningHours = result.Result.OpeningHours,
                    Types = result.Result.Types,
                    GenericPhoto = genericPhoto,
                    Review = result.Result.Review,
                    UserRatingsTotal = result.Result.UserRatingsTotal,
                    PriceLevel = result.Result.PriceLevel,
                    Vicinity = result.Result.Vicinity,
                    AdrAddress = result.Result.AdrAddress,
                    InternationalPhoneNumber = result.Result.InternationalPhoneNumber
                };
            });
        }
    }
}
