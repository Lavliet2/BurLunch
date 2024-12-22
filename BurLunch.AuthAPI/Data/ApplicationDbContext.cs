using Microsoft.EntityFrameworkCore;
using BurLunch.AuthAPI.Models;
using BurLunch.AuthAPI.Utils;

namespace BurLunch.AuthAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishType> DishTypes { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TableReservation> TableReservations { get; set; }
        public DbSet<WeeklyMenuCard> WeeklyMenuCards { get; set; }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Связь Dish -> DishType
            modelBuilder.Entity<Dish>()
                .HasOne(d => d.DishType)
                .WithMany(dt => dt.Dishes)
                .HasForeignKey(d => d.DishTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DishType>().HasData(
                new DishType { Id = 3, Name = "Салат" },
                new DishType { Id = 1, Name = "Суп" },
                new DishType { Id = 2, Name = "Горячие" },
                new DishType { Id = 4, Name = "Напиток" }
            );

            modelBuilder.Entity<TableReservation>()
                .HasMany(tr => tr.SelectedDishes)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "TableReservationDish", 
                    j => j
                        .HasOne<Dish>() 
                        .WithMany()
                        .HasForeignKey("DishId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j
                        .HasOne<TableReservation>()
                        .WithMany()
                        .HasForeignKey("TableReservationId")
                        .OnDelete(DeleteBehavior.Cascade)
            );

            // Связь TableReservation -> Schedule
            modelBuilder.Entity<TableReservation>()
                .HasOne(tr => tr.Schedule)
                .WithMany(s => s.TableReservations)
                .HasForeignKey(tr => tr.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь TableReservation -> Table
            modelBuilder.Entity<TableReservation>()
                .HasOne(tr => tr.Table)
                .WithMany()
                .HasForeignKey(tr => tr.TableId)
                .OnDelete(DeleteBehavior.Cascade);

            // Начальные данные для User
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "Admin",
                PasswordHash = PasswordHasher.HashPassword("Admin"),
                Role = "Administrator"
            });

            // Добавляем начальный стол
            modelBuilder.Entity<Table>().HasData(
                new Table { Id = 9999, Seats = 4, Description = "Стол 1 из 4 мест" }
            );

            modelBuilder.Entity<Dish>().HasData(
                new Dish { Id = 1, Name = "Ачичук", Description = "Из свежих овощей", DishTypeId = 3 },
                new Dish { Id = 2, Name = "Оливье", Description = "", DishTypeId = 3 },
                new Dish { Id = 3, Name = "Деревенский", Description = "", DishTypeId = 3 },
                new Dish { Id = 4, Name = "Чучвара", Description = "По домашнему с фрикадельками и лапшой", DishTypeId = 1 },
                new Dish { Id = 5, Name = "Мастава", Description = "Грузинский суп с куриным филе", DishTypeId = 1 },
                new Dish { Id = 6, Name = "Стейк из горбуши", Description = "С овощным миксом", DishTypeId = 2 },
                new Dish { Id = 7, Name = "Плов", Description = "Из курицы", DishTypeId = 2 },
                new Dish { Id = 8, Name = "Дамляма", Description = "Из курицы", DishTypeId = 2 },
                new Dish { Id = 9, Name = "Чай", Description = "Напиток дня", DishTypeId = 4 },
                new Dish { Id = 10, Name = "Морс", Description = "Из клюквы", DishTypeId = 4 }
            );

            modelBuilder.Entity<WeeklyMenuCard>().HasData(
                new WeeklyMenuCard { Id = 9999, Name = "Бизнес-ланч #9999" }
            );
        }
    }
}
