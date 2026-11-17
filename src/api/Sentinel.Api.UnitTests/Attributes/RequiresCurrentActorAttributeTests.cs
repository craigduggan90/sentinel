using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sentinel.Api.Attributes;
using Sentinel.Api.Exceptions;
using Sentinel.Core.Models;
using Sentinel.Core.Services;

namespace Sentinel.Api.UnitTests.Attributes;

public static class RequiresCurrentActorAttributeTests
{
    public class OnActionExecuting
    {
        private static Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext CreateContext(IActorAccessor actorAccessor)
        {
            var services = new ServiceCollection().AddSingleton(actorAccessor);
            return ActionContextFactory.CreateExecutingContext(
                new DefaultHttpContext { RequestServices = services.BuildServiceProvider() });
        }

        [Fact]
        public void Throws_WhenActorAccessorThrowsMissingHeaderException()
        {
            var actorAccessor = Substitute.For<IActorAccessor>();
            actorAccessor.Current.Returns(_ => throw new MissingHeaderException("Sentinel-User-Id"));
            var sut = new RequiresCurrentActorAttribute();

            Assert.Throws<MissingHeaderException>(() => sut.OnActionExecuting(CreateContext(actorAccessor)));
        }

        [Fact]
        public void ResolvesActorExactlyOnce_WhenActorAccessorResolvesSuccessfully()
        {
            var actorAccessor = Substitute.For<IActorAccessor>();
            actorAccessor.Current.Returns(new Actor("user-001", "display-name"));
            var sut = new RequiresCurrentActorAttribute();

            var exception = Record.Exception(() => sut.OnActionExecuting(CreateContext(actorAccessor)));

            Assert.Null(exception);
            _ = actorAccessor.Received(1).Current;
        }
    }
}