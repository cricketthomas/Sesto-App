using System;
using System.ComponentModel.DataAnnotations;
using sesto.api.Infastructure.Data.Enums;

namespace sesto.api.Infastructure.Data
{
    public class ActivityAttributesTypes
    {

        [Key]
        public int PlaceAttributeId { get; set; }
        public string AttributeName { get; set; }

    }
}
