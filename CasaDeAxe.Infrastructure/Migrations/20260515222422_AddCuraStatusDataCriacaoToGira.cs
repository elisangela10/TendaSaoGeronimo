using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CasaDeAxe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCuraStatusDataCriacaoToGira : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "StatusUsuarios",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "StatusUsuarios",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.AddColumn<string>(
                name: "Cura",
                table: "Giras",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Giras",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Giras",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Ativo", "Nome" },
                values: new object[,]
                {
                    { 1, true, "ADM" },
                    { 2, true, "PaiDeSanto" },
                    { 3, true, "Filho" },
                    { 4, true, "Assistencia" }
                });

            migrationBuilder.InsertData(
                table: "StatusUsuarios",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Ativo" },
                    { 2, "Inativo" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "StatusUsuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StatusUsuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Cura",
                table: "Giras");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Giras");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Giras");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Ativo", "Nome" },
                values: new object[,]
                {
                    { -4, true, "Assistencia" },
                    { -3, true, "Filho" },
                    { -2, true, "PaiDeSanto" },
                    { -1, true, "ADM" }
                });

            migrationBuilder.InsertData(
                table: "StatusUsuarios",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { -2, "Inativo" },
                    { -1, "Ativo" }
                });
        }
    }
}
