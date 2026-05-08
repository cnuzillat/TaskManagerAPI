using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskManagerAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetTasksForUser(int userId)
        {
            return await _context.Tasks.Where(t => t.AssignedUserId == userId).ToListAsync();
        }

        public async Task<TaskItem?> GetTaskById(int taskId, int userId)
        {
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedUserId == userId);
        }

        public async Task<TaskItem> CreateTask(string title, string description, int userId)
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
            return task;
        }

        public async Task<TaskItem?> UpdateTask(int taskId, string title, string description, Models.TaskStatus status, int userId)
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
            return task;
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
    }
}