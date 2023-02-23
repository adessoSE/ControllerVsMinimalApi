namespace Application.Todos.GetTodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Application.CQRS;
using Application.Results;

internal class GetTodoQueryHandler : IQueryHandler<GetTodoQuery, GetTodoQueryResponse>
{
    private readonly ITodoRepository todoRepository;

    public GetTodoQueryHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result<GetTodoQueryResponse>> Handle(GetTodoQuery request, CancellationToken cancellationToken)
    {
        var todo = await this.todoRepository.GetTodoByIdAsync(request.TodoId, cancellationToken);

        if (todo is null)
        {
            return Error.NotFound;
        }

        return new GetTodoQueryResponse(todo);
    }
}
