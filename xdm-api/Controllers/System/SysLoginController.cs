using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using xdm_api.Server;
using xdm_model.DTO;
using xdm_repository.Helper;
using xdm_service.Server;

namespace xdm_api.Controllers.System
{

    [Route("dev-api")]
    [ApiController]
    [Authorize]
    public class SysLoginController : ControllerBase
    {
        private readonly SysLoginServer _SysLoginServer;
        private readonly IConfiguration _configuration;
        private readonly SecurityUtils _securityUtils;
        public SysLoginController(IWebHostEnvironment env, SysLoginServer sysLoginserver, IConfiguration configuration, SecurityUtils securityUtils)
        {
            _SysLoginServer = sysLoginserver;
            _configuration = configuration;
            _securityUtils = securityUtils;
            //_uploadPath = Path.Combine(env.ContentRootPath, "uploads");
        }
        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("captchaImage")]
        [AllowAnonymous]
        public AjaxResult GetBase64CaptchaImage()
        {
            AjaxResult ajax = AjaxResult.Success();
            //是否开启验证
            bool captchaEnabled = Convert.ToBoolean(_configuration.GetSection("SysSetting:captchaEnabled").Value);
            ajax.Add("captchaEnabled", captchaEnabled);
            if (!captchaEnabled)
            {
                return ajax;
            }
            string captchaAnswer;
            //创建验证码图片，返回验证码计算答案存储到session
            string uuid = Guid.NewGuid().ToString().Replace("-", "");
            //redis存储uuid
            var base64Image = _SysLoginServer.GenerateCaptchaImageAsBase64(uuid);
            ajax.Add("uuid", uuid);
            ajax.Add("img", base64Image);
            //HttpContext.Response.StatusCode = 200;
            //return Ok(new { Image = base64Image });
            return ajax;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginBody"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public AjaxResult Login(LoginBody loginBody)
        {

            var res = _SysLoginServer.login(loginBody);
            if (res[0].Item1 == false)
            {
                AjaxResult ajax = AjaxResult.Error(500, res[0].Item2);
                return ajax;
            }
            else
            {
                AjaxResult ajax = AjaxResult.Success();
                ajax.Add("token", res[0].Item2);
                return ajax;
            }


        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        public AjaxResult logout()
        {
            AjaxResult ajax = AjaxResult.Success("退出成功", null);
            //var user = _httpContextAccessor.HttpContext.User; ;// GetUser();
            // 获取ClaimsPrincipal对象
            //var userClaims = User.Claims;
            //var userName = userClaims.FirstOrDefault(x => x.Type == "user_name")?.Value;
            //var claimuuidValue = user.Claims
            //                   .FirstOrDefault(c => c.Type == "uuid")?.Value;
            //var claimcodeValue = user.Claims
            //                   .FirstOrDefault(c => c.Type == "code")?.Value;
            //var claimusernameValue = user.Claims
            //                   .FirstOrDefault(c => c.Type == "username")?.Value;
            //var val = "captcha_codes:" + claimuuidValue;
            //_SysLoginServer.logout(val);
            return ajax;
        }

        [HttpGet("getInfo")]
        //[Authorize]
        public async Task<AjaxResult> GetInfoAsync()
        {
            AjaxResult ajax = AjaxResult.Success();
            var user = _securityUtils.getUser();



            return ajax;

        }

        [HttpGet("getRouters")]
        public async Task<AjaxResult> getRouter()
        {
            AjaxResult ajax = AjaxResult.Success();
            var userClaims = User.Claims;
            var userName = userClaims.FirstOrDefault(x => x.Type == "username")?.Value;

            List<MenuItem> listobj = new List<MenuItem>();
            listobj = await _SysLoginServer.GetRouters(userName);
            var tree = ExtendUtil.BuildTree(listobj);

            //var treeMenu = ConvertToTreeMenu(tree);
            //var json = JsonConvert.SerializeObject(treeMenu, Formatting.Indented);
            //Console.WriteLine(json);

            ajax.Add("data", tree);
            return ajax;
        }

        [HttpGet("register")]
        public AjaxResult Regregister(LoginBody loginBody)
        {
            AjaxResult ajax = AjaxResult.Success();
            var answer = HttpContext.Session.GetString("CaptchaCode");
            ajax.Add("", "");
            return ajax;
        }


    }
}

