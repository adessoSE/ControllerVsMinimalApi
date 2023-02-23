namespace Presentation.MinimalApi.Common.Decorator.PerformanceLogging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Application.Todos;

using Microsoft.Extensions.Logging;

internal class PerformanceLoggedTodoRepository : ITodoRepository
{
    private readonly ITodoRepository decoratedRepository;
    private readonly ILogger<PerformanceLoggedTodoRepository> logger;

    public PerformanceLoggedTodoRepository(ITodoRepository decoratedRepository, ILogger<PerformanceLoggedTodoRepository> logger)
    {
        this.decoratedRepository = decoratedRepository;
        this.logger = logger;
    }

    public async Task AddTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        await this.decoratedRepository.AddTodoAsync(todo, cancellationToken);

        stopwatch.Stop();

        this.logger.LogInformation("AddTodoAsync took {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
    }

    public Task DeleteTodoAsync(Guid id, CancellationToken cancellationToken)
    {
        return this.decoratedRepository.DeleteTodoAsync(id, cancellationToken);
    }

    public async Task<Todo?> GetTodoByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var todo = await this.decoratedRepository.GetTodoByIdAsync(id, cancellationToken);

        stopwatch.Stop();

        this.logger.LogInformation("GetTodoByIdAsync took {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);

        return todo;
    }

    public async Task<IEnumerable<Todo>> GetTodosAsync(CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var todos = await this.decoratedRepository.GetTodosAsync(cancellationToken);

        stopwatch.Stop();

        this.logger.LogInformation("GetTodosAsync took {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);

        return todos;
    }

    public Task UpdateTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        return this.decoratedRepository.UpdateTodoAsync(todo, cancellationToken);
    }
}
