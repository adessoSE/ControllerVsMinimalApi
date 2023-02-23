namespace Application.CQRS;

using Application.Results;

using MediatR;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResult> : IRequest<Result<TResult>>
    where TResult : notnull
{
}
