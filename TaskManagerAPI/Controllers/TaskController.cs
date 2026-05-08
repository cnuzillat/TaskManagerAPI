using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
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

            var tasks = _taskService.GetTasksForUser(userId);

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

            var createdTask = _taskService.CreateTask(dto.Title, dto.Description, userId);

            return Ok(createdTask);
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = int.Parse(userIdClaim.Value);
            var task = _taskService.GetTaskById(id, userId);
            if (task == null) return NotFound();
            if (task.AssignedUserId != userId) return Forbid();
            return Ok(task);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            try
            {
                var updatedTask = _taskService.UpdateTask(id, dto.Title, dto.Description, dto.Status, userId);

                if (updatedTask == null) return NotFound();

                return Ok(updatedTask);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var deleted = _taskService.DeleteTask(id, userId);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}