# Task Tracker CLI in C# #

A simple command-line task tracker built in C# as an implementation of the [Task Tracker project from roadmap.sh](https://roadmap.sh/projects/task-tracker).

The application lets you:

- Add tasks
- Update tasks
- Delete tasks
- Mark tasks as `in-progress`
- Mark tasks as `done`
- List all tasks
- Filter tasks by status

Tasks are stored in a `tasks.json` file in the current working directory. If the file does not exist, the application creates it automatically.

## Features ##

- Built with C# and .NET
- Uses positional command-line arguments
- Stores data in JSON
- Uses only native .NET libraries
- Handles common errors gracefully

## Task Properties ##

Each task contains:

- `id`
- `description`
- `status`
- `createdAt`
- `updatedAt`

Supported status values:

- `todo`
- `in-progress`
- `done`

## Project Structure ##

```text
TaskTrackerCli/
├── README.md
├── Program.cs
├── TaskItem.cs
├── TaskRepository.cs
├── TaskStatuses.cs
└── TaskTrackerCli.csproj
```

## Requirements ##

- .NET SDK 10.0 or later

Check your installed version:

```bash
dotnet --version
```

## Build the Project ##

From the repository root, run:

```bash
dotnet build TaskTrackerCli/TaskTrackerCli.csproj
```

Or from inside the `TaskTrackerCli` folder:

```bash
dotnet build
```

## Run the Project ##

Run commands from the repository root with:

```bash
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- <command>
```

Or from inside the `TaskTrackerCli` folder:

```bash
dotnet run -- <command>
```

Examples:

```bash
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- add "Buy groceries"
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- update 1 "Buy groceries and cook dinner"
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- mark-in-progress 1
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- mark-done 1
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list done
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list todo
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list in-progress
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list not-done
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- delete 1
```

## Available Commands ##

```bash
add "task description"
update <id> "new task description"
delete <id>
mark-in-progress <id>
mark-done <id>
list
list done
list todo
list in-progress
list not-done
help
```

## How Storage Works ##

The application saves tasks to a file named `tasks.json` in the directory where you run the command.

For example:

- If you run the app from the repository root, `tasks.json` will be created in the repository root.
- If you run the compiled DLL from another folder, `tasks.json` will be created in that folder instead.

Example JSON structure:

```json
[
  {
    "id": 1,
    "description": "Buy groceries",
    "status": "todo",
    "createdAt": "2026-03-18T17:09:54.4351267+00:00",
    "updatedAt": "2026-03-18T17:09:54.4351267+00:00"
  }
]
```

## Testing the Project ##

There is currently no automated test project in the repository, so testing is done by building the app and running manual CLI checks.

### 1. Build ##

```bash
dotnet build TaskTrackerCli/TaskTrackerCli.csproj
```

### 2. Run a manual smoke test ##

```bash
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- add "Buy groceries"
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- update 1 "Buy groceries and cook dinner"
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- mark-in-progress 1
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list in-progress
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- mark-done 1
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list done
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- delete 1
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list
```

Expected behavior:

- The first command adds a new task and prints its ID
- `list` shows the saved task
- `update` changes the task description
- `mark-in-progress` updates the status
- `mark-done` updates the status again
- `delete` removes the task
- The final `list` prints `No tasks found.`

### 3. Verify error handling ##

Examples:

```bash
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- update 99 "Missing task"
dotnet run --project TaskTrackerCli/TaskTrackerCli.csproj -- list invalid
```

Expected behavior:

- Updating a missing task should print that the task was not found
- An invalid filter should print the allowed status values

## Notes ##

- The project uses only built-in .NET APIs.
- No external libraries or frameworks are required.
- `tasks.json` is created automatically when needed.

## License ##

This project is provided for learning and portfolio purposes.
