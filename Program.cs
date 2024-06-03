using Microsoft.EntityFrameworkCore;
using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Areas.TelegramBot;
using DACN_DVTRUCTUYEN.Areas.VNPayAPI.Controllers;
using DACN_DVTRUCTUYEN.Areas.User.Services;
using DACN_DVTRUCTUYEN.Utilities;
namespace DACN_DVTRUCTUYEN
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // ... your other services
            var connection = "";
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.json");
                connection = builder.Configuration.GetConnectionString("ConnectionStrings");
            }
            else
            {
                connection = Environment.GetEnvironmentVariable("ConnectionStrings");
            }
            string nowpath = Directory.GetCurrentDirectory();
            builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));

            builder.Services.AddHostedService<OrderCancellationBackgroundService>();
            builder.Services.AddHostedService<OrderBackgroundService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            var function = new Functions();
            var telebot = new TelegramBotStatic();
            var vnpay = new VNPayStatic();
            app.Run();
        }
    }
}
