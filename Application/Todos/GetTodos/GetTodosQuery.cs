namespace Application.Todos.GetTodos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.CQRS;

public sealed record GetTodosQuery : IQuery<GetTodosQueryResponse[]>;
