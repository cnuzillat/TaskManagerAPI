using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.Tasks
{
    public class CreateTaskDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
