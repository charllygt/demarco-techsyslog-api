using MongoDB.Driver;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Domain.Common.Ids;
using TechsysLog.Domain.Users;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Infrastructure.Persistence.Mongo.Repositories;

internal sealed class UserRepository(MongoContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct) =>
        await context.Users.Find(u => u.Id == id).FirstOrDefaultAsync(ct).ConfigureAwait(false);

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken ct) =>
        await context.Users.Find(u => u.Email == email).FirstOrDefaultAsync(ct).ConfigureAwait(false);

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken ct) =>
        await context.Users.Find(u => u.Email == email).AnyAsync(ct).ConfigureAwait(false);

    public Task AddAsync(User user, CancellationToken ct) =>
        context.Users.InsertOneAsync(user, cancellationToken: ct);

    public async Task<IReadOnlyList<UserId>> GetAllUserIdsAsync(CancellationToken ct)
    {
        var ids = await context.Users.Find(FilterDefinition<User>.Empty)
            .Project(u => u.Id)
            .ToListAsync(ct)
            .ConfigureAwait(false);
        return ids;
    }
}
