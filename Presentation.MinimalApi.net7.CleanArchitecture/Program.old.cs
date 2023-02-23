//using Application.Todos;
//using Application.Todos.CompleteTodo;
//using Application.Todos.CreateTodo;
//using Application.Todos.DeleteTodo;
//using Application.Todos.GetTodo;
//using Application.Todos.GetTodos;

//using MediatR;

//using Presentation.MinimalApi.Common;
//using Presentation.MinimalApi.net7.CleanArchitecture;

//var app = WebAppBuilder.BuildApp(args);
//var group = app.MapGroup("/todos");

//group.MapGet("", async (IMediator mediator, CancellationToken cancellationToken) =>
//{
//    var response = await mediator.Send(new GetTodosQuery(), cancellationToken);

//    return response.ToTypedApiResult();
//}).WithName("GetTodos");

//group.MapGet("{id}", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
//{
//    var response = await mediator.Send(new GetTodoQuery(id), cancellationToken);

//    return response.ToTypedApiResult();
//}).WithName("GetTodo");

//group.MapPost("", async (IMediator mediator, CreateTodoCommand createTodoCommand, CancellationToken cancellationToken) =>
//{
//    var response = await mediator.Send(createTodoCommand, cancellationToken);

//    return response.ToTypedApiResult(result => TypedResults.CreatedAtRoute(result, "GetTodo", new { id = result.TodoId }));
//}).WithName("CreateTodo");

//group.MapPost("{id}/complete", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
//{
//    var response = await mediator.Send(new CompleteTodoCommand(id), cancellationToken);

//    return response.ToTypedApiResult();
//}).WithName("CompleteTodo");

//group.MapDelete("{id}", async (IMediator mediator, Guid id, CancellationToken cancellationToken) =>
//{
//    var response = await mediator.Send(new DeleteTodoCommand(id), cancellationToken);

//    return response.ToTypedApiResult();
//}).WithName("DeleteTodo");

//app.Run();