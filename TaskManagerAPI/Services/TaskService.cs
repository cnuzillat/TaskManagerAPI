using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskResponseDto>> GetTasksForUser(int userId)
        {
            var tasks = await _context.Tasks.Where(t => t.AssignedUserId == userId).ToListAsync();
            return tasks.Select(MapToResponseDto).ToList();
        }

        public async Task<TaskResponseDto?> GetTaskById(int taskId, int userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedUserId == userId);
            return task == null ? null : MapToResponseDto(task);
        }

        public async Task<TaskResponseDto> CreateTask(string title, string description, int userId)
        {
            var task = new TaskItem
            {
                Title = title,
                Description = description,
                Status = Models.TaskStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AssignedUserId = userId
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return MapToResponseDto(task);
        }

        public async Task<TaskResponseDto?> UpdateTask(int taskId, string title, string description, Models.TaskStatus status, int userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedUserId == userId);
            if (task == null) return null;
            if (!IsValidStatusTransition(task.Status, status))
                throw new InvalidOperationException($"Invalid status transition from {task.Status} to {status}");
            task.Title = title;
            task.Description = description;
            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return MapToResponseDto(task);
        }

        public async Task<bool> DeleteTask(int taskId, int userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedUserId == userId);
            if (task == null) return false;
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
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

        private TaskResponseDto MapToResponseDto(TaskItem task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}