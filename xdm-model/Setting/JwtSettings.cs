using System;
namespace xdm_model.Setting
{
    public class JwtSettings
    {
        public JwtSettings()
        {
        }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? SecretKey { get; set; }
        public int Expire { get; set; }
        public int RefreshTokenTime { get; set; }
        public string? TokenType { get; set; }
    }
}

