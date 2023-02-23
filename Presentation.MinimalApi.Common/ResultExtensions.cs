namespace Presentation.MinimalApi.Common;

using Application.Results;

using Microsoft.AspNetCore.Http;

public static class ResultExtensions
{
    public static IResult ToApiResult(this Result result, Func<IResult>? successResultMap = null)
    {
        if (result.IsSuccess)
        {
            return successResultMap?.Invoke() ?? Results.Ok();
        }

        return CreateErrorActionResult(result.Error);
    }

    public static IResult ToApiResult<TResult>(this Result<TResult> result, Func<TResult, IResult>? successResultMap = null)
        where TResult : notnull
    {
        if (result.IsSuccess)
        {
            return successResultMap?.Invoke(result.Value) 
                ?? Results.Ok(result.Value);
        }
        
        return CreateErrorActionResult(result.Error);
    }

    private static IResult CreateErrorActionResult(Error error)
    {
        return error switch
        {
            { Code: "not_found" } => Results.NotFound(),
            { Code: "unauthorized" } => Results.Unauthorized(),
            ValidationError validationError => CreateValidationErrorResult(validationError),
            BusinessError businessError => CreateBusinessErrorResult(businessError),
            UnexpectedError unexpectedError => CreateUnexpectedErrorResult(unexpectedError),
            _ => CreateUnexpectedErrorResult(),
        };
    }

    private static IResult CreateValidationErrorResult(ValidationError validationError)
    {
        var errors = validationError.ValidationResults.ToDictionary(e => e.Source.Substring(0, 1).ToLower() + e.Source.Substring(1), e => e.ErrorMessages.ToArray());

        return Results.ValidationProblem(errors, validationError.Message, title: validationError.Code);
    }

    private static IResult CreateUnexpectedErrorResult(UnexpectedError? unexpectedError = null)
    {       
        return Results.Problem(unexpectedError?.Message ?? "An internal server error occurred", statusCode: StatusCodes.Status500InternalServerError);
    }

    private static IResult CreateBusinessErrorResult(BusinessError businessError)
    {
        var statusCode = businessError.ErrorType switch
        {
            BusinessError.Type.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        return Results.Problem(businessError.Message, statusCode: statusCode, title: businessError.Code);
    }
}
