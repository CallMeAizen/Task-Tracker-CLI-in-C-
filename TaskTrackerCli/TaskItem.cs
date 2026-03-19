namespace TaskTrackerCli;

public sealed class TaskItem
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = TaskStatuses.Todo;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
