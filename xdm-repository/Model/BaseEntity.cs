using System;
using System.Data.SqlTypes;
using System.Security.Principal;
using SqlSugar;

namespace xdm_repository.Models
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            create_time = DateTime.Now;
            IsDeleted = false;
        }
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public string create_by { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime create_time { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, IsNullable = true)]
        public string modify_by { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, IsNullable = true)]
        public DateTime? modify_time { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}


