using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Models.Enums;

namespace TaskManagementAPI.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    }
}