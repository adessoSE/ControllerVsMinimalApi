namespace Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Todos;

using Infrastructure.EntityFramework;

using Microsoft.EntityFrameworkCore;

internal class TodoEfCoreRepository : ITodoRepository
{
    private readonly ApplicationDbContext dbContext;

    public TodoEfCoreRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<Todo>> GetTodosAsync(CancellationToken cancellationToken)
    {
        return await this.dbContext.Todos.ToListAsync(cancellationToken);
    }

    public async Task<Todo?> GetTodoByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await this.dbContext.Todos.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        await this.dbContext.Todos.AddAsync(todo, cancellationToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTodoAsync(Todo todo, CancellationToken cancellationToken)
    {
        this.dbContext.Todos.Update(todo);
        await this.dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTodoAsync(Guid id, CancellationToken cancellationToken)
    {
        var todo = await this.dbContext.Todos.FindAsync(new object[] { id }, cancellationToken);

        if (todo is null)
        {
            return;
        }

        this.dbContext.Todos.Remove(todo);
        await this.dbContext.SaveChangesAsync(cancellationToken);
    }
}
