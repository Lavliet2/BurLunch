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
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление DishType

            // Начальные данные для DishType
            modelBuilder.Entity<DishType>().HasData(
                new DishType { Id = 3, Name = "Салат" },
                new DishType { Id = 1, Name = "Суп" },
                new DishType { Id = 2, Name = "Горячие" },
                new DishType { Id = 4, Name = "Напиток" }
            );

            // Связь TableReservation -> Schedule
            modelBuilder.Entity<TableReservation>()
                .HasOne(tr => tr.Schedule)
                .WithMany(s => s.TableReservations)
                .HasForeignKey(tr => tr.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь TableReservation -> Table
            modelBuilder.Entity<TableReservation>()
                .HasOne(tr => tr.Table)
                .WithMany()
                .HasForeignKey(tr => tr.TableId)
                .OnDelete(DeleteBehavior.Restrict);

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
            // Добавляем запись о бронировании стола пользователем Admin
            //modelBuilder.Entity<TableReservation>().HasData(
            //    new TableReservation { Id = 1, TableId = 9999, UserId = 1 }
            //);

            // Добавляем начальные блюда
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

            // Инициализация карточки меню на неделю (Бизнес-ланч #1)
            modelBuilder.Entity<WeeklyMenuCard>().HasData(
                new WeeklyMenuCard { Id = 9999, Name = "Бизнес-ланч #9999" }
            );

            //// Связь блюд с карточкой меню (таблица многие-ко-многим)
            //modelBuilder.Entity("WeeklyMenuCardDish").HasData(
            //    new { WeeklyMenuCardId = 1, DishesId = 1 },
            //    new { WeeklyMenuCardId = 1, DishesId = 2 },
            //    new { WeeklyMenuCardId = 1, DishesId = 3 },
            //    new { WeeklyMenuCardId = 1, DishesId = 4 },
            //    new { WeeklyMenuCardId = 1, DishesId = 5 },
            //    new { WeeklyMenuCardId = 1, DishesId = 6 },
            //    new { WeeklyMenuCardId = 1, DishesId = 7 },
            //    new { WeeklyMenuCardId = 1, DishesId = 8 },
            //    new { WeeklyMenuCardId = 1, DishesId = 9 },
            //    new { WeeklyMenuCardId = 1, DishesId = 10 }
            //);

            //// Связь блюд с карточкой меню
            //modelBuilder.Entity<Dish>().HasData(
            //    new Dish { Id = 1, Name = "Ачичук", Description = "Из свежих овощей", DishTypeId = 3 },
            //    new Dish { Id = 2, Name = "Оливье", Description = "", DishTypeId = 3 },
            //    new Dish { Id = 3, Name = "Деревенский", Description = "", DishTypeId = 3 },
            //    new Dish { Id = 4, Name = "Чучвара", Description = "По домашнему с фрикадельками и лапшой", DishTypeId = 1 },
            //    new Dish { Id = 5, Name = "Мастава", Description = "Грузинский суп с куриным филе", DishTypeId = 1 },
            //    new Dish { Id = 6, Name = "Стейк из горбуши", Description = "С овощным миксом", DishTypeId = 2 },
            //    new Dish { Id = 7, Name = "Плов", Description = "Из курицы", DishTypeId = 2 },
            //    new Dish { Id = 8, Name = "Дамляма", Description = "Из курицы", DishTypeId = 2 },
            //    new Dish { Id = 9, Name = "Чай", Description = "Напиток дня", DishTypeId = 4 },
            //    new Dish { Id = 10, Name = "Морс", Description = "Из клюквы", DishTypeId = 4 }
            //);
        }
    }
}
