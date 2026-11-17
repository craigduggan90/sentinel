using FluentValidation.Results;

namespace Sentinel.Core.Exceptions;

/// <summary>Represents validation failures of a query (a read-only request).</summary>
public class QueryValidationException(IEnumerable<ValidationFailure> errors)
    : ValidationExceptionBase(errors)
{
    /// <summary>Throws a <see cref="QueryValidationException"/> if the validation result has any failures.</summary>
    /// <param name="result">The validation result.</param>
    public static void ThrowIfValidationFailed(ValidationResult result)
    {
        if (result.IsValid)
            return;

        throw new QueryValidationException(result.Errors);
    }
}