using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TechsysLog.Application.Abstractions.Authentication;
using TechsysLog.Domain.Common;
using TechsysLog.Domain.Users;

namespace TechsysLog.Infrastructure.Authentication;

internal sealed class JwtTokenService(
    IOptions<JwtSettings> options,
    IDateTimeProvider clock) : ITokenService
{
    private readonly JwtSettings _settings = options.Value;

    public GeneratedToken GenerateToken(User user)
    {
        var expiresAt = clock.UtcNow.AddHours(_settings.ExpirationHours);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(clock.UtcNow).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
                ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var keyBytes = Encoding.UTF8.GetBytes(_settings.SigningKey);
        var key = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: clock.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        var serialized = new JwtSecurityTokenHandler().WriteToken(token);
        return new GeneratedToken(serialized, expiresAt);
    }
}
