using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.DTOs;
using TimeTrackingApi.Services;
using TimeTrackingApi.Infrastructure;

namespace TimeTrackingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeEntriesController : ControllerBase
    {
        private readonly TimeTrackingDbContext _context;
        private readonly TimeEntryService _service;

        public TimeEntriesController(TimeTrackingDbContext context, TimeEntryService service)
        {
            _context = context;
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntries(
            [FromQuery] DateOnly? date,
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] Guid? userId)
        {
            var query = _context.TimeEntries.Include(te => te.Task).AsQueryable();

            if (userId.HasValue)
                query = query.Where(te => te.UserId == userId.Value);

            if (date.HasValue)
            {
                query = query.Where(te => te.Date == date.Value);
            }
            else if (year.HasValue && month.HasValue)
            {
                query = query.Where(te => te.Date.Year == year.Value && te.Date.Month == month.Value);
            }

            var entries = await query
                .Select(te => new TimeEntryDto(
                    te.Id, 
                    te.Date, 
                    te.Hours, 
                    te.Description, 
                    te.TaskId, 
                    te.Task != null ? te.Task.Name : string.Empty,
                    te.UserId))
                .ToListAsync();

            return Ok(entries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TimeEntryDto>> GetTimeEntry(Guid id)
        {
            var te = await _context.TimeEntries.Include(t => t.Task).FirstOrDefaultAsync(t => t.Id == id);
            if (te == null) return NotFound();

            return Ok(new TimeEntryDto(
                te.Id, 
                te.Date, 
                te.Hours, 
                te.Description, 
                te.TaskId, 
                te.Task != null ? te.Task.Name : string.Empty, 
                te.UserId));
        }

        [HttpPost]
        public async Task<ActionResult<TimeEntryDto>> CreateTimeEntry(CreateTimeEntryDto dto)
        {
            var result = await _service.CreateEntryAsync(dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            var entry = result.Entry!;
            return CreatedAtAction(nameof(GetTimeEntry), new { id = entry.Id }, 
                new TimeEntryDto(entry.Id, entry.Date, entry.Hours, entry.Description, entry.TaskId, string.Empty, entry.UserId));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeEntry(Guid id, UpdateTimeEntryDto dto)
        {
            var result = await _service.UpdateEntryAsync(id, dto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeEntry(Guid id)
        {
            var entry = await _context.TimeEntries.FindAsync(id);
            if (entry == null) return NotFound();

            _context.TimeEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("daily-summary")]
        public async Task<ActionResult<IEnumerable<DailyHoursSummaryDto>>> GetDailySummary(
            [FromQuery] Guid userId, 
            [FromQuery] int year, 
            [FromQuery] int month)
        {
            var dailyHours = await _context.TimeEntries
                .Where(te => te.UserId == userId && te.Date.Year == year && te.Date.Month == month)
                .GroupBy(te => te.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalHours = g.Sum(te => te.Hours)
                })
                .ToListAsync();

            var summaries = dailyHours.Select(dh => new DailyHoursSummaryDto(
                dh.Date,
                dh.TotalHours,
                dh.TotalHours switch
                {
                    < 8 => "Yellow",
                    8 => "Green",
                    _ => "Red"
                }
            )).ToList();

            return Ok(summaries);
        }
    }
}