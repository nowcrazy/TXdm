using System;
using System.Security.Claims;
using SqlSugar;
using xdm_model.DTO;
using xdm_repository.SqlSugar;

namespace xdm_api.Server
{
    public class SecurityUtils
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly SqlSugarClient _db;

        public SecurityUtils(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _db = new SugarDbcontext(_configuration).GetInstance();
        }
        public ClaimsPrincipal GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext.User;
        }

        public IEnumerable<Claim> GetCurrentUserClaims()
        {
            var user = GetCurrentUser();
            return user?.Claims;
        }

        public List<SysUser> getUser()
        {
            var user = GetCurrentUser();
            UserClaims claims = new UserClaims();
            List<SysUser> users = new List<SysUser>();
            Dept depts = new Dept();
            List<Role> roles = new List<Role>();
            if (user.Identity.IsAuthenticated)
            {
                foreach (Claim claim in user.Claims)
                {
                    if (claim.Type == "username" && claim.Value.Trim() != "")
                    {
                        claims.username = claim.Value;
                    }
                    if (claim.Type == "uuid" && claim.Value.Trim() != "")
                    {
                        claims.uuid = claim.Value;
                    }
                    if (claim.Type == "code" && claim.Value.Trim() != "")
                    {
                        claims.code = claim.Value;
                    }

                    // 处理每个声明
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }
            }
            if (claims != null)
            {

                string sql = @$" select u.user_id, u.dept_id, u.user_name, u.nick_name, u.email, u.avatar, u.phonenumber, u.password, u.sex, u.status, u.del_flag, u.login_ip, u.login_date, u.create_by, u.create_time, u.remark, 
                                    d.dept_id, d.parent_id, d.ancestors, d.dept_name, d.order_num, d.leader, d.status as dept_status,
                                    r.role_id, r.role_name, r.role_key, r.role_sort, r.data_scope, r.status as role_status
                                from sys_user u
		                        left join sys_dept d on u.dept_id = d.dept_id
		                        left join sys_user_role ur on u.user_id = ur.user_id
		                        left join sys_role r on r.role_id = ur.role_id
				                where u.user_name = {claims.username} and u.del_flag = '0' ";
                users = _db.Ado.SqlQuery<SysUser>(sql);
                return users;
            }

            return users;
        }
    }
}

