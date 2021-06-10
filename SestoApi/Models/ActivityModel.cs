using System;
using System.Collections.Generic;
using sesto.api.Infastructure.Data;

namespace sesto.api.Models
{
    public class ActivityModel
    {
        public string PlaceId { get; set; }
        public int HeadCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string FirebaseId { get; set; }
        public IEnumerable<ActivityAttribute> ActivityAttributes { get; set; }
        public int WaitTime { get; set; }
     
    }
}
