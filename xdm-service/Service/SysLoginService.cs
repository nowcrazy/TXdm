using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Repository.Helper;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SqlSugar;
using xdm_model.DTO;
using xdm_repository.Model;
using xdm_repository.SqlSugar;
using xdm_service.Server;

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

        public Task<IEnumerable<Meta>> GetRouters(string username)
        {
            return null;
        }

        public async Task<User> GetUser(string username)
        {
            User users = new User();
            string sql = @$" SELECT u.create_by AS createBy,u.create_time AS createTime,u.update_by AS updateBy,u.update_time AS updateTime,u.remark,
                            u.user_id AS userId,u.user_name AS userName,u.nick_name AS nickName,u.dept_id AS  DeptId,u.email,u.phonenumber,u.sex,u.avatar,u.password,
                            u.`status`,u.del_flag AS delFlag,u.login_ip AS loginIp,u.login_date AS loginDate
                            FROM sys_user u
                            WHERE u.login_name ='{username}';";
            //string sql = $"select user_id from sys_user where user_name='{username}'";
            users = await _db.Ado.SqlQuerySingleAsync<User>(sql);
            var deptid = users.DeptId;
            var userid = users.UserId;
            string deptsql = @$"SELECT create_by AS createBy,create_time AS createTime,update_by AS updateBy,update_time AS updateTime,dept_id AS deptId,
                                    parent_id AS parentId,ancestors ,dept_name AS deptName,order_num AS orderNum,leader ,phone ,email,`status`,del_flag
                                    FROM sys_dept where dept_id={deptid};";
            var deptdata = await _db.Ado.SqlQueryAsync<Dept>(deptsql);
            List<Role> roles = new List<Role>();
            string rolessql = @$"SELECT  r.create_by AS createBy,r.create_time AS createTime,r.update_by AS updateBy,r.update_time AS updateTime,r.role_id AS roleId,role_name AS roleName,role_sort AS roleSort,
                                        data_scope AS dataScope,`status`,del_flag
                                        FROM  sys_user_role ur left join sys_role r on r.role_id = ur.role_id
                                        WHERE user_id='{userid}';";
            roles = await _db.Ado.SqlQueryAsync<Role>(rolessql);
            if (username == "admin")
            {
                users.Admin = true;

            }
            foreach (var item in roles)
            {
                if (username == "admin")
                {
                    item.Admin = true;
                }
            }
            if (deptdata.Count > 0)
            {
                Dept dept = new Dept
                {
                    CreateBy = deptdata.FirstOrDefault().CreateBy,
                    CreateTime = deptdata.FirstOrDefault().CreateTime,
                    UpdateBy = deptdata.FirstOrDefault().UpdateBy,
                    UpdateTime = deptdata.FirstOrDefault().UpdateTime,
                    Remark = deptdata.FirstOrDefault().Remark,
                    DeptId = deptdata.FirstOrDefault().DeptId,
                    parentId = deptdata.FirstOrDefault().parentId,
                    ancestors = deptdata.FirstOrDefault().ancestors,
                    deptName = deptdata.FirstOrDefault().deptName,
                    leader = deptdata.FirstOrDefault().leader,
                    phone = deptdata.FirstOrDefault().phone,
                    email = deptdata.FirstOrDefault().email,
                    status = deptdata.FirstOrDefault().status,
                    delFlag = deptdata.FirstOrDefault().delFlag,

                };
                users.Dept = dept;
            }

            users.Roles = roles;

            var jsonString = JsonConvert.SerializeObject(users);
            return users; ;
        }

        public async Task<IEnumerable<sys_user>> GetUsername(string username)
        {

            return null;
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
            res.SetAdd(name, token);


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

        public string logout(string name)
        {

            string redisres = _redisUtil.GetDatabase().StringGet(name).ToString();
            if (redisres.Trim() == "")
            {
                throw new Exception("退出失败");
            }

            return redisres;
        }
    }
}

#region 废弃代码
//ConnectionMultiplexer conn = ConnectionMultiplexer.Connect("192.168.5.37:6379");
//IDatabase db = conn.GetDatabase();
//db.StringSet("userName", "admin");
//RedisValue redisValue = db.StringGet("userName");
//Console.WriteLine(redisValue);

//db.StringSet("key", DateTime.Now.ToString(), TimeSpan.FromSeconds(60));
#endregion