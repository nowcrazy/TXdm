using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using SqlSugar;
using xdm_repository.Models;

namespace xdm_repository.Model
{
    public class sys_user : BaseEntity
    {
        public sys_user()
        {
        }
        [Key, Required]
        [Display(Name = "用户ID")]
        public int user_id { get; set; }
        public int dept_id { get; set; }
        public string? user_name { get; set; }
        public string? nick_name { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string? phonenumber { get; set; }
        public string? sex { get; set; }
        public string? avatar { get; set; }
        [DataType(DataType.Password)]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Password must be between 8 to 12 characters long.")]
        public string? password { get; set; }
        public string? status { get; set; }
        public string? del_flag { get; set; }
        public string? login_ip { get; set; }
        public DateTime? login_date { get; set; }
        public string? remark { get; set; }
    }
}

