namespace Sentinel.Core.Exceptions;

/// <summary>Represents an error arising when the current actor may not perform the requested action.</summary>
public class AccessDeniedException : Exception
{
    internal const string DefaultMessage = "Access to the requested resource or action is denied.";

    /// <summary>Initializes a new instance of the <see cref="AccessDeniedException"/> class.</summary>
    public AccessDeniedException()
        : base(DefaultMessage)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AccessDeniedException"/> class.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, if any.</param>
    public AccessDeniedException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}