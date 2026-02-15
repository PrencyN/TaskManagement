using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models.Enums;

namespace TaskManagementAPI.Services
{
    public interface ITaskService
    {
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(TaskManagementStatus? status, TaskPriority? priority);
        Task<TaskResponseDto?> GetTaskByIdAsync(int id);
        Task<TaskResponseDto?> UpdateTaskStatusAsync(int id, TaskManagementStatus newStatus);
    }
}