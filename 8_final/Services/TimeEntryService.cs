using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.DTOs;
using TimeTrackingApi.Domain.Entities;
using TimeTrackingApi.Infrastructure;

namespace TimeTrackingApi.Services
{
    public class TimeEntryService
    {
        private readonly TimeTrackingDbContext _context;

        public TimeEntryService(TimeTrackingDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string ErrorMessage, TimeEntry? Entry)> CreateEntryAsync(CreateTimeEntryDto dto)
        {
            var task = await _context.Tasks.FindAsync(dto.TaskId);
            if (task == null)
                return (false, "Указанная задача не найдена.", null);
            
            if (!task.IsActive)
                return (false, "Нельзя списать время на неактивную задачу.", null);

            decimal currentHoursForDay = await _context.TimeEntries
                .Where(te => te.UserId == dto.UserId && te.Date == dto.Date)
                .SumAsync(te => te.Hours);

            if (currentHoursForDay + dto.Hours > 24)
                return (false, $"Суммарное время за {dto.Date} превысит 24 часа. Доступно для списания: {24 - currentHoursForDay} ч.", null);

            var entry = new TimeEntry
            {
                Id = Guid.NewGuid(),
                Date = dto.Date,
                Hours = dto.Hours,
                Description = dto.Description,
                TaskId = dto.TaskId,
                UserId = dto.UserId
            };

            _context.TimeEntries.Add(entry);
            await _context.SaveChangesAsync();

            return (true, string.Empty, entry);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateEntryAsync(Guid id, UpdateTimeEntryDto dto)
        {
            var entry = await _context.TimeEntries
                .Include(te => te.Task)
                .FirstOrDefaultAsync(te => te.Id == id);

            if (entry == null)
                return (false, "Проводка не найдена.");

            if (entry.Task != null && !entry.Task.IsActive)
            {
                if (entry.TaskId != dto.TaskId)
                {
                    return (false, "Невозможно изменить задачу для данной проводки, так как текущая задача в ней неактивна.");
                }
            }

            if (entry.TaskId != dto.TaskId)
            {
                var targetTask = await _context.Tasks.FindAsync(dto.TaskId);
                if (targetTask == null)
                    return (false, "Новая задача не найдена.");
                if (!targetTask.IsActive)
                    return (false, "Нельзя перенести проводку на неактивную задачу.");
            }

            decimal hoursWithoutCurrentEntry = await _context.TimeEntries
                .Where(te => te.UserId == entry.UserId && te.Date == dto.Date && te.Id != id)
                .SumAsync(te => te.Hours);

            if (hoursWithoutCurrentEntry + dto.Hours > 24)
                return (false, $"Превышен лимит времени за {dto.Date}. Невозможно установить {dto.Hours} ч.");

            entry.Date = dto.Date;
            entry.Hours = dto.Hours;
            entry.Description = dto.Description;
            entry.TaskId = dto.TaskId;

            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }
    }
}