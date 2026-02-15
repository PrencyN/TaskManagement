using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models.Enums;
using TaskManagementAPI.Models.Sorting;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _taskService.CreateTaskAsync(createTaskDto);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTasks(
            [FromQuery] TaskManagementStatus? status,
            [FromQuery] TaskPriority? priority,
            [FromQuery] TaskSortDto sort)
        {
            var sortOptions = sort.ToOptions();
            var tasks = await _taskService.GetAllTasksAsync(status, priority, sortOptions);
            return Ok(tasks);
        }

        private bool IsValidSortField(string sortBy)
        {
            var validFields = new[] { "created", "priority", "title", "status" };
            return validFields.Contains(sortBy.ToLower());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found", id);
                return NotFound($"Task with ID {id} not found");
            }
            return Ok(task);
        }

        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto updateStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTask = await _taskService.UpdateTaskStatusAsync(id, updateStatusDto.Status);
                if (updatedTask == null)
                {
                    return NotFound($"Task with ID {id} not found");
                }
                return Ok(updatedTask);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid status transition for task {TaskId}", id);
                return BadRequest(ex.Message);
            }
        }
    }
}