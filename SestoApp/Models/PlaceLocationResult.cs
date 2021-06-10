using System;
using System.Collections.Generic;
using System.Linq;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.Details.Response;

namespace SestoApp.Models
{
    public class PlaceLocationResult : DetailsResult
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string GenericPhoto { get; set; }
        public virtual ICollection<PlaceActivity> CurrentActivity { get; set; }
        public virtual IList<DailyAggregatedActivity> DailyAggregatedActivity { get; set; }
        // Inherited

    }
}
