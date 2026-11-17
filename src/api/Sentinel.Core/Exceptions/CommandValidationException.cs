using FluentValidation.Results;

namespace Sentinel.Core.Exceptions;

/// <summary>Represents validation failures of a command (a request which mutates state).</summary>
public class CommandValidationException(IEnumerable<ValidationFailure> errors)
    : ValidationExceptionBase(errors)
{
    /// <summary>Throws a <see cref="CommandValidationException"/> if the validation result has any failures.</summary>
    /// <param name="result">The validation result.</param>
    public static void ThrowIfValidationFailed(ValidationResult result)
    {
        if (result.IsValid)
            return;

        throw new CommandValidationException(result.Errors);
    }
}