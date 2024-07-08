using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Repository.Helper;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SqlSugar;
using StackExchange.Redis;
using xdm_model.DTO;
using xdm_repository.Model;
using xdm_repository.SqlSugar;
using xdm_service.Server;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace xdm_service.Service
{
    public class SysLoginService : SysLoginServer
    {
        private readonly JwtUtil _jwtUtil;
        private readonly RedisUtil _redisUtil;
        private readonly SqlSugarClient _db;
        private readonly IConfiguration _configuration;

        public SysLoginService(JwtUtil jwtUtil, RedisUtil redisUtil, IConfiguration configuration)
        {
            _jwtUtil = jwtUtil;
            _redisUtil = redisUtil;
            _configuration = configuration;
            _db = new SugarDbcontext(_configuration).GetInstance();
        }
        public string GenerateCaptchaImageAsBase64(string uuid)
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

            //captchaAnswer = answer.ToString();
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
            //return "data:image/png;base64," + base64CaptchaImage;

            //存储token
            var res = _redisUtil.GetDatabase();
            var captcha_codes = "captcha_codes:" + uuid;
            //res.SetAdd(captcha_codes, token);
            res.StringGetSet(captcha_codes, answer.ToString());
            return base64CaptchaImage;
        }

        public async Task<List<MenuItem>> GetRouters(string username)
        {
            List<MenuItem> listobj = new List<MenuItem>();
            string sql = @$"	select distinct m.menu_id, m.parent_id, m.menu_name, m.path, m.component, m.`query`, m.visible, m.status, ifnull(m.perms,'') as perms, m.is_frame, m.is_cache, m.menu_type, m.icon, m.order_num, m.create_time
		                            from sys_menu m
		                            	 left join sys_role_menu rm on m.menu_id = rm.menu_id
		                            	 left join sys_user_role ur on rm.role_id = ur.role_id
		                            	 left join sys_role ro on ur.role_id = ro.role_id
		                            	 left join sys_user u on ur.user_id = u.user_id
		                            where u.user_id = 2 and m.menu_type in ('M', 'C') and m.status = 0  AND ro.status = 0
		                            order by m.parent_id, m.order_num";
            listobj = await _db.Ado.SqlQueryAsync<MenuItem>(sql);
            return listobj;
        }


        public async Task<User> GetUser(string username)
        {
            string sql = @$"	";
            //var menus = await _db.Ado.SqlQueryAsync<Menu>(sql);
            return null; ;
        }

        public async Task<IEnumerable<sys_user>> GetUsername(string username)
        {

            return null;
        }

        public List<(bool, string)> login(LoginBody loginbody)
        {
            //List<string> result = new List<string>();
            var result = new List<(bool, string)>();

            //登录前验证
            string valmsg = validateCaptcha(loginbody.username, loginbody.uuid, loginbody.code);
            //
            string checkmsg = loginPreCheck(loginbody.username, loginbody.password);
            if (valmsg == "" && checkmsg == "")
            {
                //生成token
                var token = _jwtUtil.IssueJwt(loginbody);
                //存储token
                var res = _redisUtil.GetDatabase();
                var captcha_codes = "captcha_codes:" + loginbody.uuid;
                //res.SetAdd(captcha_codes, token);
                res.StringGetSet(captcha_codes, token);
                result.Add((true, token));
            }
            else
            {
                string msg = "";
                if (valmsg == "")
                {
                    msg = checkmsg;
                }
                else if (checkmsg == "")
                {
                    msg = valmsg;
                }
                result.Add((false, msg));
            }

            return result;
        }
        public string validateCaptcha(string username, string uuid, string code)
        {
            string verifyKey = "captcha_codes:" + uuid;
            string captcha = _redisUtil.GetDatabase().StringGet(verifyKey);
            string msg = "";
            if (captcha == null)
            {
                msg = "验证码失效";
                //throw new Exception("验证码失效");
            }
            if (code != captcha)
            {
                msg = "验证码错误";
                //throw new Exception("验证码错误");
            }
            return msg;
        }
        public string loginPreCheck(string username, string password)
        {
            string msg = "";
            // 用户名或密码为空 错误
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                msg = "用户名或密码为空";
                //throw new Exception("用户名或密码为空");
            }
            // 密码如果不在指定范围内 错误
            if (password.Length < 5
                    || password.Length > 20)
            {
                msg = "密码如果不在指定范围内";
                //throw new Exception("密码如果不在指定范围内");
            }
            // 用户名不在指定范围内 错误
            if (username.Length < 2
                    || username.Length > 20)
            {
                msg = "用户名不在指定范围内";
                //throw new Exception("用户名不在指定范围内");
            }
            // IP黑名单校验
            //String blackStr = configService.selectConfigByKey("sys.login.blackIPList");
            //if (IpUtils.isMatchedIp(blackStr, IpUtils.getIpAddr()))
            //{
            //    throw new Exception("");
            //}
            return msg;
        }

        public bool logout(string captcha_codes)
        {

            string redisres = _redisUtil.GetDatabase().StringGet("login_token").ToString();
            if (redisres.Trim() == "")
            {
                return false;
            }
            else
            {
                var token = _redisUtil.GetDatabase().StringGetDelete("login_token");
                return true;
            }
            //RedisValue[] members = _redisUtil.GetDatabase().SetMembers("login_token");
            //if (members.Count() < 0)
            //{
            //    return false;
            //}
            //else
            //{
            //    var tokenval = members.Contains("login_token");
            //    return true;
            //}

        }

        public Task<List<string>> GetPermissions(string username)
        {

            return null;
        }
    }
}

