using System;
using Microsoft.AspNetCore.Mvc;
using Repository.Helper;
using xdm_model.DTO;
using xdm_repository.Server;
using xdm_service.Server;

namespace xdm_api.Controllers.System
{
    [Route("api")]
    [ApiController]
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
        [HttpPost("login")]
        public AjaxResult Login(LoginBody loginBody)
        {
            AjaxResult ajax = AjaxResult.Success();
            var answer = HttpContext.Session.GetString("CaptchaCode");
            String token = _SysLoginServer.login(loginBody.username, loginBody.password, loginBody.code, loginBody.uuid, answer);

            ajax.Add("xdmtoken", token);
            return ajax;
        }

        [HttpGet("captchaImage")]
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

