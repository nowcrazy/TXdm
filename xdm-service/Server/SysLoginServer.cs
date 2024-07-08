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
        string GenerateCaptchaImageAsBase64(string uuid);
        List<(bool, string)> login(LoginBody loginBody);
        bool logout(string name);
        Task<IEnumerable<sys_user>> GetUsername(string username);
        Task<List<MenuItem>> GetRouters(string username);
        Task<List<string>> GetPermissions(string username);
    }
}

