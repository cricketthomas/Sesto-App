using System;
namespace SestoApp.Models
{
    public class DailyAggregatedActivity
    {
        public DayOfWeek DayOfWeek { get; set; }
        public decimal AverageHeadCount { get; set; }
        public decimal AverageWaitTime { get; set; }
    }
}
