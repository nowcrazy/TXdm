using System;
namespace xdm_model.DTO
{
    public class User
    {

        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string? Remark { get; set; }
        //public Params? Params { get; set; }
        public int? UserId { get; set; }
        public int? DeptId { get; set; }
        public string? UserName { get; set; }
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string? Sex { get; set; }
        public string? Avatar { get; set; }
        public string? Password { get; set; }
        public string? Status { get; set; }
        public string? DelFlag { get; set; }
        public string? LoginIp { get; set; }
        public DateTime? LoginDate { get; set; }
        public Dept Dept { get; set; }
        public List<Role>? Roles { get; set; }
        public bool Admin { get; set; }

    }
    public class Dept
    {
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string? Remark { get; set; }
        public int DeptId { get; set; }
        public int parentId { get; set; }
        public string? ancestors { get; set; }
        public string? deptName { get; set; }
        public string? orderNum { get; set; }
        public string? leader { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public string? status { get; set; }
        public string? delFlag { get; set; }
    }

    public class Role
    {
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string? Remark { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? RoleKey { get; set; }
        public int RoleSort { get; set; }
        public string? DataScope { get; set; }
        public bool MenuCheckStrictly { get; set; }
        public bool DeptCheckStrictly { get; set; }
        public string? Status { get; set; }
        public string? DelFlag { get; set; }
        public bool Flag { get; set; }
        public bool Admin { get; set; }
    }
}

