namespace TaskManagementAPI.Models.Sorting
{
    public class TaskSortOptions
    {
        public TaskSortField SortBy { get; set; } = TaskSortField.CreatedAt;
        public bool SortDescending { get; set; } = true;
    }
}