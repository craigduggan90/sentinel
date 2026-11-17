using Sentinel.Domain.Entities;

namespace Sentinel.Data.Repositories.Verifications;

/// <summary>Describes a read-write repository containing instances of <see cref="Verification"/>.</summary>
public interface IVerificationsRepository : IReadWriteRepository<Verification>, IReadOnlyVerificationsRepository;