namespace Presentation.ControllerApi.Controllers;

using Application.Results;

using Microsoft.AspNetCore.Mvc;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult CreateActionResult(Result result, Func<IActionResult>? successResultMap = null)
    {
        if (result.IsSuccess)
        {
            return successResultMap?.Invoke() ?? this.Ok();
        }

        return this.CreateErrorActionResult(result.Error);
    }

    protected IActionResult CreateActionResult<TResult>(Result<TResult> result, Func<TResult, IActionResult>? successResultMap = null)
        where TResult : notnull
    {
        if (result.IsSuccess)
        {
            return successResultMap?.Invoke(result.Value) ??
                this.Ok(result.Value);
        }

        return this.CreateErrorActionResult(result.Error);
    }

    private IActionResult CreateErrorActionResult(Error error)
    {
        return error switch
        {
            { Code: "not_found" } => this.NotFound(),
            { Code: "unauthorized" } => this.Unauthorized(),
            ValidationError validationError => this.CreateValidationErrorResult(validationError),
            BusinessError businessError => this.CreateBusinessErrorResult(businessError),
            UnexpectedError unexpectedError => this.CreateUnexpectedErrorResult(unexpectedError),
            _ => this.CreateUnexpectedErrorResult(),
        };
    }

    private IActionResult CreateValidationErrorResult(ValidationError validationError)
    {
        var errors = validationError.ValidationResults.ToDictionary(e => e.Source.Substring(0, 1).ToLower() + e.Source.Substring(1), e => e.ErrorMessages.ToArray());

        var details = new ValidationProblemDetails(errors)
        {
            Title = validationError.Message,
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Instance = this.HttpContext.Request.Path,
        };

        return this.ValidationProblem(details);
    }

    private IActionResult CreateUnexpectedErrorResult(UnexpectedError? unexpectedError = null)
    {
        var problem = new ProblemDetails()
        {
            Title = unexpectedError?.Message ?? "An internal server error occurred",
            Type = "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1",
            Status = StatusCodes.Status500InternalServerError,
            Instance = this.HttpContext.Request.Path,
        };

        return this.StatusCode(problem.Status.Value, problem);
    }

    private IActionResult CreateBusinessErrorResult(BusinessError businessError)
    {
        var statusCode = businessError.ErrorType switch
        {
            BusinessError.Type.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        return this.Problem(businessError.Message, statusCode: statusCode, title: businessError.Code);
    }
}
