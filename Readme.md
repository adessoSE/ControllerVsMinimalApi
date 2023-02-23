# ASP.NET Core Controller vs. Minimal API

This project compares traditional ASP.NET Core controllers with the new Minimal API approach. The project is built using the Clean Architecture pattern with CQRS and Mediator, and demonstrates how to implement these concepts in the context of a .NET web application.

## Projects

The project structure is as follows:

- `Application`: contains the application logic, such as commands, queries, and command/query handlers.
- `Infrastructure`: contains the implementation of repositories and other data access components.
- `PerformanceTests`: contains performance tests for the application.
- `Presentation.ControllerApi`: contains the traditional ASP.NET Core controller project.
- `Presentation.MinimalApi.Common`: contains common code shared between the Minimal API projects.
- `Presentation.MinimalApi.Common.net7`: contains common code shared between the Minimal API projects for .NET 7.
- `Presentation.MinimalApi.net6`: contains the Minimal API project for .NET 6.
- `Presentation.MinimalApi.net7`: contains the Minimal API project for .NET 7.
- `Presentation.MinimalApi.net6.CleanArchitecture`: contains an implementation of Clean Architecture using Minimal API and .NET 6.
- `Presentation.MinimalApi.net7.CleanArchitecture`: contains an implementation of Clean Architecture using Minimal API and .NET 7.

The `Presentation.ControllerApi` project is the basic project that implements traditional controllers. The `Presentation.MinimalApi.net6` project shows the minimal API implementation for .NET 6, and `Presentation.MinimalApi.net7` shows the implementation for .NET 7. The `*.CleanArchitecture` projects implement Clean Architecture on top of the `Application` and `Infrastructure` projects.

It is possible to configure `UseDatabase:true` in the `appsettings.json` or as a `dotnet run` command line parameter to enable the SQL Database Repository with Entity Framework Core. Otherwise, a Repository with an in-memory concurrent dictionary is used for the `Todo` entity.

To run the SQL database in Docker, the following command is required:

```cmd
docker run --env=ACCEPT_EULA=Y --env=SA_PASSWORD=YourStrong!Passw0rd -p 6433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
```

The `appsettings.json` looks as follows for each web API project. The only difference is the database name.

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System": "Warning"
      }
    }
  },
  "ConnectionStrings": {
    "SqlDb": "Server=127.0.0.1,6433;Database=TodoMinimal7;User=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;"
  }
}
```

## Features

- A SQL database repository with Entity Framework Core or an in-memory concurrent dictionary repository for the Todo entity.
- Configuration option to use a SQL database through `appsettings.json` or the command line.
- An example of the decorator pattern with caching and performance logging.
- All exceptions caught by the `UnexpectedExceptionPipelineBehaviour.cs` middleware, which returns a `Result` class.
- FluentValidation used for strongly-typed validation rules with an example in `CreateTodoCommandValidator.cs`.
- Command and Query structure with validators and response as in `CreateTodoCommand.cs`, `CreateTodoCommandHandler.cs`, `CreateTodoCommandResponse.cs`, and `CreateTodoCommandValidator.cs`.

## Command and Query Structure

The project follows the Command Query Responsibility Segregation (CQRS) pattern, which separates the operations that change data from the operations that read data. The CQRS pattern helps to create a more scalable, maintainable, and testable architecture.

The structure of a command or query follows the naming convention:

- `[NameOfOperation]Command.cs`
- `[NameOfOperation]CommandHandler.cs`
- `[NameOfOperation]CommandResponse.cs` (optional)
- `[NameOfOperation]CommandValidator.cs` (optional)

For queries, only the validator is optional.

### Commands

Commands represent operations that change data, such as creating, updating, or deleting entities. Each command consists of four parts:

1. **Command**: A class that encapsulates the data required for the operation. It should be a plain old C# object (POCO) with properties that match the data that needs to be sent.

2. **Command Handler**: A class that implements the `ICommandHandler<TCommand, TResult>` interface and contains the logic to perform the operation. The command handler is responsible for loading any necessary data, validating the input, and updating the data store.

3. **Command Response** (optional): A class that encapsulates the result of the operation. It should be a plain old C# object (POCO) with properties that match the data that needs to be returned.

4. **Command Validator** (optional): A class that uses Fluent Validation, a popular .NET library for building strongly-typed validation rules, to validate the input data for the command. The validator should be a separate class to keep the validation logic separate from the command logic.

### Queries

Queries represent operations that read data, such as retrieving a list of entities or a single entity. Each query consists of four parts:

1. **Query**: A class that encapsulates the data required for the operation. It should be a plain old C# object (POCO) with properties that match the data that needs to be sent.

2. **Query Handler**: A class that implements the `IQueryHandler<TQuery, TResult>` interface and contains the logic to perform the operation. The query handler is responsible for loading any necessary data, validating the input, and returning the result.

3. **Query Response**: A class that encapsulates the result of the operation. It should be a plain old C# object (POCO) with properties that match the data that needs to be returned.

4. **Query Validator** (optional): A class that uses Fluent Validation to validate the input data for the query. The validator should be a separate class to keep the validation logic separate from the query logic.


## FluentValidation

The project uses FluentValidation, which is a popular .NET library for building strongly-typed validation rules. The project includes an example of a validator for the `CreateTodoCommand` in the `CreateTodoCommandValidator.cs` file.

```csharp
namespace Application.Todos.CreateTodo;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

internal class CreateTodoCommandValidator :      AbstractValidator<CreateTodoCommand>
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
```

## Results and Error Handling

The application uses the **Result pattern** to represent the results of operations. The `Result` class contains information about whether the operation was successful and an error message if it was not. `IsSuccess` is a boolean value that indicates whether the operation was successful. If `IsSuccess` is `false`, then `Error` is not null and contains information about the error.

All exceptions in the application are caught by the `UnexpectedExceptionPipelineBehaviour.cs` middleware, which returns a `Result` object. This middleware logs the exception and returns an `UnexpectedError` object.

The `Result` class has static methods for creating successful and unsuccessful results. There is also an implicit operator conversion that allows you to pass an `Error` object directly to the `Result` class.

### Examples

Here is an example of the `CompleteTodoCommandHandler.cs` file that uses the `Result` pattern:

```csharp
internal class CompleteTodoCommandHandler : ICommandHandler<CompleteTodoCommand>
{
    private readonly ITodoRepository todoRepository;

    public CompleteTodoCommandHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result> Handle(CompleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await this.todoRepository.GetTodoByIdAsync(request.TodoId, cancellationToken);

        if (todo is null)
        {
            return Error.NotFound;
        }

        if (todo.IsCompleted)
        {
            return BusinessError.Conflict("Todo.AlreadyCompleted", $"The todo {todo.Id} is already completed.");
        }

        todo.Complete();

        await this.todoRepository.UpdateTodoAsync(todo, cancellationToken);

        return Result.Success();
    }
}
```

The `CompleteTodoCommandHandler.cs` file returns a `Result` object. If the operation is successful, the `Result` object is returned with `IsSuccess` set to `true`. If the operation is unsuccessful, the `Result` object is returned with `IsSuccess` set to `false` and `Error` set to an `Error` object.

```csharp
internal class GetTodoQueryHandler : IQueryHandler<GetTodoQuery, GetTodoQueryResponse>
{
    private readonly ITodoRepository todoRepository;

    public GetTodoQueryHandler(ITodoRepository todoRepository)
    {
        this.todoRepository = todoRepository;
    }

    public async Task<Result<GetTodoQueryResponse>> Handle(GetTodoQuery request, CancellationToken cancellationToken)
    {
        var todo = await this.todoRepository.GetTodoByIdAsync(request.TodoId, cancellationToken);

        if (todo is null)
        {
            return Error.NotFound;
        }

        return new GetTodoQueryResponse(todo);
    }
}
```

The `GetTodoQueryHandler.cs` file returns a `Result` object with a generic type parameter. If the operation is successful, the `Result` object is returned with `IsSuccess` set to `true` and `Value` set to the `GetTodoQueryResponse` object. If the operation is unsuccessful, the `Result` object is returned with `IsSuccess` set to `false` and `Error` set to an `Error` object.

## Decoration

The project includes an example of the decorator pattern, which can be seen as middleware for interfaces. Decorators extend the functionality of an interface without changing the code of the interface. The order of the decorators is important, as they are called in the order in which they are registered. Decorators must be used at the end, as the interfaces to be decorated must already be registered.

```csharp
builder.Services.Decorate<ITodoRepository, CachedTodoRespository>().AddMemoryCache();
builder.Services.Decorate<ITodoRepository, PerformanceLoggedTodoRepository>();
```

The `CachedTodoRepository` class and the `PerformanceLoggedTodoRepository` class in the `Presentation.MinimalApi.Common` project are examples of decorators.

## Performance

The results of the performance tests are as follows:

### In Memory DB

|              | Requests per Second | Requests | Data Transfer |
|--------------|---------------------|----------|---------------|
| Controller   | 9.444               | 566.663  | 333.9 MB      |
| Minimal API .NET 6 | 11.128              | 667.733  | 488.1 MB      |
| Minimal API .NET 7 | 10.796              | 647.799  | 381.7 MB      |

### EF Core SQL DB

|              | Requests per Second | Requests | Data Transfer |
|--------------|---------------------|----------|---------------|
| Controller   | 1.112               | 66.741   | 39.2 MB       |
| Minimal API .NET 6 | 1.178               | 70.725   | 51.6 MB       |
| Minimal API .NET 7 | 1.161               | 69.678   | 40.9 MB       |

## Conclusion

This project showed the differences between a traditional Controller-based approach and a Minimal API-based approach in ASP.NET Core. While both approaches have their strengths and weaknesses, the Minimal API approach offers a simpler, more lightweight way of building web APIs.

In addition, the project also demonstrated the use of the Clean Architecture pattern with CQRS and Mediator, which provides a solid foundation for building scalable and maintainable applications.

Other topics covered in the project include the use of the Result pattern instead of exceptions, the use of FluentValidation for input validation, and the use of decorators to add functionality to existing interfaces.

Finally, the project also showed the importance of performance testing, and demonstrated that the Minimal API approach can offer significant performance benefits over the traditional Controller-based approach.

Overall, this project provides a useful starting point for developers who are interested in building web APIs with ASP.NET Core using the Minimal API approach and Clean Architecture pattern.
