using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.DTOs;
using TimeTrackingApi.Domain.Entities;
using TimeTrackingApi.Infrastructure;

namespace TimeTrackingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TimeTrackingDbContext _context;

        public TasksController(TimeTrackingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .Select(t => new TaskDto(t.Id, t.Name, t.ProjectId, t.Project != null ? t.Project.Name : string.Empty, t.IsActive))
                .ToListAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(Guid id)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            return Ok(new TaskDto(task.Id, task.Name, task.ProjectId, task.Project != null ? task.Project.Name : string.Empty, task.IsActive));
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto dto)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId);
            if (!projectExists) return BadRequest("Указанный проект не найден.");

            var task = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ProjectId = dto.ProjectId,
                IsActive = dto.IsActive
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new TaskDto(task.Id, task.Name, task.ProjectId, string.Empty, task.IsActive));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, TaskDto dto)
        {
            if (id != dto.Id) return BadRequest("Идентификаторы не совпадают.");

            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            var projectExists = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId);
            if (!projectExists) return BadRequest("Указанный проект не найден.");

            task.Name = dto.Name;
            task.ProjectId = dto.ProjectId;
            task.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.Include(t => t.TimeEntries).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            if (task.TimeEntries.Any())
            {
                return BadRequest("Нельзя удалить задачу, по которой уже есть списания времени.");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}