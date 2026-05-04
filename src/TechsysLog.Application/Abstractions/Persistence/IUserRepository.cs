using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Users;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct);
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct);
    Task<bool> EmailExistsAsync(Email email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task<IReadOnlyList<UserId>> GetAllUserIdsAsync(CancellationToken ct);
}
