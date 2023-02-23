namespace Presentation.ControllerApi.Controllers;

using Application.Todos;
using Application.Todos.CompleteTodo;
using Application.Todos.CreateTodo;
using Application.Todos.DeleteTodo;
using Application.Todos.GetTodo;
using Application.Todos.GetTodos;

using MediatR;

using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
public class TodosController : ApiControllerBase
{
    private readonly IMediator mediator;

    public TodosController(IMediator mediator)
	{
        this.mediator = mediator;
    }

    [HttpGet(Name = "GetTodos")]
    public async Task<IActionResult> GetTodos(CancellationToken cancellationToken)
    {
        var response = await this.mediator.Send(new GetTodosQuery(), cancellationToken);

        return this.CreateActionResult(response);
    }

    [HttpGet("{id}", Name = "GetTodo")]
    public async Task<IActionResult> GetTodo(Guid id, CancellationToken cancellationToken)
    {
        var response = await this.mediator.Send(new GetTodoQuery(id), cancellationToken);

        return this.CreateActionResult(response);
    }

    [HttpPost(Name = "CreateTodo")]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoCommand createTodoCommand, CancellationToken cancellationToken)
    {
        var response = await this.mediator.Send(createTodoCommand, cancellationToken);

        return this.CreateActionResult(response, result => this.CreatedAtRoute("GetTodo", new { id = result.TodoId }, result));
    }

    [HttpPost("{id}/Complete", Name = "CompleteTodo")]
    public async Task<IActionResult> CompleteTodo(Guid id, CancellationToken cancellationToken)
    {
        var response = await this.mediator.Send(new CompleteTodoCommand(id), cancellationToken);

        return this.CreateActionResult(response);
    }

    [HttpDelete("{id}", Name = "DeleteTodo")]
    public async Task<IActionResult> DeleteTodo(Guid id, CancellationToken cancellationToken)
    {
        var response = await this.mediator.Send(new DeleteTodoCommand(id), cancellationToken);

        return this.CreateActionResult(response);
    }
}
