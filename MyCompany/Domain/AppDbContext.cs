using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCompany.Domain.Entities;

namespace MyCompany.Domain
{
    public class AppDbContext : IdentityDbContext<IdentityUser> //стандартный класс для пользователя
    {
        public DbSet<ServiceCategory> ServiceCategories { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            string adminName = "admin";
            string roleAdminId = "CD37CD19-2F81-4737-A1D3-95A95869A21B" ;
            string userAdminId = "ACCFA7D5-998D-42DE-ABB2-F94979E32468";

            //Создание роли админа
            builder.Entity<IdentityRole>().HasData(new IdentityRole()
            {
                Id = roleAdminId,
                Name = adminName,
                NormalizedName = adminName.ToUpper()
            });

            //Добавление нового IdentityUser в качестве админа
            builder.Entity<IdentityUser>().HasData(new IdentityUser()
            {
                Id = userAdminId,
                UserName = adminName,
                NormalizedUserName = adminName.ToUpper(),
                Email = "admin@admin.com",
                NormalizedEmail = "admin@admin.com",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(new IdentityUser(), adminName),
                SecurityStamp = string.Empty,
                PhoneNumberConfirmed = true
            });

            //Определение роли админа
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>()
            { 
                RoleId =roleAdminId,
                UserId = userAdminId,
            });
        }
    }
}
