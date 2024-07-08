using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using xdm_model.DTO;

namespace Repository.Helper
{
    public class JwtUtil
    {
        private readonly IConfiguration _configuration;

        public JwtUtil(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string IssueJwt(LoginBody loginbody)
        {
            var claims = new[]
            {
                new Claim("username",loginbody.username),
                new Claim("code",loginbody.code),
                new Claim("uuid",loginbody.uuid),

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
            //var jwtHandler = new JwtSecurityTokenHandler();
            //var encodedJwt = jwtHandler.WriteToken(jwtSecurityToken);
            //var user = SerializeJwt(token);
            return token;
        }
        public static LoginBody SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            LoginBody tokenModelJwt = new LoginBody();

            // token校验
            if (jwtHandler.CanReadToken(jwtStr))
            {

                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);

                Claim[] claimArr = jwtToken?.Claims?.ToArray();
                if (claimArr != null && claimArr.Length > 0)
                {
                    tokenModelJwt.username = claimArr.FirstOrDefault(a => a.Type == "user_name")?.Value;
                    tokenModelJwt.code = claimArr.FirstOrDefault(a => a.Type == "code")?.Value;
                    tokenModelJwt.uuid = claimArr.FirstOrDefault(a => a.Type == "uuid")?.Value;
                }
            }
            return tokenModelJwt;
        }
    }
}

