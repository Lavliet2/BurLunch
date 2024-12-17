using Microsoft.EntityFrameworkCore;
using BurLunch.AuthAPI.Models;
using BurLunch.AuthAPI.Utils;

namespace BurLunch.AuthAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishType> DishTypes { get; set; }
        public DbSet<WeeklyMenuCard> WeeklyMenuCards { get; set; }
        public DbSet<DailyMenu> DailyMenus { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Связь Dish -> DishType
            modelBuilder.Entity<Dish>()
                .HasOne(d => d.DishType)
                .WithMany(dt => dt.Dishes)
                .HasForeignKey(d => d.DishTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление DishType

            // Начальные данные для DishType
            modelBuilder.Entity<DishType>().HasData(
                new DishType { Id = 3, Name = "Салат" },
                new DishType { Id = 1, Name = "Суп" },
                new DishType { Id = 2, Name = "Горячие" },
                new DishType { Id = 4, Name = "Напиток" }
            );

            // Начальные данные для User
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "Admin",
                PasswordHash = PasswordHasher.HashPassword("Admin"),
                Role = "Administrator"
            });
        }
    }
}
