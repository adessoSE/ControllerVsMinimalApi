namespace Application.Todos.GetTodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

internal class GetTodoQueryValidator : AbstractValidator<GetTodoQuery>
{
	public GetTodoQueryValidator()
	{
		this.RuleFor(t => t.TodoId).NotEmpty();
	}
}
