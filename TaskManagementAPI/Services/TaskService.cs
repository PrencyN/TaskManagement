using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using TaskManagementAPI.Models.Enums;
using TaskManagementAPI.Models.Sorting;
using TaskManagementAPI.Repositories;

namespace TaskManagementAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository repository, ILogger<TaskService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Status = TaskManagementStatus.Pending,
                Priority = createTaskDto.Priority,
                CreatedAt = DateTime.UtcNow
            };

            var createdTask = await _repository.CreateAsync(task);
            _logger.LogInformation("Task created with ID: {TaskId}", createdTask.Id);

            return MapToDto(createdTask);
        }

        public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(
            TaskManagementStatus? status, 
            TaskPriority? priority,
            TaskSortOptions? sortOptions = null)
        {
            var tasks = await _repository.GetAllAsync();

            // Apply filters
            if (status.HasValue)
            {
                tasks = tasks.Where(t => t.Status == status.Value);
            }

            if (priority.HasValue)
            {
                tasks = tasks.Where(t => t.Priority == priority.Value);
            }

            // Apply sorting
            tasks = SortTasks(tasks, sortOptions ?? new TaskSortOptions());

            return tasks.Select(MapToDto);
        }

        private IEnumerable<TaskItem> SortTasks(IEnumerable<TaskItem> tasks, TaskSortOptions sortOptions)
        {
            var sortedTasks = sortOptions.SortBy switch
            {
                TaskSortField.Priority => SortByPriority(tasks),
                TaskSortField.Title => SortByTitle(tasks),
                TaskSortField.Status => SortByStatus(tasks),
                _ => SortByCreatedDate(tasks)
            };

            return sortOptions.SortDescending 
                ? sortedTasks.Reverse() 
                : sortedTasks;
        }

        private IEnumerable<TaskItem> SortByPriority(IEnumerable<TaskItem> tasks)
        {
            // Priority order: High > Medium > Low
            return tasks.OrderBy(t => t.Priority == TaskPriority.Low)
                    .ThenBy(t => t.Priority == TaskPriority.Medium)
                    .ThenBy(t => t.Priority == TaskPriority.High);
        }

        private IEnumerable<TaskItem> SortByTitle(IEnumerable<TaskItem> tasks)
        {
            return tasks.OrderBy(t => t.Title);
        }

        private IEnumerable<TaskItem> SortByStatus(IEnumerable<TaskItem> tasks)
        {
            // Status order: Pending > InProgress > Completed
            return tasks.OrderBy(t => t.Status == TaskManagementStatus.Pending)
                    .ThenBy(t => t.Status == TaskManagementStatus.InProgress)
                    .ThenBy(t => t.Status == TaskManagementStatus.Completed);
        }

        private IEnumerable<TaskItem> SortByCreatedDate(IEnumerable<TaskItem> tasks)
        {
            return tasks.OrderBy(t => t.CreatedAt);
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int id)
        {
            var task = await _repository.GetByIdAsync(id);
            return task != null ? MapToDto(task) : null;
        }

        public async Task<TaskResponseDto?> UpdateTaskStatusAsync(int id, TaskManagementStatus newStatus)
        {
            var task = await _repository.GetByIdAsync(id);
            if (task == null)
            {
                return null;
            }

            // Validate status transition
            if (!IsValidStatusTransition(task.Status, newStatus))
            {
                throw new InvalidOperationException($"Invalid status transition from {task.Status} to {newStatus}");
            }

            task.Status = newStatus;
            var updatedTask = await _repository.UpdateAsync(task);
            
            _logger.LogInformation("Task {TaskId} status updated from {OldStatus} to {NewStatus}", 
                id, task.Status, newStatus);

            return MapToDto(updatedTask!);
        }

        private bool IsValidStatusTransition(TaskManagementStatus currentStatus, TaskManagementStatus newStatus)
        {
            if (currentStatus == TaskManagementStatus.Completed)
            {
                return false; // Completed tasks cannot be updated
            }

            return (currentStatus, newStatus) switch
            {
                (TaskManagementStatus.Pending, TaskManagementStatus.InProgress) => true,
                (TaskManagementStatus.InProgress, TaskManagementStatus.Completed) => true,
                (TaskManagementStatus.Pending, TaskManagementStatus.Completed) => false, // Cannot skip InProgress
                (TaskManagementStatus.InProgress, TaskManagementStatus.Pending) => false, // Cannot go back
                _ => currentStatus == newStatus // Allow same status
            };
        }

        private TaskResponseDto MapToDto(TaskItem task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                CreatedAt = task.CreatedAt
            };
        }
    }
}