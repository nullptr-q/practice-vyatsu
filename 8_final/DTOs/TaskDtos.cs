using System;
using System.ComponentModel.DataAnnotations;

namespace TimeTrackingApi.DTOs
{
    public record TaskDto(Guid Id, string Name, Guid ProjectId, string ProjectName, bool IsActive);
    public record CreateTaskDto([Required] string Name, Guid ProjectId, bool IsActive = true);
}