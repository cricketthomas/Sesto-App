using System;
using System.ComponentModel.DataAnnotations;
using sesto.api.Infastructure.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace sesto.api.Infastructure.Data
{
    public class PlaceActivity
    {
        [Key]
        public Guid ActivityId { get; set; }

        [ForeignKey("PlaceLocation")]
        public string PlaceId { get; set; }

        [ForeignKey("User")]
        public string FirebaseId { get; set; }

        public int HeadCount { get; set; }
        public int WaitTime { get; set; }


        public IEnumerable<ActivityAttribute> ActivityAttributes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
