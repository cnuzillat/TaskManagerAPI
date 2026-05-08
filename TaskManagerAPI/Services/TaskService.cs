using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public List<TaskItem> GetTasksForUser(int userId)
        {
            return _context.Tasks.Where(t => t.AssignedUserId == userId).ToList();
        }

        public TaskItem? GetTaskById(int taskId, int userId)
        {
            return _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.AssignedUserId == userId);
        }

        public TaskItem CreateTask(string title, string description, int userId)
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
            _context.SaveChanges();
            return task;
        }

        public TaskItem? UpdateTask(int taskId, string title, string description, Models.TaskStatus status, int userId)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.AssignedUserId == userId);
            if (task == null) return null;
            if (!IsValidStatusTransition(task.Status, status))
                throw new InvalidOperationException($"Invalid status transition from {task.Status} to {status}");
            task.Title = title;
            task.Description = description;
            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();
            return task;
        }

        public bool DeleteTask(int taskId, int userId)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.AssignedUserId == userId);
            if (task == null) return false;
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return true;
        }

        public bool IsValidStatusTransition(Models.TaskStatus currentStatus, Models.TaskStatus newStatus)
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
