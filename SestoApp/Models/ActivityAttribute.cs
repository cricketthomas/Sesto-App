using System;
namespace SestoApp.Models
{
    public class ActivityAttribute
    {

        public int ActivityAttributeId { get; set; }
        public int AttributeTypeId { get; set; }

        public Guid ActivityId { get; set; }
        public PlaceActivity Activity { get; set; }

    }
}
