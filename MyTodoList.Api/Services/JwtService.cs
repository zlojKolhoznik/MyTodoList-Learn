using Microsoft.IdentityModel.Tokens;
using MyTodoList.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyTodoList.Api.Services
{
    public class JwtService : IJwtService
    {
        private const string UserIdClaimType = "UserId";
        private const string UserNameClaimType = "UserName";

        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Generate(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = [
                new Claim(UserIdClaimType, user.Id),
                new Claim(UserNameClaimType, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.NormalizedUserName),
                ];
            var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: creds,
                    notBefore: DateTime.Now
                );
            return tokenHandler.WriteToken(token);
        }
    }
}
