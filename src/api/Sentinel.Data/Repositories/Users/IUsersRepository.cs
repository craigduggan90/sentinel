using Sentinel.Domain.Entities;

namespace Sentinel.Data.Repositories.Users;

/// <summary>Describes a read-write repository containing instances of <see cref="User"/>.</summary>
public interface IUsersRepository : IReadWriteRepository<User>, IReadOnlyUsersRepository;