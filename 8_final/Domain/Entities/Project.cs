using System;
using System.Collections.Generic;

namespace TimeTrackingApi.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
    }
}