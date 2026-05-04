namespace TechsysLog.Application.Abstractions.Authentication;

public interface IPasswordHasher
{
    string Hash(string plainTextPassword);
    bool Verify(string plainTextPassword, string hashedPassword);
}
