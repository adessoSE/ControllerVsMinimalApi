namespace Application.Results;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using FluentValidation.TestHelper;

public class Result
{
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure { get; }
    public Error? Error { get; }

    protected Result(Error? error)
    {
        IsSuccess = error is null;
        IsFailure = error is not null;
        Error = error;
    }

    public static Result Success() => new Result(null);

    public static Result<TValue> Success<TValue>(TValue value)
        where TValue : notnull => Result<TValue>.Success(value);

    public static Result Fail(string code, string message) => new Result(new Error(code, message));

    public static Result Fail(Error error) => new Result(error);

    public static TResult Fail<TResult>(Error error)
        where TResult : Result
    {
        var resultType = typeof(TResult);

        if (resultType.IsGenericType)
        {
            var genericArgument = resultType.GetGenericArguments().First();

            var result = resultType
                .GetGenericTypeDefinition()
                .MakeGenericType(genericArgument)
                .GetMethod(nameof(Fail), new[] { typeof(Error) })!
                .Invoke(null, new object[] { error })!;

            return (TResult)result;
        }

        return (TResult)Fail(error);
    }

    public static implicit operator Result(Error error) => Fail(error);
}

public class Result<TValue> : Result
    where TValue : notnull
{
    [MemberNotNullWhen(false, nameof(Error))]
    [MemberNotNullWhen(true, nameof(Value))]
    public new bool IsSuccess => base.IsSuccess;

    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public new bool IsFailure => base.IsFailure;
    public new Error? Error => base.Error;

    public TValue? Value { get; }

    protected Result(TValue? value, Error? error)
        : base(error)
    {
        Value = value;
    }

    public static Result<TValue> Success(TValue value) => new Result<TValue>(value, null);
    public static new Result<TValue> Fail(string code, string message) => new Result<TValue>(default, new Error(code, message));
    public static new Result<TValue> Fail(Error error) => new Result<TValue>(default, error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Fail(error);
}