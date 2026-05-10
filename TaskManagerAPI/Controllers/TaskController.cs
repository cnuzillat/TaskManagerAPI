using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManagerAPI.Services;
using TaskManagerAPI.DTOs;

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

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var tasks = await _taskService.GetTasksForUser(userId.Value);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var createdTask = await _taskService.CreateTask(dto.Title, dto.Description, userId.Value);

            return CreatedAtAction(nameof(GetTaskById), new { createdTask.Id }, createdTask);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var task = await _taskService.GetTaskById(id, userId.Value);
            if (task == null) return NotFound();

            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var updatedTask = await _taskService.UpdateTask(id, dto.Title, dto.Description, dto.Status, userId.Value);
            if (updatedTask == null) return NotFound();

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var deleted = await _taskService.DeleteTask(id, userId.Value);
            if (!deleted) return NotFound();

            return NoContent();
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                return null;

            return int.Parse(claim.Value);
        }
    }
}