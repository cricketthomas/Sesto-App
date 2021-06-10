using System;
using System.Collections.Generic;

namespace SestoApp.Models
{
    public class PlaceActivity
    {
        public Guid? ActivityId { get; set; }
        public string PlaceId { get; set; }
        public string? FirebaseId { get; set; }
        public int HeadCount { get; set; }
        public int WaitTime { get; set; }
        public IEnumerable<ActivityAttribute> ActivityAttributes { get; set; }

        //public DateTime CreatedAt { get; set; } = DateTime.Now;

    }

}
