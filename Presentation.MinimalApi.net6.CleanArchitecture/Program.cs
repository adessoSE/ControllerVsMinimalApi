using Application.Todos;
using Application.Todos.CompleteTodo;
using Application.Todos.CreateTodo;
using Application.Todos.DeleteTodo;
using Application.Todos.GetTodo;
using Application.Todos.GetTodos;

using MediatR;

using Presentation.MinimalApi.Common;

var app = WebAppBuilder.BuildApp(args);

app.MapGet("/todos", async (IMediator mediator, CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new GetTodosQuery(), cancellationToken);

    return response.ToApiResult();
}).WithName("GetTodos");

app.MapGet("/todos/{id}", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new GetTodoQuery(id), cancellationToken);

    return response.ToApiResult();
}).WithName("GetTodo");

app.MapPost("/todos", async (IMediator mediator, CreateTodoCommand createTodoCommand, CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(createTodoCommand, cancellationToken);

    return response.ToApiResult(result => Results.CreatedAtRoute("GetTodo", new { id = result.TodoId }, result));
}).WithName("CreateTodo");

app.MapPost("/todos/{id}/complete", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new CompleteTodoCommand(id), cancellationToken);

    return response.ToApiResult();
}).WithName("CompleteTodo");

app.MapDelete("/todos/{id}", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new DeleteTodoCommand(id), cancellationToken);

    return response.ToApiResult();
}).WithName("DeleteTodo");

app.Run();