using System;
using System.Collections.Generic;

namespace TimeTrackingApi.Domain.Entities
{
    public class TaskEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        
        public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    }
}