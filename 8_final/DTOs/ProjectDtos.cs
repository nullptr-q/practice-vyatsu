using System;
using System.ComponentModel.DataAnnotations;

namespace TimeTrackingApi.DTOs
{
    public record ProjectDto(Guid Id, string Name, string Code, bool IsActive);
    public record CreateProjectDto([Required] string Name, [Required] string Code, bool IsActive = true);
}