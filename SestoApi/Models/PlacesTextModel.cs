using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;

namespace sesto.api.Models
{
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

