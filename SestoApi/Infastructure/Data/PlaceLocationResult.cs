using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Places.Common;
using GoogleApi.Entities.Places.Common.Enums;
using GoogleApi.Entities.Places.Details.Response;
using sesto.api.Models;

namespace sesto.api.Infastructure.Data
{
    public class PlaceLocationResult : DetailsResult
    {
        [Key]
        public override string PlaceId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public string GenericPhoto { get; set; }

        [ForeignKey("PlaceId")]
        public virtual ICollection<PlaceActivity> CurrentActivity { get; set; }

        [NotMapped]
        public virtual IList<DailyAggregatedActivity> DailyAggregatedActivity { get; set; }



        // Inherited

        [NotMapped]
        public override IEnumerable<AddressComponent> AddressComponents { get; set; }

        [NotMapped]
        public override string FormattedAddress { get; set; }

        [NotMapped]
        public override string FormattedPhoneNumber { get; set; }

        [NotMapped]
        public override string AdrAddress { get; set; }

        [NotMapped]
        public override Geometry Geometry { get; set; }

        [NotMapped]
        public override string Icon { get; set; }

        [NotMapped]
        public override string InternationalPhoneNumber { get; set; }

        [NotMapped]
        public override string Name { get; set; }

        [NotMapped]
        public override OpeningHours OpeningHours { get; set; }

        [NotMapped]
        public override IEnumerable<Photo> Photos { get; set; }

        [NotMapped]
        public override PriceLevel PriceLevel { get; set; }

        [NotMapped]
        public override BusinessStatus BusinessStatus { get; set; }

        [NotMapped]
        public override double Rating { get; set; }

        [NotMapped]
        public override int UserRatingsTotal { get; set; }

        [NotMapped]
        public override IEnumerable<Review> Review { get; set; }

        [NotMapped]
        public override IEnumerable<PlaceLocationType> Types { get; set; }

        [NotMapped]
        public override string Url { get; set; }

        [NotMapped]
        public override string Vicinity { get; set; }

        [NotMapped]
        public override int UtcOffset { get; set; }

        [NotMapped]
        public override string Website { get; set; }
    }
}
