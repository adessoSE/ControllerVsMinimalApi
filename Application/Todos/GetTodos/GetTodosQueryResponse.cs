namespace Application.Todos.GetTodos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed record GetTodosQueryResponse(Guid Id, string Title, string Description, bool IsCompleted, DateTime UpdatedAt);
