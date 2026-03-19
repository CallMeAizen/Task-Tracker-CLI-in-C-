using System.Text.Json;

namespace TaskTrackerCli;

internal static class Program
{
    private static int Main(string[] args)
    {
        var repository = new TaskRepository();

        try
        {
            return Run(args, repository);
        }
        catch (JsonException)
        {
            Console.Error.WriteLine("Unable to read tasks.json because it is not valid JSON.");
            Console.Error.WriteLine("Fix the file contents or delete it so the app can recreate it.");
            return 1;
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"A file system error occurred: {ex.Message}");
            return 1;
        }
    }

    private static int Run(string[] args, TaskRepository repository)
    {
        if (args.Length == 0)
        {
            return PrintUsage("A command is required.");
        }

        var command = args[0].Trim().ToLowerInvariant();

        return command switch
        {
            "add" => AddTask(args, repository),
            "update" => UpdateTask(args, repository),
            "delete" => DeleteTask(args, repository),
            "mark-in-progress" => MarkTask(args, repository, TaskStatuses.InProgress),
            "mark-done" => MarkTask(args, repository, TaskStatuses.Done),
            "list" => ListTasks(args, repository),
            "help" or "--help" or "-h" => PrintUsage(),
            _ => PrintUsage($"Unknown command '{args[0]}'.")
        };
    }

    private static int AddTask(string[] args, TaskRepository repository)
    {
        if (args.Length < 2)
        {
            return PrintUsage("The add command requires a description.");
        }

        var description = JoinDescription(args, 1);
        if (description is null)
        {
            return PrintError("Task description cannot be empty.");
        }

        var task = repository.Add(description);
        Console.WriteLine($"Task added successfully (ID: {task.Id})");
        return 0;
    }

    private static int UpdateTask(string[] args, TaskRepository repository)
    {
        if (args.Length < 3)
        {
            return PrintUsage("The update command requires an ID and a description.");
        }

        if (!TryParseId(args[1], out var id))
        {
            return PrintError("Task ID must be a positive integer.");
        }

        var description = JoinDescription(args, 2);
        if (description is null)
        {
            return PrintError("Task description cannot be empty.");
        }

        var updatedTask = repository.Update(id, description);
        if (updatedTask is null)
        {
            return PrintError($"Task with ID {id} was not found.");
        }

        Console.WriteLine("Task updated successfully");
        return 0;
    }

    private static int DeleteTask(string[] args, TaskRepository repository)
    {
        if (args.Length != 2)
        {
            return PrintUsage("The delete command requires exactly one task ID.");
        }

        if (!TryParseId(args[1], out var id))
        {
            return PrintError("Task ID must be a positive integer.");
        }

        if (!repository.Delete(id))
        {
            return PrintError($"Task with ID {id} was not found.");
        }

        Console.WriteLine("Task deleted successfully");
        return 0;
    }

    private static int MarkTask(string[] args, TaskRepository repository, string status)
    {
        if (args.Length != 2)
        {
            return PrintUsage($"The {args[0]} command requires exactly one task ID.");
        }

        if (!TryParseId(args[1], out var id))
        {
            return PrintError("Task ID must be a positive integer.");
        }

        var updatedTask = repository.UpdateStatus(id, status);
        if (updatedTask is null)
        {
            return PrintError($"Task with ID {id} was not found.");
        }

        Console.WriteLine($"Task marked as {status} successfully");
        return 0;
    }

    private static int ListTasks(string[] args, TaskRepository repository)
    {
        if (args.Length > 2)
        {
            return PrintUsage("The list command accepts at most one optional status filter.");
        }

        IEnumerable<TaskItem> tasks = repository.GetAll();

        if (args.Length == 2 && !TryApplyListFilter(args[1], tasks, out tasks))
        {
            return PrintError("Status filter must be one of: todo, in-progress, done, not-done.");
        }

        var taskList = tasks.OrderBy(task => task.Id).ToList();
        if (taskList.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return 0;
        }

        foreach (var task in taskList)
        {
            Console.WriteLine($"[{task.Id}] {task.Description}");
            Console.WriteLine($"Status: {task.Status}");
            Console.WriteLine($"Created: {task.CreatedAt:O}");
            Console.WriteLine($"Updated: {task.UpdatedAt:O}");
            Console.WriteLine();
        }

        return 0;
    }

    private static bool TryApplyListFilter(string rawFilter, IEnumerable<TaskItem> tasks, out IEnumerable<TaskItem> filteredTasks)
    {
        var filter = rawFilter.Trim().ToLowerInvariant();

        switch (filter)
        {
            case TaskStatuses.Todo:
                filteredTasks = tasks.Where(task => task.Status == TaskStatuses.Todo);
                return true;
            case TaskStatuses.InProgress:
                filteredTasks = tasks.Where(task => task.Status == TaskStatuses.InProgress);
                return true;
            case TaskStatuses.Done:
                filteredTasks = tasks.Where(task => task.Status == TaskStatuses.Done);
                return true;
            case "not-done":
                filteredTasks = tasks.Where(task => task.Status != TaskStatuses.Done);
                return true;
            default:
                filteredTasks = Array.Empty<TaskItem>();
                return false;
        }
    }

    private static bool TryParseId(string rawId, out int id)
    {
        return int.TryParse(rawId, out id) && id > 0;
    }

    private static string? JoinDescription(string[] args, int startIndex)
    {
        var description = string.Join(" ", args.Skip(startIndex)).Trim();
        return string.IsNullOrWhiteSpace(description) ? null : description;
    }

    private static int PrintUsage(string? error = null)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.Error.WriteLine(error);
            Console.Error.WriteLine();
        }

        Console.WriteLine("Usage:");
        Console.WriteLine("  add \"task description\"");
        Console.WriteLine("  update <id> \"task description\"");
        Console.WriteLine("  delete <id>");
        Console.WriteLine("  mark-in-progress <id>");
        Console.WriteLine("  mark-done <id>");
        Console.WriteLine("  list [todo|in-progress|done|not-done]");
        Console.WriteLine("  help");
        return error is null ? 0 : 1;
    }

    private static int PrintError(string message)
    {
        Console.Error.WriteLine(message);
        return 1;
    }
}
