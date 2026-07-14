using System;

namespace TimeTrackingApi.Domain.Entities
{
    public class TimeEntry
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public decimal Hours { get; set; }
        public string Description { get; set; } = string.Empty;
        
        public Guid TaskId { get; set; }
        public TaskEntity? Task { get; set; }
        
        public Guid UserId { get; set; } 
    }
}