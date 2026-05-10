namespace TaskManagerAPI.DTOs
{
    public class TaskResponseDto
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
