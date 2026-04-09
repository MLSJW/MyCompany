using MyCompany.Infrastructure;

namespace MyCompany
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            
            //подключение json и дефолтных путей
            IConfigurationBuilder configBuild = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = configBuild.Build();
            AppConfig config = configuration.GetSection("Project").Get < AppConfig > ()!;
            

            //Контроллеры
            builder.Services.AddControllersWithViews();


            //Сборка конфига
            WebApplication app = builder.Build();
            //app.MapGet("/", () => "Hello World!");
            //Подключение статичных файлов из wwwroot
            app.UseStaticFiles();

            //Маршрутизация
            app.UseRouting();

            //Регистрация маршрутов
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}
