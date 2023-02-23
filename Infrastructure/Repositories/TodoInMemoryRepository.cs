namespace Infrastructure.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Application.Todos;

internal class TodoInMemoryRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, Todo> todos = new();

    public Task AddTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        this.todos.TryAdd(todo.Id, todo);

        return Task.CompletedTask;
    }

    public Task DeleteTodoAsync(Guid id, CancellationToken cancellationToken)
    {
        this.todos.TryRemove(id, out _);

        return Task.CompletedTask;
    }

    public Task<Todo?> GetTodoByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        this.todos.TryGetValue(id, out var todo);

        return Task.FromResult(todo);
    }

    public Task<IEnumerable<Todo>> GetTodosAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<Todo>>(this.todos.Values);
    }

    public Task UpdateTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        this.todos.AddOrUpdate(todo.Id, todo, (key, existing) => todo);

        return Task.CompletedTask;
    }
}
