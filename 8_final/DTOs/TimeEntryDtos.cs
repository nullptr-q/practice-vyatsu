using System;
using System.ComponentModel.DataAnnotations;

namespace TimeTrackingApi.DTOs
{
    public record TimeEntryDto(
        Guid Id, 
        DateOnly Date, 
        decimal Hours, 
        string Description, 
        Guid TaskId, 
        string TaskName,
        Guid UserId
    );

    public record CreateTimeEntryDto(
        DateOnly Date,
        [Range(0.01, 24.0, ErrorMessage = "Количество часов должно быть от 0.01 до 24")] decimal Hours,
        string Description,
        Guid TaskId,
        Guid UserId
    );

    public record UpdateTimeEntryDto(
        DateOnly Date,
        [Range(0.01, 24.0, ErrorMessage = "Количество часов должно быть от 0.01 до 24")] decimal Hours,
        string Description,
        Guid TaskId
    );

    public record DailyHoursSummaryDto(
        DateOnly Date, 
        decimal TotalHours, 
        string StatusColor
    );
}