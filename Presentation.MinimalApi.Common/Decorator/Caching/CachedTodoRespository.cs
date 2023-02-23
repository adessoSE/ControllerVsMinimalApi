namespace Presentation.MinimalApi.Common.Decorator.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Application.Todos;

using Microsoft.Extensions.Caching.Memory;

internal class CachedTodoRespository : ITodoRepository
{
    private readonly ITodoRepository todoRepository;
    private readonly IMemoryCache cache;

    public CachedTodoRespository(ITodoRepository todoRepository, IMemoryCache cache)
    {
        this.todoRepository = todoRepository;
        this.cache = cache;
    }

    public Task AddTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        this.cache.CreateEntry(todo.Id)
            .SetValue(todo)
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

        // Invalidate example:
        this.cache.Remove("todos");

        return this.todoRepository.AddTodoAsync(todo, cancellationToken);
    }

    public Task DeleteTodoAsync(Guid id, CancellationToken cancellationToken)
    {
        return this.todoRepository.DeleteTodoAsync(id, cancellationToken);
    }

    public Task<Todo?> GetTodoByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return this.cache.GetOrCreateAsync(id, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return await this.todoRepository.GetTodoByIdAsync(id, cancellationToken);
        });
    }

    public Task<IEnumerable<Todo>> GetTodosAsync(CancellationToken cancellationToken)
    {
        return this.cache.GetOrCreateAsync("todos", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return await this.todoRepository.GetTodosAsync(cancellationToken);
        })!;
    }

    public Task UpdateTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        this.cache.Remove(todo.Id);

        return this.todoRepository.UpdateTodoAsync(todo, cancellationToken);
    }
}
