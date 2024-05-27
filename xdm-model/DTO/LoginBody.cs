using System;
namespace xdm_model.DTO
{
    public class LoginBody
    {
        public LoginBody()
        {
        }
        public string username { get; set; }
        public string password { get; set; }
        public string code { get; set; }
        public string uuid { get; set; }
    }
}

