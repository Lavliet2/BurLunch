﻿// <auto-generated />
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
    [Migration("20241215145200_InitialCreate")]
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

            modelBuilder.Entity("BurLunch.AuthAPI.Models.DailyMenu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("integer");

                    b.Property<int>("WeeklyMenuCardId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("WeeklyMenuCardId");

                    b.ToTable("DailyMenus");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.Dish", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("DailyMenuId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DishTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DailyMenuId");

                    b.HasIndex("DishTypeId");

                    b.ToTable("Dishes");
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

                    b.Property<string>("WeekNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("WeeklyMenuCards");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.DailyMenu", b =>
                {
                    b.HasOne("BurLunch.AuthAPI.Models.WeeklyMenuCard", "WeeklyMenuCard")
                        .WithMany("DailyMenus")
                        .HasForeignKey("WeeklyMenuCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WeeklyMenuCard");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.Dish", b =>
                {
                    b.HasOne("BurLunch.AuthAPI.Models.DailyMenu", null)
                        .WithMany("Dishes")
                        .HasForeignKey("DailyMenuId");

                    b.HasOne("BurLunch.AuthAPI.Models.DishType", "DishType")
                        .WithMany("Dishes")
                        .HasForeignKey("DishTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DishType");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.DailyMenu", b =>
                {
                    b.Navigation("Dishes");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.DishType", b =>
                {
                    b.Navigation("Dishes");
                });

            modelBuilder.Entity("BurLunch.AuthAPI.Models.WeeklyMenuCard", b =>
                {
                    b.Navigation("DailyMenus");
                });
#pragma warning restore 612, 618
        }
    }
}
