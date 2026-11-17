using Sentinel.Core.Models;

namespace Sentinel.Core.Services;

public interface IActorAccessor
{
    /// <summary>The actor for the current request.</summary>
    Actor Current { get; }
}