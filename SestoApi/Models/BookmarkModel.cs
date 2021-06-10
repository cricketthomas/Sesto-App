using System;
namespace sesto.api.Models
{
    public class BookmarkModel
    {
        public string Id { get; set; }
        public string FirebaseId { get; set; }
        public string PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string FormattedAddress { get; set; }
    }
}
