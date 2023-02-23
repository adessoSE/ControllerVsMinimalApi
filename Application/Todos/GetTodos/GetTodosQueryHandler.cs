namespace Application.Todos.GetTodos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Application.CQRS;
using Application.Results;

internal class GetTodosQueryHandler : IQueryHandler<GetTodosQuery, GetTodosQueryResponse[]>
{
    private readonly ITodoRepository todoRepository;

    public GetTodosQueryHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result<GetTodosQueryResponse[]>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var todos = await this.todoRepository.GetTodosAsync(cancellationToken);

        var result = todos
            .Select(todo => new GetTodosQueryResponse(todo.Id, todo.Title, todo.Description, todo.IsCompleted, todo.UpdatedAt))
            .ToArray();

        return result;
    }
}
