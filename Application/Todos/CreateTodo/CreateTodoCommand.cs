namespace Application.Todos.CreateTodo;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.CQRS;

public record CreateTodoCommand(string Title, string Description) : ICommand<CreateTodoCommandResponse>;
