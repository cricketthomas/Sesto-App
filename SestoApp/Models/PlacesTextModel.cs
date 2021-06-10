using System;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Places.Search.Common;

namespace SestoApp.Models
{

    /// <summary>
    /// the customized response from the google places api
    /// </summary>
    public class PlacesTextModel
    {
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public bool? IsCurrentlyOpen { get; set; }
        public BusinessStatus BusinessStatus { get; set; }
        public string FormattedAddress { get; set; }
        public double Rating { get; set; }
        public Location Location { get; set; }
        public string PhotoUrl { get; set; }
        public PlaceLocationType? Type { get; set; }
        public string[] Types { get; set; }
        public string Slug { get; set; }

    }
}
