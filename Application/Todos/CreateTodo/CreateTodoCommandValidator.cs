namespace Application.Todos.CreateTodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

internal class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
	public CreateTodoCommandValidator()
	{
		this.RuleFor(t => t.Title)
			.NotEmpty()
			.MinimumLength(5)
			.MaximumLength(20);

		this.RuleFor(t => t.Description)
			.NotEmpty()
			.MaximumLength(100);
	}
}
