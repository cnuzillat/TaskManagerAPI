using TaskManagerAPI.DTOs.Tasks;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public interface ITaskService
    {
        Task<List<TaskResponseDto>> GetTasksForUser(int userId);
        Task<TaskResponseDto?> GetTaskById(int taskId, int userId);
        Task<TaskResponseDto> CreateTask(string title, string description, int userId);
        Task<TaskResponseDto?> UpdateTask(int taskId, string title, string description, Models.TaskStatus status, int userId);
        Task<bool> DeleteTask(int taskId, int userId);
    }
}