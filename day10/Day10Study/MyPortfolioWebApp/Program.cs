using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyPortfolioWebApp.Models;
using System.Runtime;

namespace MyPortfolioWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // DB연결 초기화 설정
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
                builder.Configuration.GetConnectionString("SmartHomeConnection"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SmartHomeConnection"))
            ));

            // ASP.NET Core Identity 설정
            // 원본은 IdentityUser -> CustomUser 로 변경
            builder.Services.AddIdentity<CustomUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 패스워드 정책
            // 변경 전. 최대 6자리 이상, 특수문자 1개 포함, 영어대소문자 포함
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // 이런 암호 간단화는 되도록 하지말 것
                options.Password.RequiredLength = 4; // 최소길이 4자리
                options.Password.RequireNonAlphanumeric = false; // 특수문자 사용안함
                options.Password.RequireUppercase = false; // 대문자 사용안함
                options.Password.RequireLowercase = false; // 소문자 사용안함
                options.Password.RequireDigit = false; // 숫자필수 여부
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();  // ASP.NET Core Identity 계정
            app.UseAuthorization();   // 권한

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
