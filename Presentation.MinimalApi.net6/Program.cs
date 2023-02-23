using Application.Todos;

using Presentation.MinimalApi.Common;

var app = WebAppBuilder.BuildApp(args);

app.MapGet("/todos", async (ITodoRepository todoRepository, CancellationToken cancellationToken) =>
{
    var todos = await todoRepository.GetTodosAsync(cancellationToken);

    return todos.Select(todo => 
        new TodoListItemDto(todo.Id,
                            todo.Title,
                            todo.Description,
                            todo.IsCompleted,
                            todo.UpdatedAt));
    })
   .WithName("GetTodos");

app.MapGet("/todos/{id}", async (ITodoRepository todoRepository, Guid id, CancellationToken cancellationToken) =>
    await todoRepository.GetTodoByIdAsync(id, cancellationToken))
   .WithName("GetTodo");

app.MapPost("/todos", async (ITodoRepository todoRepository, CreateTodoDto todoDto, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrEmpty(todoDto.Title))
    {
        return Results.BadRequest("The title is required.");
    }

    if (string.IsNullOrEmpty(todoDto.Title)
        || todoDto.Title.Length is < 5 or > 20)
    {
        return Results.BadRequest("The title must be between 5 and 20 characters.");
    }

    if (string.IsNullOrEmpty(todoDto.Description))
    {
        return Results.BadRequest("The description is required.");
    }

    if (todoDto.Description.Length > 100)
    {
        return Results.BadRequest("The description must be less than 100 characters.");
    }

    var todo = new Todo(todoDto.Title, todoDto.Description);

    await todoRepository.AddTodoAsync(todo, cancellationToken);

    return Results.CreatedAtRoute("GetTodo", new { id = todo.Id }, todo);
}).WithName("CreateTodo");

app.MapPost("/todos/{id}/complete", async (ITodoRepository todoRepository, Guid id, CancellationToken cancellationToken) =>
{
    var todo = await todoRepository.GetTodoByIdAsync(id, cancellationToken);

    if (todo is null)
    {
        return Results.NotFound();
    }

    if (todo.IsCompleted)
    {
        return Results.Conflict($"The todo {todo.Id} is already completed.");
    }

    todo.Complete();

    await todoRepository.UpdateTodoAsync(todo, cancellationToken);

    return Results.Ok();
}).WithName("CompleteTodo");

app.MapDelete("/todos/{id}", async (ITodoRepository todoRepository, Guid id, CancellationToken cancellationToken) =>
    await todoRepository.DeleteTodoAsync(id, cancellationToken))
   .WithName("DeleteTodo");

app.Run();

public sealed record CreateTodoDto(string Title, string Description);
public sealed record TodoListItemDto(Guid Id, string Title, string Description, bool IsCompleted, DateTime UpdatedAt);