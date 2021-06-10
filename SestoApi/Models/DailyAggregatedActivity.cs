using System;
namespace sesto.api.Models
{
    public class DailyAggregatedActivity
    {
        public DayOfWeek DayOfWeek { get; set; }
        public decimal AverageHeadCount { get; set; }
        public decimal AverageWaitTime { get; set; }
    }
}
