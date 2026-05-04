using NSubstitute;
using Shouldly;
using TechsysLog.Application.Abstractions.Authentication;
using TechsysLog.Application.Abstractions.Persistence;
using TechsysLog.Application.Users.CreateUser;
using TechsysLog.Domain.Users;
using TechsysLog.Domain.Users.ValueObjects;

namespace TechsysLog.Application.Tests.Users;

public sealed class CreateUserCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
    {
        _sut = new CreateUserCommandHandler(_users, _hasher);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateUser()
    {
        var command = new CreateUserCommand("João", "joao@test.com", "Senha@123");
        _users.EmailExistsAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(false);
        _hasher.Hash("Senha@123").Returns("$2a$12$hashed");

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Name.ShouldBe("João");
        result.Value.Email.ShouldBe("joao@test.com");
        await _users.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldFail()
    {
        var command = new CreateUserCommand("João", "exists@test.com", "Senha@123");
        _users.EmailExistsAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(UserErrors.EmailAlreadyInUse);
        await _users.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldFail()
    {
        var command = new CreateUserCommand("João", "invalid", "Senha@123");

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldStartWith("Email.");
    }
}
