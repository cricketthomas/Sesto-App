using System;
using System.ComponentModel.DataAnnotations.Schema;
using sesto.api.Infastructure.Data.Enums;

namespace sesto.api.Infastructure.Data
{
    public class ActivityAttribute
    {

        public int ActivityAttributeId { get; set; }
        public int AttributeTypeId { get; set; }

        public Guid ActivityId { get; set; }
        public virtual PlaceActivity Activity { get; set; }

    }
}
