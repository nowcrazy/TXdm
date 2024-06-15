using System;
namespace xdm_repository.Dapper
{
    public class DapperDbcontext
    {
        public DapperDbcontext()
        {
        }
    }
}
#region 废弃代码
/// <summary>
/// dapper--mysql _connection = _configuration.GetConnectionString("MySqlConnection");
/// </summary>
/// <returns></returns>
//public async Task<IEnumerable<sys_user>> GetUs()
//{
//    using (IDbConnection db = new MySqlConnection(_connection))
//    {
//        String sql = "select * from sys_user ";
//        var emps = await db.QueryAsync<sys_user>(sql);
//        return emps;
//    }
//}
#endregion

