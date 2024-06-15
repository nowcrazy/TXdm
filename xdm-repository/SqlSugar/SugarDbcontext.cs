using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace xdm_repository.SqlSugar
{
    public class SugarDbcontext
    {
        private readonly IConfiguration _configuration;
        public SugarDbcontext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public SqlSugarClient GetInstance()
        {
            //// 假设你有某种方式可以获取到当前租户的信息
            //var tenantId = GetCurrentTenantId();
            //var connectionString = GetTenantConnectionString(tenantId);

            //var db = new SqlSugarClient(new ConnectionConfig()
            //{
            //    ConnectionString = connectionString,
            //    DbType = DbType.MySql, // 根据实际数据库类型设置
            //    IsAutoCloseConnection = true,
            //    InitKeyType = InitKeyType.Attribute
            //});
            string conn = _configuration.GetConnectionString("Main");

            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = conn,//"server=192.168.5.39;port=3306;database=xdm;user=root;password=mysql_889",
                DbType = DbType.MySql, // 根据实际数据库类型设置
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            }); ;

            return db;
        }



    }

}
