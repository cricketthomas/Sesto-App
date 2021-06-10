using System;
using System.Collections.Generic;

namespace SestoApp.Models
{
    public class PlaceActivityModel
    {
        public string PlaceId { get; set; }
        public string FirebaseId { get; set; }
        public int HeadCount { get; set; }
        public int WaitTime { get; set; }
        public IEnumerable<ActivityAttributeModel> ActivityAttributes { get; set; }

    }

}
