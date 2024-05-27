using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Repository.Helper
{
    public class JwtUtil
    {
        private readonly IConfiguration _configuration;

        public JwtUtil(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(string user_name, string password, string code, string uuid)
        {
            var claims = new[]
            {
                new Claim("user_name",user_name),
                new Claim("password",password),
                new Claim("code",code),
                new Claim("uuid",uuid),

            };
            //获取appsetting中jwt中的SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSetting:Key"]));
            //选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;
            //生成Credentials
            var signCredentials = new SigningCredentials(secretKey, algorithm);
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["JwtSetting:Issuer"],
                _configuration["JwtSetting:Audience"],
                 claims,
                 DateTime.Now,
                 DateTime.Now.AddSeconds(30),
                 signCredentials
                );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
    }
}

