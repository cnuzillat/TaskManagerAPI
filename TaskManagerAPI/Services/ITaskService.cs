using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public interface ITaskService
    {
        List<TaskItem> GetTasksForUser(int userId);
        TaskItem? GetTaskById(int taskId, int userId);
        TaskItem CreateTask(string title, string description, int userId);
        TaskItem? UpdateTask(int taskId, string title, string description, Models.TaskStatus status, int userId);
        bool DeleteTask(int taskId, int userId);
    }
}
