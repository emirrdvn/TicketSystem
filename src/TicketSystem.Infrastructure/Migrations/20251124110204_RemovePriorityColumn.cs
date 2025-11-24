using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePriorityColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tickets");

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 11, 2, 4, 12, DateTimeKind.Utc).AddTicks(2264));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 11, 2, 4, 12, DateTimeKind.Utc).AddTicks(2266));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 11, 2, 4, 12, DateTimeKind.Utc).AddTicks(2267));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 11, 2, 4, 12, DateTimeKind.Utc).AddTicks(2269));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 11, 2, 4, 12, DateTimeKind.Utc).AddTicks(2270));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 11, 2, 4, 12, DateTimeKind.Utc).AddTicks(2271));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 11, 2, 4, 12, DateTimeKind.Utc).AddTicks(2273));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 11, 2, 4, 12, DateTimeKind.Utc).AddTicks(2274));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 11, 43, 2, 91, DateTimeKind.Utc).AddTicks(4177));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 11, 43, 2, 91, DateTimeKind.Utc).AddTicks(4179));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 11, 43, 2, 91, DateTimeKind.Utc).AddTicks(4181));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 11, 43, 2, 91, DateTimeKind.Utc).AddTicks(4182));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 11, 43, 2, 91, DateTimeKind.Utc).AddTicks(4183));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 11, 43, 2, 91, DateTimeKind.Utc).AddTicks(4185));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 11, 43, 2, 91, DateTimeKind.Utc).AddTicks(4186));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 18, 11, 43, 2, 91, DateTimeKind.Utc).AddTicks(4188));
        }
    }
}
