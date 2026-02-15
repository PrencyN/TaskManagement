using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Models.Enums;

namespace TaskManagementAPI.DTOs
{
    public class UpdateTaskStatusDto
    {
        [Required]
        public TaskManagementStatus Status { get; set; }
    }
}