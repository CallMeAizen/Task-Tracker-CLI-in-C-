using System.Text.Json;

namespace TaskTrackerCli;

public sealed class TaskRepository
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public TaskRepository(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(Directory.GetCurrentDirectory(), "tasks.json");
    }

    public IReadOnlyList<TaskItem> GetAll()
    {
        return LoadTasks();
    }

    public TaskItem Add(string description)
    {
        var tasks = LoadTasks();
        var now = DateTimeOffset.UtcNow;

        var task = new TaskItem
        {
            Id = tasks.Count == 0 ? 1 : tasks.Max(existingTask => existingTask.Id) + 1,
            Description = description,
            Status = TaskStatuses.Todo,
            CreatedAt = now,
            UpdatedAt = now
        };

        tasks.Add(task);
        SaveTasks(tasks);
        return task;
    }

    public TaskItem? Update(int id, string description)
    {
        var tasks = LoadTasks();
        var task = tasks.FirstOrDefault(existingTask => existingTask.Id == id);

        if (task is null)
        {
            return null;
        }

        task.Description = description;
        task.UpdatedAt = DateTimeOffset.UtcNow;
        SaveTasks(tasks);
        return task;
    }

    public bool Delete(int id)
    {
        var tasks = LoadTasks();
        var removedCount = tasks.RemoveAll(existingTask => existingTask.Id == id);

        if (removedCount == 0)
        {
            return false;
        }

        SaveTasks(tasks);
        return true;
    }

    public TaskItem? UpdateStatus(int id, string status)
    {
        var tasks = LoadTasks();
        var task = tasks.FirstOrDefault(existingTask => existingTask.Id == id);

        if (task is null)
        {
            return null;
        }

        task.Status = status;
        task.UpdatedAt = DateTimeOffset.UtcNow;
        SaveTasks(tasks);
        return task;
    }

    private List<TaskItem> LoadTasks()
    {
        EnsureStorageExists();

        var json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<TaskItem>>(json, _serializerOptions) ?? [];
    }

    private void SaveTasks(List<TaskItem> tasks)
    {
        EnsureStorageExists();
        var json = JsonSerializer.Serialize(tasks, _serializerOptions);
        File.WriteAllText(_filePath, json);
    }

    private void EnsureStorageExists()
    {
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }
}
