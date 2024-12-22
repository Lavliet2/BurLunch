using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BurLunch.AuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DishTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Seats = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyMenuCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyMenuCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WeeklyMenuId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_WeeklyMenuCards_WeeklyMenuId",
                        column: x => x.WeeklyMenuId,
                        principalTable: "WeeklyMenuCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScheduleId = table.Column<int>(type: "integer", nullable: false),
                    TableId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SeatsReserved = table.Column<int>(type: "integer", nullable: false),
                    ReservationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableReservations_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TableReservations_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TableReservations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DishTypeId = table.Column<int>(type: "integer", nullable: false),
                    TableReservationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dishes_DishTypes_DishTypeId",
                        column: x => x.DishTypeId,
                        principalTable: "DishTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dishes_TableReservations_TableReservationId",
                        column: x => x.TableReservationId,
                        principalTable: "TableReservations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DishWeeklyMenuCard",
                columns: table => new
                {
                    DishesId = table.Column<int>(type: "integer", nullable: false),
                    WeeklyMenuCardsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishWeeklyMenuCard", x => new { x.DishesId, x.WeeklyMenuCardsId });
                    table.ForeignKey(
                        name: "FK_DishWeeklyMenuCard_Dishes_DishesId",
                        column: x => x.DishesId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishWeeklyMenuCard_WeeklyMenuCards_WeeklyMenuCardsId",
                        column: x => x.WeeklyMenuCardsId,
                        principalTable: "WeeklyMenuCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DishTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Суп" },
                    { 2, "Горячие" },
                    { 3, "Салат" },
                    { 4, "Напиток" }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Description", "Seats" },
                values: new object[] { 9999, "Стол 1 из 4 мест", 4 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, "wcIksDzZvHtqhtd/XazkAZF2bEhc1V3EjK+ayHMzXW8=", "Administrator", "Admin" });

            migrationBuilder.InsertData(
                table: "WeeklyMenuCards",
                columns: new[] { "Id", "Name" },
                values: new object[] { 9999, "Бизнес-ланч #9999" });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Description", "DishTypeId", "Name", "TableReservationId" },
                values: new object[,]
                {
                    { 1, "Из свежих овощей", 3, "Ачичук", null },
                    { 2, "", 3, "Оливье", null },
                    { 3, "", 3, "Деревенский", null },
                    { 4, "По домашнему с фрикадельками и лапшой", 1, "Чучвара", null },
                    { 5, "Грузинский суп с куриным филе", 1, "Мастава", null },
                    { 6, "С овощным миксом", 2, "Стейк из горбуши", null },
                    { 7, "Из курицы", 2, "Плов", null },
                    { 8, "Из курицы", 2, "Дамляма", null },
                    { 9, "Напиток дня", 4, "Чай", null },
                    { 10, "Из клюквы", 4, "Морс", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_DishTypeId",
                table: "Dishes",
                column: "DishTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_TableReservationId",
                table: "Dishes",
                column: "TableReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_DishWeeklyMenuCard_WeeklyMenuCardsId",
                table: "DishWeeklyMenuCard",
                column: "WeeklyMenuCardsId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_WeeklyMenuId",
                table: "Schedules",
                column: "WeeklyMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_TableReservations_ScheduleId",
                table: "TableReservations",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TableReservations_TableId",
                table: "TableReservations",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_TableReservations_UserId",
                table: "TableReservations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishWeeklyMenuCard");

            migrationBuilder.DropTable(
                name: "Dishes");

            migrationBuilder.DropTable(
                name: "DishTypes");

            migrationBuilder.DropTable(
                name: "TableReservations");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WeeklyMenuCards");
        }
    }
}
