namespace Presentation.MinimalApi.Common.net7;

using Application.Results;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

public static class ResultExtensions
{
    public static TypedApplicationResult<Ok> ToTypedApiResult(this Result result)
    {
        var successResult = result.IsSuccess ? TypedResults.Ok() : null;

        return new TypedApplicationResult<Ok>(successResult, result.Error);
    }

    public static TypedApplicationResult<TSuccessResult> ToTypedApiResult<TSuccessResult>(this Result result, Func<TSuccessResult> successResultMap)
        where TSuccessResult : IResult
    {
        var successResult = result.IsSuccess ? successResultMap.Invoke() : default;

        return new TypedApplicationResult<TSuccessResult>(successResult, result.Error);
    }

    public static TypedApplicationResult<Ok<TResult>> ToTypedApiResult<TResult>(this Result<TResult> result)
        where TResult : notnull
    {
        var successResult = result.IsSuccess ? TypedResults.Ok(result.Value) : null;

        return new TypedApplicationResult<Ok<TResult>>(successResult, result.Error);
    }

    public static TypedApplicationResult<TSuccessResult> ToTypedApiResult<TResult, TSuccessResult>(this Result<TResult> result, Func<TResult, TSuccessResult> successResultMap)
                where TSuccessResult : IResult
        where TResult : notnull
    {
        var successResult = result.IsSuccess ? successResultMap.Invoke(result.Value) : default;

        return new TypedApplicationResult<TSuccessResult>(successResult, result.Error);
    }
}
