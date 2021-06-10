using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sesto.api.Infastructure.Data;
using sesto.api.Models;

namespace sesto.api.Services.Interfaces
{
    public interface IPlaceActivityDataRepository
    {
        Task<PlaceActivity> UpdatePlaceActivity(PlaceActivity placeActivity);
        Task<PlaceLocationResult> CreatePlaceLocation(string placeId);
        Task<PlaceLocationResult> GetPlaceLocationAndActivity(string placeId);
        Task<PlaceLocationResult> AggregatedActivity(string placeId);

    }

}
