namespace Application.Results;

using System.Collections.Immutable;
using System.Runtime.CompilerServices;

public sealed record ValidationError(ImmutableArray<ValidationError.ValidationResult> ValidationResults)
    : Error("validation_error", "Validation error")
{
    public sealed record ValidationResult(string Source, ImmutableArray<string> ErrorMessages);
}