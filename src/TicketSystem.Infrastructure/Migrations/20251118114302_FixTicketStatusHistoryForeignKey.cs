using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTicketStatusHistoryForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketStatusHistories_Users_ChangedByUserUserId",
                table: "TicketStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_TicketStatusHistories_ChangedByUserUserId",
                table: "TicketStatusHistories");

            migrationBuilder.DropColumn(
                name: "ChangedByUserUserId",
                table: "TicketStatusHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "TicketStatusHistories",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistories_ChangedBy",
                table: "TicketStatusHistories",
                column: "ChangedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketStatusHistories_Users_ChangedBy",
                table: "TicketStatusHistories",
                column: "ChangedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketStatusHistories_Users_ChangedBy",
                table: "TicketStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_TicketStatusHistories_ChangedBy",
                table: "TicketStatusHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "TicketStatusHistories",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedByUserUserId",
                table: "TicketStatusHistories",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 7, 40, 32, 593, DateTimeKind.Utc).AddTicks(4786));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 7, 40, 32, 593, DateTimeKind.Utc).AddTicks(4791));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 7, 40, 32, 593, DateTimeKind.Utc).AddTicks(4792));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 7, 40, 32, 593, DateTimeKind.Utc).AddTicks(4794));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 7, 40, 32, 593, DateTimeKind.Utc).AddTicks(4795));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 7, 40, 32, 593, DateTimeKind.Utc).AddTicks(4796));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 7, 40, 32, 593, DateTimeKind.Utc).AddTicks(4797));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 17, 7, 40, 32, 593, DateTimeKind.Utc).AddTicks(4799));

            migrationBuilder.CreateIndex(
                name: "IX_TicketStatusHistories_ChangedByUserUserId",
                table: "TicketStatusHistories",
                column: "ChangedByUserUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketStatusHistories_Users_ChangedByUserUserId",
                table: "TicketStatusHistories",
                column: "ChangedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
