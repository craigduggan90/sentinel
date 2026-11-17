using FluentValidation.Results;
using Sentinel.Core.Exceptions;

namespace Sentinel.Core.UnitTests.Exceptions;

public static class CommandValidationExceptionTests
{
    public class Constructor
    {
        [Fact]
        public void SetsErrorsAndMessage_WhenConstructed()
        {
            ValidationFailure[] errors = [new("PropertyName", "Error message")];

            var exception = new CommandValidationException(errors);

            Assert.Same(errors, exception.Errors);
            Assert.Equal("One or more validation errors occurred.", exception.Message);
        }

        [Fact]
        public void SetsEmptyErrors_WhenConstructedWithEmptyCollection()
        {
            var exception = new CommandValidationException([]);

            Assert.Empty(exception.Errors);
        }
    }

    public class ThrowIfValidationFailed
    {
        [Fact]
        public void DoesNotThrow_WhenResultIsValid()
        {
            var exception = Record.Exception(() => CommandValidationException.ThrowIfValidationFailed(new ValidationResult()));

            Assert.Null(exception);
        }

        [Fact]
        public void ThrowsWithOriginalErrors_WhenResultIsInvalid()
        {
            ValidationFailure[] failures = [new("PropertyName", "Error message")];

            var exception = Assert.Throws<CommandValidationException>(
                () => CommandValidationException.ThrowIfValidationFailed(new ValidationResult(failures)));

            Assert.Equivalent(failures, exception.Errors);
        }
    }
}