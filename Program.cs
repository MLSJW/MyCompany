using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyCompany.Domain;
using MyCompany.Domain.Entities;
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
            // ПРИНУДИТЕЛЬНОЕ ЛОГИРОВАНИЕ
            try
            {
                Directory.CreateDirectory("/data");
                File.WriteAllText("/data/startup_log.txt", $"App started at {DateTime.Now}\n");
                File.AppendAllText("/data/startup_log.txt", $"Current directory: {Directory.GetCurrentDirectory()}\n");

                if (Directory.Exists("/app"))
                    File.AppendAllText("/data/startup_log.txt", $"Files in /app count: {Directory.GetFiles("/app").Length}\n");

                File.AppendAllText("/data/startup_log.txt", $"appsettings.json exists: {File.Exists("/app/appsettings.json")}\n");
            }
            catch (Exception ex)
            {
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

                // ПРОВЕРКА: если строка подключения невалидна или БД недоступна, приложение не падает при старте
                try
                {
                    //Подключение контекса БД
                    builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(config.Database.ConnectionString).ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

                    builder.Services.AddTransient<IServiceCategoriesRepository, EFServiceCategoriesRepository>();
                    builder.Services.AddTransient<IServicesRepository, EFServicesRepository>();
                    builder.Services.AddTransient<DataManager>();

                    File.AppendAllText("/data/startup_log.txt", $"DB configured with connection string: {config.Database.ConnectionString}\n");
                }
                catch (Exception dbEx)
                {
                    File.AppendAllText("/data/error_log.txt", $"DB Configuration Error: {dbEx.Message}\n");
                    Console.WriteLine($"DB Configuration Error: {dbEx.Message}");
                    // Регистрируем репозитории-заглушки, чтобы приложение не упало
                    builder.Services.AddSingleton<IServiceCategoriesRepository, NullServiceCategoriesRepository>();
                    builder.Services.AddSingleton<IServicesRepository, NullServicesRepository>();
                    builder.Services.AddSingleton<DataManager>();
                }

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

                // Диагностический endpoint
                app.MapGet("/health", () => "OK - Application is running!");
                app.MapGet("/check-db", async () =>
                {
                    try
                    {
                        using var scope = app.Services.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        await db.Database.CanConnectAsync();
                        return "Database connection OK";
                    }
                    catch (Exception ex)
                    {
                        return $"Database error: {ex.Message}";
                    }
                });

                app.UseSerilogRequestLogging();

                // Включаем детальные ошибки для диагностики
                app.UseDeveloperExceptionPage();

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
                try
                {
                    Directory.CreateDirectory("/data");
                    File.AppendAllText("/data/error_log.txt", $"{DateTime.Now}: {ex.ToString()}\n");
                }
                catch { }

                Console.WriteLine($"FATAL ERROR: {ex.ToString()}");
                throw;
            }
        }
    }

    // Заглушки для репозиториев, если БД недоступна
    // Заглушки для репозиториев, если БД недоступна
    public class NullServiceCategoriesRepository : IServiceCategoriesRepository
    {
        // Синхронные методы (если есть)
        public IQueryable<ServiceCategory> GetServiceCategories() => new List<ServiceCategory>().AsQueryable();
        public ServiceCategory GetServiceCategoryById(int id) => null!;
        public ServiceCategory GetServiceCategoryByCodeWord(string codeWord) => null!;
        public void SaveServiceCategory(ServiceCategory entity) { }
        public void DeleteServiceCategory(int id) { }

        // Асинхронные методы
        public async Task<ServiceCategory?> GetServiceCategoryByIdAsync(int id) => await Task.FromResult(null as ServiceCategory);
        public async Task<ServiceCategory?> GetServiceCategoryByCodeWordAsync(string codeWord) => await Task.FromResult(null as ServiceCategory);
        public async Task<IEnumerable<ServiceCategory>> GetServiceCategoriesAsync() => await Task.FromResult(new List<ServiceCategory>().AsEnumerable());
        public async Task SaveServiceCategoryAsync(ServiceCategory entity) => await Task.CompletedTask;
        public async Task DeleteServiceCategoryAsync(int id) => await Task.CompletedTask;
    }

    public class NullServicesRepository : IServicesRepository
    {
        // Синхронные методы (если есть)
        public IQueryable<Service> GetServices() => new List<Service>().AsQueryable();
        public Service GetServiceById(int id) => null!;
        public Service GetServiceByCodeWord(string codeWord) => null!;
        public void SaveService(Service entity) { }
        public void DeleteService(int id) { }

        // Асинхронные методы
        public async Task<Service?> GetServiceByIdAsync(int id) => await Task.FromResult(null as Service);
        public async Task<Service?> GetServiceByCodeWordAsync(string codeWord) => await Task.FromResult(null as Service);
        public async Task<IEnumerable<Service>> GetServicesAsync() => await Task.FromResult(new List<Service>().AsEnumerable());
        public async Task SaveServiceAsync(Service entity) => await Task.CompletedTask;
        public async Task DeleteServiceAsync(int id) => await Task.CompletedTask;
    }
}