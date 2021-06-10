using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Details.Response;
using GoogleApi.Entities.Places.Search.Find.Response;
using sesto.api.Infastructure.Data;
using sesto.api.Models;

namespace sesto.api.Services.Interfaces
{
    public interface IGooglePlaceRepository
    {

        Task<PlacesFindSearchResponse> GetPlaceAsync(string input, double latitude, double longitude, int mileRadius);
        Task<IEnumerable<PlacesTextModel>> TextSearchPlacesAsync(string input, double latitude, double longitude, int mileRadius);
        Task<PlaceLocationResult> GetPlaceById(string placeId);
    }
}
