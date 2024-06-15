using xdm_model.DTO;
using xdm_repository.Model;

namespace xdm_service.Server
{
    public interface SysLoginServer
    {
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="captchaAnswer">答案</param>
        /// <returns></returns>
        string GenerateCaptchaImageAsBase64(out string captchaAnswer);
        string login(string name, string pwd, string code, string uuid, string answer);
        string logout(string name);
        Task<User> GetUser(string username);
        Task<IEnumerable<sys_user>> GetUsername(string username);
        Task<IEnumerable<Meta>> GetRouters(string username);
    }
}

