using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.Sorting;

namespace TaskManagementAPI.DTOs
{
    public class TaskSortDto
    {
        /// <summary>
        /// Field to sort by. Available values: created, priority, title, status
        /// </summary>
        [FromQuery(Name = "sortBy")]
        [DefaultValue(TaskSortField.CreatedAt)]
        public TaskSortField SortBy { get; set; } = TaskSortField.CreatedAt;

        /// <summary>
        /// Sort direction. true for descending (default), false for ascending
        /// </summary>
        [FromQuery(Name = "sortDescending")]
        [DefaultValue(true)]
        public bool SortDescending { get; set; } = true;

        public TaskSortOptions ToOptions()
        {
            return new TaskSortOptions
            {
                SortBy = SortBy,
                SortDescending = SortDescending
            };
        }
    }
}