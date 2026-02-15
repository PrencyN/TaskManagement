using Moq;
using Xunit;
using TaskManagementAPI.Models;
using TaskManagementAPI.Models.Enums;
using TaskManagementAPI.Repositories;
using TaskManagementAPI.Services;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models.Sorting;
using Microsoft.Extensions.Logging;

namespace TaskManagementAPI.Tests
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockRepository;
        private readonly Mock<ILogger<TaskService>> _mockLogger;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            _mockRepository = new Mock<ITaskRepository>();
            _mockLogger = new Mock<ILogger<TaskService>>();
            _service = new TaskService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_ValidTransition_ShouldUpdateStatus()
        {
            // Arrange
            var taskId = 1;
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "desc",
                Status = TaskManagementStatus.Pending,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem t) => t);

            // Act
            var result = await _service.UpdateTaskStatusAsync(taskId, TaskManagementStatus.InProgress);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("InProgress", result.Status);
        }

        [Fact]
        public async Task UpdateTaskStatusAsync_InvalidTransition_ShouldThrowException()
        {
            // Arrange
            var taskId = 1;
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "desc",
                Status = TaskManagementStatus.Completed,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateTaskStatusAsync(taskId, TaskManagementStatus.Pending));
        }

        [Fact]
        public async Task GetAllTasksAsync_ShouldReturnFilteredAndSortedTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "A", Description = "", Status = TaskManagementStatus.Pending, Priority = TaskPriority.High, CreatedAt = DateTime.UtcNow.AddMinutes(-10) },
                new TaskItem { Id = 2, Title = "B", Description = "", Status = TaskManagementStatus.InProgress, Priority = TaskPriority.Low, CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
                new TaskItem { Id = 3, Title = "C", Description = "", Status = TaskManagementStatus.Pending, Priority = TaskPriority.Medium, CreatedAt = DateTime.UtcNow }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(tasks);

            var sortOptions = new TaskSortOptions
            {
                SortBy = TaskSortField.Priority,
                SortDescending = false
            };

            // Act
            var result = await _service.GetAllTasksAsync(TaskManagementStatus.Pending, null, sortOptions);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, t => Assert.Equal("Pending", t.Status));
            Assert.Equal("A", resultList[1].Title); // High priority should be last in ascending
            Assert.Equal("C", resultList[0].Title); // Medium priority should be first in ascending
        }

        [Fact]
        public async Task GetTaskByIdAsync_TaskExists_ReturnsTaskResponseDto()
        {
            // Arrange
            var taskId = 1;
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "desc",
                Status = TaskManagementStatus.Pending,
                Priority = TaskPriority.High,
                CreatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync(task);

            // Act
            var result = await _service.GetTaskByIdAsync(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            Assert.Equal("Test Task", result.Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_TaskDoesNotExist_ReturnsNull()
        {
            // Arrange
            var taskId = 99;
            _mockRepository.Setup(r => r.GetByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _service.GetTaskByIdAsync(taskId);

            // Assert
            Assert.Null(result);
        }
    }
}