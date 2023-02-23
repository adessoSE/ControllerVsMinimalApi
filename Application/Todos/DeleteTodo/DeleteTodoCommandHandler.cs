namespace Application.Todos.DeleteTodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Application.CQRS;
using Application.Results;

internal class DeleteTodoCommandHandler : ICommandHandler<DeleteTodoCommand>
{
    private readonly ITodoRepository todoRepository;

    public DeleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        await this.todoRepository.DeleteTodoAsync(request.TodoId, cancellationToken);

        return Result.Success();
    }
}
