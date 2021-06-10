using System;
using SQLite;

namespace SestoApp.Infastructure
{
    public class Bookmark
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string FormattedAddress { get; set; }
        public string PhotoUrl { get; set; }
        public string Slug { get; set; }
        public string FirebaseId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}