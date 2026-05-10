using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs
{
    public class UpdateTaskDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public Models.TaskStatus Status { get; set; }
    }
}
