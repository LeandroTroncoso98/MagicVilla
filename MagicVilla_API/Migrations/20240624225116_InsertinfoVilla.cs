using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InsertinfoVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villa",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenUrl", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Detalle de la villa..", new DateTime(2024, 6, 24, 19, 51, 16, 486, DateTimeKind.Local).AddTicks(489), new DateTime(2024, 6, 24, 19, 51, 16, 486, DateTimeKind.Local).AddTicks(475), "", 50, "Villa Real", 5, 200.0 },
                    { 2, "", "Detalle de la villa..", new DateTime(2024, 6, 24, 19, 51, 16, 486, DateTimeKind.Local).AddTicks(493), new DateTime(2024, 6, 24, 19, 51, 16, 486, DateTimeKind.Local).AddTicks(492), "", 40, "Premium vista a la piscina", 4, 150.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villa",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villa",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
