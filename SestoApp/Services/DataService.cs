using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SestoApp.Interfaces;
using SestoApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SestoApp.Services
{
    public static class DataService
    {
        public static List<PlacesTextModel> Locations { get; set; }
        public static PlaceLocationResult LocationsDetails { get; set; }
        public static IFirebaseAuthentication auth = DependencyService.Get<IFirebaseAuthentication>();
        static CancellationTokenSource cts;
        private static readonly ILogger _logger;

        #region AppConfiguration

        private static async Task<JsonElement> GetAppConfigValue(string key)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resName = assembly.GetManifestResourceNames()
                ?.FirstOrDefault(r => r.EndsWith("settings.json", StringComparison.OrdinalIgnoreCase));
            using var file = assembly.GetManifestResourceStream(resName);
            using var sr = new StreamReader(file);
            var json = await JsonDocument.ParseAsync(file);
            json.RootElement.TryGetProperty(key, out var jsonValue);
            return jsonValue;
        }

        #endregion



        #region HttpClient    
        public static HttpClient BaseClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(GetAppConfigValue("BaseUrl").Result.GetString());
            var oauthToken = string.Empty;

            try
            {
                oauthToken = SecureStorage.GetAsync("oauth_token").Result;
            }
            catch (Exception ex)
            {
                oauthToken = Preferences.Get("SestoAppFirebaseToken", "");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
            return client;
        }
        #endregion

        #region GetCurrentLocation
        async static Task<Location> GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();

                var requestCancellationTime = (await GetAppConfigValue("requestCancellationTime")).GetInt32();
                cts.CancelAfter(requestCancellationTime);

                var location = await Geolocation.GetLocationAsync(request, cts.Token);
                var x = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    Debug.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    return location;
                }
                return null;
            }

            catch (FeatureNotSupportedException fnsEx)
            {
                _logger.LogWarning("location exception", fnsEx);
                return null;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                _logger.LogWarning("location exception", fneEx);
                return null;
            }
            catch (PermissionException pEx)
            {
                _logger.LogWarning("location exception", pEx);
                return null;

            }
            catch (Exception ex)
            {
                _logger.LogWarning("location exception", ex);
                return null;
            }
        }
        #endregion



        #region GetSearchResults
        public async static Task<List<PlacesTextModel>> GetSearchResults(string queryString, int mileRadius = 1)
        {
            await auth.RefreshAuthToken(false);

            var _client = BaseClient();
            var location = await GetCurrentLocation();
            var normalizedQuery = queryString?.ToLower().Trim() ?? "";

            var queryRequestString = $"/api/places/search?query={normalizedQuery}";
            if (location != null)
            {
                queryRequestString += $"&latitude={location.Latitude}&longitude={location.Longitude}&mileRadius={mileRadius}";
            }
            Debug.WriteLine(queryString);

            cts = new CancellationTokenSource();

            var requestCancellationTime = (await GetAppConfigValue("requestCancellationTime")).GetInt32();
            cts.CancelAfter(requestCancellationTime);

            var responseMessage = await _client.GetAsync(queryRequestString, cts.Token);
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                return await JsonSerializer.DeserializeAsync<List<PlacesTextModel>>(await responseMessage.Content.ReadAsStreamAsync(), options);
            }
            return null;
        }
        #endregion

        #region GetPlaceById
        public async static Task<PlaceLocationResult> GetGooglePlaceById(string placeId)
        {
            await auth.RefreshAuthToken(false);

            var _client = BaseClient();
            var queryRequestString = $"/api/places/details/{placeId}";

            try
            {
                var responseMessage = await _client.GetAsync(queryRequestString);

                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var options = new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    cts = new CancellationTokenSource();
                    var requestCancellationTime = (await GetAppConfigValue("requestCancellationTime")).GetInt32();
                    cts.CancelAfter(requestCancellationTime);
                    return await JsonSerializer.DeserializeAsync<PlaceLocationResult>(await responseMessage.Content.ReadAsStreamAsync(), options, cts.Token);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("unable to fetch place");
            }

        }
        #endregion



        #region CreateActivityReport
        public async static Task CreateActivityReport(PlaceActivityModel activity)
        {
            await auth.RefreshAuthToken(false);

            var _client = BaseClient();
            var queryRequestString = $"/api/places/activity/{activity.PlaceId}";
            var content = JsonSerializer.Serialize(activity);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            try
            {
                var responseMessage = await _client.PutAsync(queryRequestString, httpContent);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    return;

                if (responseMessage.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw new ApplicationException($"{responseMessage.RequestMessage}");
                }

                if (responseMessage.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new ApplicationException($"{responseMessage.RequestMessage}");
                }

            }
            catch (Exception Ex)
            {
                _logger.LogCritical("error updating place activity.");
            }

        }
        #endregion





        #region GetActivityReport
        public async static Task<PlaceActivity> GetActivityReport(string placeId)
        {

            await auth.RefreshAuthToken(false);
            var _client = BaseClient();
            var queryRequestString = $"/api/places/activity/{placeId}";
            try
            {
                var responseMessage = await _client.GetAsync(queryRequestString);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var options = new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    cts = new CancellationTokenSource();
                    var requestCancellationTime = (await GetAppConfigValue("requestCancellationTime")).GetInt32();
                    cts.CancelAfter(requestCancellationTime);
                    return await JsonSerializer.DeserializeAsync<PlaceActivity>(await responseMessage.Content.ReadAsStreamAsync(), options, cts.Token);
                }

                return null;
            }
            catch (Exception Ex)
            {
                throw new Exception("There was an issue fetching activity for this location.", Ex);
                //await App.Current.MainPage.DisplayAlert("Uh oh..", "The bookmark was not saved.", "Dismiss");
            }
        }
        #endregion
        #region CreateBookmark
        //public async static Task CreateBookmark(Bookmark bookmark)
        //{

        //    await auth.RefreshAuthToken(false);
        //    var _client = BaseClient();
        //    var queryRequestString = $"/api/bookmark";
        //    try
        //    {
        //        var responseMessage = await _client.GetAsync(queryRequestString);
        //        var requestCancellationTime = (await GetAppConfigValue("requestCancellationTime")).GetInt32();
        //        cts.CancelAfter(requestCancellationTime);
        //        if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
        //            return;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Unable to create bookmark.");
        //    }

        //}
        #endregion
        #region ValidateBookmark


        /// <summary>
        /// searchs for whether the bookmark exists. If it does, return it. otherwise, return null.
        /// </summary>
        /// <param name="placeId"></param>
        /// <returns></returns>
        //public async static Task<Bookmark> ValidateBookmark(string placeId)
        //{
        //    await auth.RefreshAuthToken(false);
        //    var _client = BaseClient();
        //    var queryRequestString = $"/api/bookmark/validate/{placeId}";
        //    try
        //    {
        //        var responseMessage = await _client.GetAsync(queryRequestString);

        //        if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            var options = new JsonSerializerOptions()
        //            {
        //                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //            };

        //            cts = new CancellationTokenSource();
        //            var requestCancellationTime = (await GetAppConfigValue("requestCancellationTime")).GetInt32();
        //            cts.CancelAfter(requestCancellationTime); cts.CancelAfter(requestCancellationTime);
        //            return await JsonSerializer.DeserializeAsync<Bookmark>(await responseMessage.Content.ReadAsStreamAsync(), options, cts.Token);
        //        }
        //        if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
        //        {

        //            return null;
        //        }
        //        return null;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Unable to create bookmark.");
        //    }

        //}
        #endregion
        #region DeleteBookmark
        //public async static Task DeleteBookmark(string bookmarkId)
        //{
        //    await auth.RefreshAuthToken(false);
        //    var _client = BaseClient();
        //    var queryRequestString = $"/api/bookmark/{bookmarkId}";
        //    try
        //    {
        //        var responseMessage = await _client.GetAsync(queryRequestString);
        //        var requestCancellationTime = (await GetAppConfigValue("requestCancellationTime")).GetInt32();
        //        cts.CancelAfter(requestCancellationTime);
        //        if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
        //            return;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Unable to delete bookmark.");
        //    }
        //}
        #endregion
    }
}