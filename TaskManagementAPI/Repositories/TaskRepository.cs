using System.Collections.Concurrent;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ConcurrentDictionary<int, TaskItem> _tasks = new();
        private int _currentId = 0;

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            task.Id = Interlocked.Increment(ref _currentId);
            _tasks[task.Id] = task;
            return await Task.FromResult(task);
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await Task.FromResult(_tasks.Values.ToList());
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            _tasks.TryGetValue(id, out var task);
            return await Task.FromResult(task);
        }

        public async Task<TaskItem?> UpdateAsync(TaskItem task)
        {
            if (_tasks.TryGetValue(task.Id, out var existingTask))
            {
                _tasks[task.Id] = task;
                return await Task.FromResult(task);
            }
            return await Task.FromResult<TaskItem?>(null);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await Task.FromResult(_tasks.ContainsKey(id));
        }
    }
}