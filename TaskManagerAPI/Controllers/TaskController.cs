using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var tasks = _context.Tasks
                .Where(t => t.AssignedUserId == userId)
                .ToList();

            return Ok(tasks);
        }

        public class CreateTaskDto
        {
            [Required]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }
        }

        [HttpPost]
        public IActionResult CreateTask(CreateTaskDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = "Open",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AssignedUserId = userId
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return Ok(task);
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id && t.AssignedUserId == userId);
            if (task == null) return NotFound();
            if (task.AssignedUserId != userId) return Forbid();
            return Ok(task);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, TaskItem updatedTask)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id && t.AssignedUserId == userId);
            if (task == null) return NotFound();
            if (task.AssignedUserId != userId) return Forbid();
            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.Status = updatedTask.Status;
            task.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null) return NotFound();

            if (task.AssignedUserId != userId) return NotFound();

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return NoContent();
        }
    }
}