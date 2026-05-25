namespace TaskManagerAPI.DTOs.Tasks
{
    public class TaskQueryParametersDto
    {
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public bool Descending { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
