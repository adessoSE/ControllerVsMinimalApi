namespace Presentation.MinimalApi.Common.net7;

using System.Reflection;
using System.Threading.Tasks;

using Application.Results;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;


public class TypedApplicationResult<TSuccessResult> : IResult, INestedHttpResult, IEndpointMetadataProvider
    where TSuccessResult : IResult
{
    public IResult Result { get; }

    static void IEndpointMetadataProvider.PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        Populate<Results<TSuccessResult, NotFound, UnauthorizedHttpResult, ProblemHttpResult, ValidationProblem>>(method, builder);
    }

    private static void Populate<TStaticPopulate>(MethodInfo method, EndpointBuilder builder)
        where TStaticPopulate : IEndpointMetadataProvider
    {
        TStaticPopulate.PopulateMetadata(method, builder);
    }

    internal TypedApplicationResult(TSuccessResult? successResult, Error? error)
    {
        this.Result = successResult ?? CreateErrorActionResult(error!);
    }

    Task IResult.ExecuteAsync(HttpContext httpContext)
    {
        return this.Result.ExecuteAsync(httpContext);
    }

    private static IResult CreateErrorActionResult(Error error)
    {
        return error switch
        {
            { Code: "not_found" } => TypedResults.NotFound(),
            { Code: "unauthorized" } => TypedResults.Unauthorized(),
            ValidationError validationError => CreateValidationErrorResult(validationError),
            BusinessError businessError => CreateBusinessErrorResult(businessError),
            UnexpectedError unexpectedError => CreateUnexpectedErrorResult(unexpectedError),
            _ => CreateUnexpectedErrorResult(),
        };
    }

    private static ValidationProblem CreateValidationErrorResult(ValidationError validationError)
    {
        var errors = validationError.ValidationResults.ToDictionary(e => e.Source.Substring(0, 1).ToLower() + e.Source.Substring(1), e => e.ErrorMessages.ToArray());

        return TypedResults.ValidationProblem(errors, validationError.Message, title: validationError.Code);
    }

    private static ProblemHttpResult CreateUnexpectedErrorResult(UnexpectedError? unexpectedError = null)
    {
        return TypedResults.Problem(unexpectedError?.Message ?? "An internal server error occurred", statusCode: StatusCodes.Status500InternalServerError);
    }

    private static ProblemHttpResult CreateBusinessErrorResult(BusinessError businessError)
    {
        var statusCode = businessError.ErrorType switch
        {
            BusinessError.Type.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        return TypedResults.Problem(businessError.Message, statusCode: statusCode, title: businessError.Code);
    }
}
