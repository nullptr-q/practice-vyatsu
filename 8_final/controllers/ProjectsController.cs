using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.DTOs;
using TimeTrackingApi.Domain.Entities;
using TimeTrackingApi.Infrastructure;

namespace TimeTrackingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly TimeTrackingDbContext _context;

        public ProjectsController(TimeTrackingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _context.Projects
                .Select(p => new ProjectDto(p.Id, p.Name, p.Code, p.IsActive))
                .ToListAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            return Ok(new ProjectDto(project.Id, project.Name, project.Code, project.IsActive));
        }

        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto dto)
        {
            if (await _context.Projects.AnyAsync(p => p.Code == dto.Code))
            {
                return BadRequest("Проект с таким кодом уже существует.");
            }

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                IsActive = dto.IsActive
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, new ProjectDto(project.Id, project.Name, project.Code, project.IsActive));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, ProjectDto dto)
        {
            if (id != dto.Id) return BadRequest("Идентификаторы не совпадают.");

            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            project.Name = dto.Name;
            project.Code = dto.Code;
            project.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _context.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return NotFound();

            if (project.Tasks.Any())
            {
                return BadRequest("Нельзя удалить проект, содержащий задачи.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}