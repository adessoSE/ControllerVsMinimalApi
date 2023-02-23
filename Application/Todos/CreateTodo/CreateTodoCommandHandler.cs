namespace Application.Todos.CreateTodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Application.CQRS;
using Application.Results;

internal class CreateTodoCommandHandler : ICommandHandler<CreateTodoCommand, CreateTodoCommandResponse>
{
    private readonly ITodoRepository todoRepository;

    public CreateTodoCommandHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result<CreateTodoCommandResponse>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new Todo(request.Title, request.Description);

        await this.todoRepository.AddTodoAsync(todo, cancellationToken);

        return new CreateTodoCommandResponse(todo.Id);
    }
}
