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

        public class UpdateTaskDto
        {
            [Required]
            public string Title { get; set; }

            [Required]
            public string Description { get; set; }

            [Required]
            public Models.TaskStatus Status { get; set; }
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
                Status = Models.TaskStatus.Open,
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
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();
            if (task.AssignedUserId != userId) return Forbid();
            return Ok(task);
        }

        private bool IsValidStatusTransition(Models.TaskStatus currentStatus, Models.TaskStatus newStatus)
        {
            if (currentStatus == newStatus) return true;

            return currentStatus switch
            {
                Models.TaskStatus.Open => newStatus == Models.TaskStatus.InProgress,
                Models.TaskStatus.InProgress => newStatus == Models.TaskStatus.Completed,
                Models.TaskStatus.Completed => false,
                _ => false
            };
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null) 
                return NotFound();

            if (task.AssignedUserId != userId) 
                return Forbid();

            if (!IsValidStatusTransition(task.Status, dto.Status))
            {
                return BadRequest($"Invalid status transition from '{task.Status}' to '{dto.Status}'");
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
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

            if (task == null) return Forbid();

            if (task.AssignedUserId != userId) return Forbid();

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return NoContent();
        }
    }
}