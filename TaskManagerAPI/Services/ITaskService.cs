using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetTasksForUser(int userId);
        Task<TaskItem?> GetTaskById(int taskId, int userId);
        Task<TaskItem> CreateTask(string title, string description, int userId);
        Task<TaskItem?> UpdateTask(int taskId, string title, string description, Models.TaskStatus status, int userId);
        Task<bool> DeleteTask(int taskId, int userId);
    }
}