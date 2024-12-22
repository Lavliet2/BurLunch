﻿// <auto-generated />
using System;
using BurLunch.AuthAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BurLunch.AuthAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241222130346_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BurLunch.AuthAPI.Models.Dish", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DishTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DishTypeId");

                    b.ToTable("Dishes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Из свежих овощей",
                            DishTypeId = 3,
                            Name = "Ачичук"
                        },
                        new
                        {
                            Id = 2,
                            Description = "",
                            DishTypeId = 3,
                            Name = "Оливье"
                        },
                        new
                        {
                            Id = 3,
                            Description = "",
                            DishTypeId = 3,
                            Name = "Деревенский"
                        },
                        new
                        {
                            Id = 4,
                            Description = "По домашнему с фрикадельками и лапшой",
                            DishTypeId = 1,
                            Name = "Чучвара"
                        },
                        new
                        {
                            Id = 5,
                            Description = "Грузинский суп с куриным филе",
                            DishTypeId = 1,
                            Name = "Мастава"
                        },
                        new
                        {
                            Id = 6,
                            Description = "С овощным миксом",
                            DishTypeId = 2,
                            Name = "Стейк из горбуши"
                        },
                        new
                        {
                            Id = 7,
                            Description = "Из курицы",
                            DishTypeId = 2,
                            Name = "Плов"
                        },
                        new
                        {
                            Id = 8,
                            Description = "Из курицы",
                            DishTypeId = 2,
                            Name = "Дамляма"
                        },
                        new
                        {
                            Id = 9,
                            Description = "Напиток дня",
                            DishTypeId = 4,
                            Name = "Чай"
                        },
                        new
                        {
                            Id = 10,
                            Description = "Из клюквы",
                            DishTypeId = 4,
                            Name = "Морс"
                        });
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.DishType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DishTypes");

                    b.HasData(
                        new
                        {
                            Id = 3,
                            Name = "Салат"
                        },
                        new
                        {
                            Id = 1,
                            Name = "Суп"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Горячие"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Напиток"
                        });
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("WeeklyMenuId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WeeklyMenuId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.Table", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Seats")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Tables");

                    b.HasData(
                        new
                        {
                            Id = 9999,
                            Description = "Стол 1 из 4 мест",
                            Seats = 4
                        });
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.TableReservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ReservationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("integer");

                    b.Property<int>("SeatsReserved")
                        .HasColumnType("integer");

                    b.Property<int>("TableId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.HasIndex("TableId");

                    b.HasIndex("UserId");

                    b.ToTable("TableReservations");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            PasswordHash = "wcIksDzZvHtqhtd/XazkAZF2bEhc1V3EjK+ayHMzXW8=",
                            Role = "Administrator",
                            Username = "Admin"
                        });
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.WeeklyMenuCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WeeklyMenuCards");

                    b.HasData(
                        new
                        {
                            Id = 9999,
                            Name = "Бизнес-ланч #9999"
                        });
                });

            modelBuilder.Entity("DishWeeklyMenuCard", b =>
                {
                    b.Property<int>("DishesId")
                        .HasColumnType("integer");

                    b.Property<int>("WeeklyMenuCardsId")
                        .HasColumnType("integer");

                    b.HasKey("DishesId", "WeeklyMenuCardsId");

                    b.HasIndex("WeeklyMenuCardsId");

                    b.ToTable("DishWeeklyMenuCard");
                });

            modelBuilder.Entity("TableReservationDish", b =>
                {
                    b.Property<int>("DishId")
                        .HasColumnType("integer");

                    b.Property<int>("TableReservationId")
                        .HasColumnType("integer");

                    b.HasKey("DishId", "TableReservationId");

                    b.HasIndex("TableReservationId");

                    b.ToTable("TableReservationDish");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.Dish", b =>
                {
                    b.HasOne("BurLunch.AuthAPI.Models.DishType", "DishType")
                        .WithMany("Dishes")
                        .HasForeignKey("DishTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DishType");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.Schedule", b =>
                {
                    b.HasOne("BurLunch.AuthAPI.Models.WeeklyMenuCard", "WeeklyMenu")
                        .WithMany()
                        .HasForeignKey("WeeklyMenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WeeklyMenu");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.TableReservation", b =>
                {
                    b.HasOne("BurLunch.AuthAPI.Models.Schedule", "Schedule")
                        .WithMany("TableReservations")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BurLunch.AuthAPI.Models.Table", "Table")
                        .WithMany()
                        .HasForeignKey("TableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BurLunch.AuthAPI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");

                    b.Navigation("Table");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DishWeeklyMenuCard", b =>
                {
                    b.HasOne("BurLunch.AuthAPI.Models.Dish", null)
                        .WithMany()
                        .HasForeignKey("DishesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BurLunch.AuthAPI.Models.WeeklyMenuCard", null)
                        .WithMany()
                        .HasForeignKey("WeeklyMenuCardsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TableReservationDish", b =>
                {
                    b.HasOne("BurLunch.AuthAPI.Models.Dish", null)
                        .WithMany()
                        .HasForeignKey("DishId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BurLunch.AuthAPI.Models.TableReservation", null)
                        .WithMany()
                        .HasForeignKey("TableReservationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.DishType", b =>
                {
                    b.Navigation("Dishes");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.Schedule", b =>
                {
                    b.Navigation("TableReservations");
                });
#pragma warning restore 612, 618
        }
    }
}
