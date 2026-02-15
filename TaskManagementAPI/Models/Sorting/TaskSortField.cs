using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TaskManagementAPI.Models.Sorting
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskSortField
    {
        [EnumMember(Value = "created")]
        [Description("Sort by creation date")]
        CreatedAt,

        [EnumMember(Value = "priority")]
        [Description("Sort by priority (High > Medium > Low)")]
        Priority,

        [EnumMember(Value = "title")]
        [Description("Sort by title alphabetically")]
        Title,

        [EnumMember(Value = "status")]
        [Description("Sort by status (Pending > InProgress > Completed)")]
        Status
    }
}