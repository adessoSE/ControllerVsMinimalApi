namespace Presentation.MinimalApi.net7.CleanArchitecture.Endpoints;

using Application.Todos.CompleteTodo;
using Application.Todos.CreateTodo;
using Application.Todos.DeleteTodo;
using Application.Todos.GetTodo;
using Application.Todos.GetTodos;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using Presentation.MinimalApi.Common.net7;
using Presentation.MinimalApi.Common.net7.EndpointMapper;

internal class TodoEndpoints : IEndpointMapper
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todos");

        group.MapGet("", GetTodos).WithName(nameof(GetTodos));
        group.MapGet("{id}", GetTodo).WithName(nameof(GetTodo));
        group.MapPost("", CreateTodo).WithName(nameof(CreateTodo));
        group.MapPost("{id}/complete", CompleteTodo).WithName(nameof(CompleteTodo));
        group.MapDelete("{id}", DeleteTodo).WithName(nameof(DeleteTodo));
    }

    //public static IEndpointRouteBuilder MapTodoEndpoints(this IEndpointRouteBuilder app)
    //{
    //    MapEndpoints(app);

    //    return app;
    //}


    public static async Task<TypedApplicationResult<Ok<GetTodosQueryResponse[]>>> GetTodos(IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTodosQuery(), cancellationToken);

        return response.ToTypedApiResult();
    }

    public static async Task<TypedApplicationResult<Ok<GetTodoQueryResponse>>> GetTodo(IMediator mediator, Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTodoQuery(id), cancellationToken);

        return response.ToTypedApiResult();
    }

    public static async Task<TypedApplicationResult<CreatedAtRoute<CreateTodoCommandResponse>>> CreateTodo(IMediator mediator, CreateTodoCommand createTodoCommand, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(createTodoCommand, cancellationToken);

        return response.ToTypedApiResult(result => TypedResults.CreatedAtRoute(result, nameof(GetTodo), new { id = result.TodoId }));
    }

    public static async Task<TypedApplicationResult<Ok>> CompleteTodo(IMediator mediator, Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CompleteTodoCommand(id), cancellationToken);

        return response.ToTypedApiResult();
    }

    public static async Task<TypedApplicationResult<Ok>> DeleteTodo(IMediator mediator, Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteTodoCommand(id), cancellationToken);

        return response.ToTypedApiResult();
    }
}