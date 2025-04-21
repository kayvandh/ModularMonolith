using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Domain
{
    public class ScheduledJob
    {
        public int Id { get; set; }

        public string JobType { get; set; } = string.Empty;     
        public string? Payload { get; set; } = string.Empty;    

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ScheduleTime { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? ExecutedAt { get; set; }

        public string Status { get; set; } = "Pending"; // Pending / Scheduled / Executed / Failed
        public string? Error { get; set; }

        public bool IsRecurring { get; set; } = false;   
        public string? CronExpression { get; set; }
    }
}
