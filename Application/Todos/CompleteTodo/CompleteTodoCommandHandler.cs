namespace Application.Todos.CompleteTodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Application.CQRS;
using Application.Results;

internal class CompleteTodoCommandHandler : ICommandHandler<CompleteTodoCommand>
{
    private readonly ITodoRepository todoRepository;

    public CompleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result> Handle(CompleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await this.todoRepository.GetTodoByIdAsync(request.TodoId, cancellationToken);

        if (todo is null)
        {
            return Error.NotFound;
        }

        if (todo.IsCompleted)
        {
            return BusinessError.Conflict("Todo.AlreadyCompleted", $"The todo {todo.Id} is already completed.");
        }

        todo.Complete();

        await this.todoRepository.UpdateTodoAsync(todo, cancellationToken);

        return Result.Success();
    }
}
