using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Providers;

public class TokenProvider(IOptions<JwtConfiguration> jwtConfiguration)
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    public string CreateToken(UserEntity user)
    {
        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // user id claim
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "https://localhost:7183",
            audience: "https://localhost:7183",
            claims: claims,
            expires: DateTime.UtcNow.Add(_jwtConfiguration.TokenLifetime),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}
