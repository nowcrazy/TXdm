using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using Repository.Helper;
using xdm_api.Middleware;
using xdm_model.Models;
using xdm_repository.DBContext;
using xdm_service.Server;
using xdm_service.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// NLog日志配置
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(LogLevel.Trace);
}).UseNLog(); // 使用NLog作为日志提供程序

// 从配置中读取JWT设置并注册JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSetting"));

// 配置并注册JWT认证服务
builder.Services.AddSingleton(new JwtUtil(builder.Configuration));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSetting:Issuer"],
        ValidAudience = builder.Configuration["JwtSetting:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSetting:Key"])),
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

// 从配置中读取Redis设置并注册RedisUtil
builder.Services.AddSingleton(new RedisUtil(
    builder.Configuration.GetSection("RedisSetting:Default:Connection").Value,
    builder.Configuration.GetSection("RedisSetting:Default:InstanceName").Value,
    0
));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost",
                                              "http://www.contoso.com")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
});

// 添加控制器服务和Swagger服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加内存缓存和会话服务
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(3600); // 设置会话的超时时间
    options.Cookie.HttpOnly = true; // 设为true，增强会话cookie的安全性
    options.Cookie.IsEssential = true; // 标记会话cookie为必须的，以便在GDPR下工作
});

// 注册EF Core数据库上下文
builder.Services.AddDbContext<OperationContext>(option =>
{
    option.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"), new MySqlServerVersion(new Version(8, 0, 0)));
});

// 注册文件上传设置
builder.Services.Configure<sys_file>(builder.Configuration.GetSection("FileUploadSettings"));

// 注册应用服务和仓储
builder.Services.AddScoped<SysLoginServer, SysLoginService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 中间件配置顺序

app.UseHttpsRedirection(); // HTTPS 重定向中间件

app.UseStaticFiles(); // 静态文件中间件

app.UseRouting(); // 路由中间件

app.UseCors("MyAllowSpecificOrigins"); // CORS 中间件

app.UseAuthentication(); // 认证中间件
app.UseAuthorization(); // 授权中间件

app.UseSession(); // 启用会话

app.UseCustomErrorHandling(); // 添加自定义错误处理中间件

// 配置终结点路由
app.MapControllers();
app.Run();