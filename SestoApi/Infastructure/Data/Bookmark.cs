using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sesto.api.Infastructure.Data
{
    /// <summary>
    /// this is the table of saved place by user bound to their profile.
    /// </summary>
    public class Bookmark
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public string FirebaseId { get; set; }
        public string PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string FormattedAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual User User { get; set; }





        [NotMapped]
        public virtual string PhotoUrl { get; set; }



    }
}
