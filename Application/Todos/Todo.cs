namespace Application.Todos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Todo
{
    public Guid Id { get; }
    public string Title { get; }
    public string Description { get; }
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    public Todo(string title, string description)
    {
        this.Id = Guid.NewGuid();
        this.Title = title;
        this.Description = description;
        this.CreatedAt = DateTime.UtcNow;
        this.UpdatedAt = this.CreatedAt;
    }

    public void Complete()
    {
        this.IsCompleted = true;
        this.UpdatedAt = DateTime.UtcNow;
    }
}
