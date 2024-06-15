using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using xdm_model.DTO;
using xdm_service.Server;

namespace xdm_api.Controllers.System
{

    [Route("api")]
    [ApiController]
    [Authorize]
    public class SysLoginController : ControllerBase
    {
        private readonly SysLoginServer _SysLoginServer;
        private readonly IConfiguration _configuration;

        public SysLoginController(IWebHostEnvironment env, SysLoginServer sysLoginserver, IConfiguration configuration)
        {
            _SysLoginServer = sysLoginserver;
            _configuration = configuration;

            //_uploadPath = Path.Combine(env.ContentRootPath, "uploads");
        }
        /// <summary>
        /// 测试授权认证
        /// </summary>
        /// <returns></returns>
        [HttpGet("get")]

        public ActionResult<string> Get()
        {
            return "这是一个受保护的资源";
        }

        /// <summary>
        /// 登录-存储token
        /// </summary>
        /// <param name="loginBody"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public AjaxResult Login(LoginBody loginBody)
        {
            AjaxResult ajax = AjaxResult.Success();
            var answer = HttpContext.Session.GetString("CaptchaCode");
            String token = _SysLoginServer.login(loginBody.username, loginBody.password, loginBody.code, loginBody.uuid, answer);

            ajax.Add(loginBody.username, token);
            return ajax;
        }

        [HttpGet("register")]
        public AjaxResult regregister(LoginBody loginBody)
        {
            AjaxResult ajax = AjaxResult.Success();
            var answer = HttpContext.Session.GetString("CaptchaCode");
            ajax.Add("", "");
            return ajax;
        }

        [HttpGet("getInfo")]
        //[Authorize]
        public async Task<AjaxResult> GetInfoAsync()
        {
            AjaxResult ajax = AjaxResult.Success();
            // 获取ClaimsPrincipal对象
            var userClaims = User.Claims;
            var userName = userClaims.FirstOrDefault(x => x.Type == "user_name")?.Value;
            //var userName = "admin";
            //var res = _SysLoginServer.GetUser(userName);
            var res = await _SysLoginServer.GetUser(userName);

            ajax.Add("user", res);
            return ajax;
        }
        [HttpPost("logout")]
        public AjaxResult logout()
        {
            AjaxResult ajax = AjaxResult.Success();
            // 获取ClaimsPrincipal对象
            var userClaims = User.Claims;
            var userName = userClaims.FirstOrDefault(x => x.Type == "user_name")?.Value;
            string res = _SysLoginServer.logout(userName);
            if (res != "退出失败")
            {
                ajax.Add("msg", "退出成功");
            }
            else
            {
                ajax.Add("msg", "退出成功");
            }

            return ajax;
        }

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
            var base64Image = _SysLoginServer.GenerateCaptchaImageAsBase64(out captchaAnswer);
            //redis存储uuid

            HttpContext.Session.SetString("CaptchaCode", captchaAnswer);

            ajax.Add("uuid", Guid.NewGuid());
            ajax.Add("img", base64Image);
            //HttpContext.Response.StatusCode = 200;
            //return Ok(new { Image = base64Image });
            return ajax;
        }


    }
}

