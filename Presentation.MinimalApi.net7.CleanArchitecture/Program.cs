using Application;
using Application.Todos;

using Infrastructure;

using Presentation.MinimalApi.Common;
using Presentation.MinimalApi.Common.Decorator.Caching;
using Presentation.MinimalApi.Common.Decorator.PerformanceLogging;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureLogging();

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddStartupConfigurations(builder.Configuration);

//TodoEndpoints.MapEndpoints(app);
//app.MapTodoEndpoints();

// Decorator können als "Middleware" für Interfaces betrachtet werden.
// Sie können die Funktionalität eines Interfaces erweitern, ohne den Code des Interfaces zu ändern.
// Die Reihenfolge der Decorator ist wichtig, da sie in der Reihenfolge aufgerufen werden, in der sie registriert wurden.
// Decorator müssen am Ende verwendet werden, da die zu dekorierenden Interfaces bereits registriert werden müssen.
//
// English:
// Decorators can be seen as "middleware" for interfaces.
// They can extend the functionality of an interface without changing the code of the interface.
// The order of the decorators is important, as they are called in the order in which they are registered. 
// Decorators must be used at the end, as the interfaces to be decorated must already be registered.
builder.Services.Decorate<ITodoRepository, CachedTodoRespository>().AddMemoryCache();
builder.Services.Decorate<ITodoRepository, PerformanceLoggedTodoRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapEndpoints();
app.UseStartupConfigurations();

app.Run();