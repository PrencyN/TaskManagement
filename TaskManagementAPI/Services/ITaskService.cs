using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models.Enums;
using TaskManagementAPI.Models.Sorting;

namespace TaskManagementAPI.Services
{
    public interface ITaskService
    {
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(
            TaskManagementStatus? status, 
            TaskPriority? priority,
            TaskSortOptions? sortOptions = null);
        Task<TaskResponseDto?> GetTaskByIdAsync(int id);
        Task<TaskResponseDto?> UpdateTaskStatusAsync(int id, TaskManagementStatus newStatus);
    }
}