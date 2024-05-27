using System;
using System.Net.Http;
using Microsoft.VisualBasic;
using Repository.Helper;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using xdm_service.Server;

namespace xdm_service.Service
{
    public class SysLoginService : SysLoginServer
    {
        private readonly JwtUtil _jwtUtil;
        private readonly RedisUtil _redisUtil;
        public SysLoginService(JwtUtil jwtUtil, RedisUtil redisUtil)
        {
            _jwtUtil = jwtUtil;
            _redisUtil = redisUtil;
        }
        public string GenerateCaptchaImageAsBase64(out string captchaAnswer)
        {
            var rng = new Random();
            var num1 = rng.Next(1, 10);
            var num2 = rng.Next(1, 10);
            var operation = new[] { "+", "-", "*" }[rng.Next(3)];
            var answer = operation switch
            {
                "+" => num1 + num2,
                "-" => num1 - num2,
                "*" => num1 * num2,
                _ => 0
            };

            captchaAnswer = answer.ToString();
            var captchaCode = $"{num1} {operation} {num2} = ?";

            var font = SystemFonts.CreateFont("Arial", 12, FontStyle.Regular); // This line creates a Font object
            using var image = new Image<Rgba32>(100, 50);
            image.Mutate(ctx => ctx
                .Fill(Color.White)
                .DrawText(captchaCode, font, Color.Black, new PointF(10, 10))
            );

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var base64CaptchaImage = Convert.ToBase64String(ms.ToArray());
            //return base64CaptchaImage;
            return "data:image/png;base64," + base64CaptchaImage;
        }

        public string login(string name, string pwd, string code, string uuid, string answer)
        {
            //验证码验证
            if (code != answer)
            {
                throw new Exception("验证码错误");
            }
            //登录前验证
            loginCheck(name, pwd);
            //授权认证

            //生成token
            var token = _jwtUtil.CreateToken(name, pwd, code, uuid);
            //存储token
            var res = _redisUtil.GetDatabase();
            res.SetAdd("xdmtoken", token);
            return token;
        }
        public void loginCheck(string username, string password)
        {
            // 用户名或密码为空 错误
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("登录失败");
            }
            // 密码如果不在指定范围内 错误
            if (password.Length > 12 && password.Length < 2)
            {
                throw new Exception("密码长度2-12，请检查");
            }
            // 用户名不在指定范围内 错误
            if (username.Length > 12 && username.Length < 2)
            {
                throw new Exception("用户名2-10，请检查");
            }
            // IP黑名单校验

        }
    }
}

