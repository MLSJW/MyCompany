using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyCompany.Domain;
using MyCompany.Domain.Repositories.Abstract;
using MyCompany.Domain.Repositories.EntityFramework;
using MyCompany.Infrastructure;
using Serilog;
using System.IO;

namespace MyCompany
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // ПРИНУДИТЕЛЬНОЕ ЛОГИРОВАНИЕ ДЛЯ ДИАГНОСТИКИ
            try
            {
                Directory.CreateDirectory("/data"); // убедимся, что папка существует
                File.WriteAllText("/data/startup_log.txt", $"App started at {DateTime.Now}\n");
                File.AppendAllText("/data/startup_log.txt", $"Current directory: {Directory.GetCurrentDirectory()}\n");

                // Проверяем наличие файлов в /app
                if (Directory.Exists("/app"))
                    File.AppendAllText("/data/startup_log.txt", $"Files in /app: {string.Join(", ", Directory.GetFiles("/app"))}\n");
                else
                    File.AppendAllText("/data/startup_log.txt", $"Directory /app does NOT exist!\n");

                File.AppendAllText("/data/startup_log.txt", $"appsettings.json exists: {File.Exists("/app/appsettings.json")}\n");
            }
            catch (Exception ex)
            {
                // Если даже логирование упало — хотя бы попытаемся
                Console.WriteLine($"Logging error: {ex.Message}");
            }

            try
            {
                WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
                builder.WebHost.UseUrls("http://0.0.0.0:80");

                //подключение json и дефолтных путей
                IConfigurationBuilder configBuild = new ConfigurationBuilder()
                    .SetBasePath(builder.Environment.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();

                IConfiguration configuration = configBuild.Build();
                AppConfig config = configuration.GetSection("Project").Get<AppConfig>()!;

                //Подключение контекса БД
                builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(config.Database.ConnectionString).ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

                builder.Services.AddTransient<IServiceCategoriesRepository, EFServiceCategoriesRepository>();
                builder.Services.AddTransient<IServicesRepository, EFServicesRepository>();
                builder.Services.AddTransient<DataManager>();

                //Настройка Identity системы
                builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = false;
                }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

                //Настройка куки авторизации
                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.Cookie.Name = "myCompanyAuth";
                    options.Cookie.HttpOnly = true;
                    options.LoginPath = "/account/login";
                    options.AccessDeniedPath = "/admin/accessdenied";
                    options.SlidingExpiration = true;
                });

                //Контроллеры
                builder.Services.AddControllersWithViews();
                builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

                //Сборка конфига
                WebApplication app = builder.Build();

                // ДИАГНОСТИЧЕСКИЙ ENDPOINT (проверка, что приложение живое)
                app.MapGet("/health", () => "OK - Application is running!");

                app.UseSerilogRequestLogging();
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                //Подключение статичных файлов из wwwroot
                app.UseStaticFiles();

                //Маршрутизация
                app.UseRouting();

                //Подключение авторизации и аутентификации
                app.UseCookiePolicy();
                app.UseAuthentication();
                app.UseAuthorization();

                //Регистрация маршрутов
                app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                // Записываем ошибку в файл
                try
                {
                    Directory.CreateDirectory("/data");
                    File.AppendAllText("/data/error_log.txt", $"{DateTime.Now}: {ex.ToString()}\n");
                }
                catch { }

                // Также пишем в консоль (попадёт в логи Amvera)
                Console.WriteLine($"FATAL ERROR: {ex.ToString()}");
                throw;
            }
        }
    }
}