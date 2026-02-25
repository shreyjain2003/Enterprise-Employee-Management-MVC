using EmployeeDepartmentMVC.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EmployeeDepartmentMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 🔥 Create builder first
            var builder = WebApplication.CreateBuilder(args);

            // 🔥 Configure Serilog properly using builder.Configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:
                        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
                        "[SessionId: {SessionId}] " +
                        "[TraceId: {TraceId}] " +
                        "{Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            // 🔥 Replace default logging
            builder.Host.UseSerilog();

            // Add MVC
            builder.Services.AddControllersWithViews();

            // 🔥 Enable Session
            builder.Services.AddSession();

            // 🔥 Enable HTTP logging (optional but good)
            builder.Services.AddHttpLogging(options =>
            {
                options.LoggingFields =
                    Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
            });

            // InMemory Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("EmployeeDeptDB"));
            builder.Services.AddControllersWithViews();
            builder.Services.AddDistributedMemoryCache();


            var app = builder.Build();
       

            // Middleware pipeline

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // 🔥 Enable Session Middleware
            app.UseSession();

            app.Use(async (context, next) =>
            {
                // Force session to initialize
                if (!context.Session.Keys.Contains("SessionStarted"))
                {
                    context.Session.SetString("SessionStarted", "True");
                }

                using (Serilog.Context.LogContext.PushProperty("SessionId", context.Session.Id))
                using (Serilog.Context.LogContext.PushProperty("TraceId", context.TraceIdentifier))
                {
                    await next();
                }
            });

            // 🔥 Enable HTTP request logging
            app.UseHttpLogging();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Department}/{action=Index}/{id?}");

            app.Run();
        }
    }
}