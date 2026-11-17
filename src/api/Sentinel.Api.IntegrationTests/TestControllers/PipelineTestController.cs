using Beans.Patchable;
using Beans.Requestable;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Attributes;
using Sentinel.Api.Controllers.V1.Abstract;
using Sentinel.Api.Exceptions;
using Sentinel.Api.Infrastructure;
using Sentinel.Core.Exceptions;
using Sentinel.Core.Services;
using Sentinel.Data.Context;
using Sentinel.Data.Services;

namespace Sentinel.Api.IntegrationTests.TestControllers;

/// <summary>
/// Test-only endpoints, added to the real application as an extra application part by
/// <see cref="ApiWebApplicationFactory"/>. They let the integration tests drive the real middleware, filters,
/// exception handlers, serializers and container without Sentinel.Api itself exposing anything but /health.
/// </summary>
public class PipelineTestController : ApiControllerBase
{
    public const string Route = "api/v1/pipelinetest";

    public sealed record UnregisteredQuery : IRequest<string>;

    public sealed record ValidatedCommand(string? Name);

    public sealed class ValidatedCommandValidator : AbstractValidator<ValidatedCommand>
    {
        public ValidatedCommandValidator() => RuleFor(command => command.Name).NotEmpty().MaximumLength(5);
    }

    public sealed record PatchModel(Optional<string?> Name, DateTime When, DateTime? MaybeWhen);

    [HttpGet("throw/{kind}")]
    public IActionResult Throw(string kind) => throw (kind switch
    {
        "access-denied" => new AccessDeniedException(),
        "not-found" => new NotFoundException("Widget", "widget-001"),
        "command-validation" => new CommandValidationException([new ValidationFailure("Name", "Bad name.")]),
        "query-validation" => new QueryValidationException([new ValidationFailure("Cursor", "Bad cursor.")]),
        "missing-header" => new MissingHeaderException("If-Match"),
        "missing-scope" => new MissingScopeException(Scopes.Authorizer),
        _ => new InvalidOperationException("Something internal and secret.")
    });

    [HttpPost("validate")]
    public IActionResult Validate(ValidatedCommand command)
    {
        CommandValidationException.ThrowIfValidationFailed(new ValidatedCommandValidator().Validate(command));
        return NoContent();
    }

    [HttpGet("mediator")]
    public async Task<IActionResult> Mediator([FromServices] IMediator mediator, CancellationToken cancellationToken)
        => Ok(await mediator.SendAsync(new UnregisteredQuery(), cancellationToken));

    [HttpGet("scoped"), RequiresScope(Scopes.Authorizer)]
    public IActionResult Scoped() => NoContent();

    [HttpGet("header"), RequiresHeader("Idempotency-Key")]
    public IActionResult Header() => NoContent();

    [HttpGet("actor"), RequiresCurrentActor]
    public IActionResult Actor([FromServices] IActorAccessor actorAccessor) => Ok(actorAccessor.Current);

    [HttpPatch("echo")]
    public IActionResult Echo(PatchModel model) => Ok(new
    {
        NameIsSet = model.Name.IsSet,
        Name = model.Name.Value,
        model.When,
        model.MaybeWhen
    });

    [HttpPost("database")]
    public async Task<IActionResult> Database(
        [FromServices] ApiDbContext readContext,
        [FromServices] IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Ok(new
        {
            CanConnect = await readContext.Database.CanConnectAsync(cancellationToken),
            SharesContext = ReferenceEquals(readContext, unitOfWork.Context),
            Provider = unitOfWork.Context.Database.ProviderName
        });
    }
}