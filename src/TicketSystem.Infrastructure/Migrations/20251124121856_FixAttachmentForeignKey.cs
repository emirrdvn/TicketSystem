using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAttachmentForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_TicketMessages_MessageId",
                table: "TicketAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_Users_UploaderUserId",
                table: "TicketAttachments");

            migrationBuilder.DropIndex(
                name: "IX_TicketAttachments_UploaderUserId",
                table: "TicketAttachments");

            migrationBuilder.DropColumn(
                name: "UploaderUserId",
                table: "TicketAttachments");

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "TicketAttachments",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "TicketAttachments",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "TicketAttachments",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 12, 18, 56, 429, DateTimeKind.Utc).AddTicks(603));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 12, 18, 56, 429, DateTimeKind.Utc).AddTicks(606));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 12, 18, 56, 429, DateTimeKind.Utc).AddTicks(609));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 12, 18, 56, 429, DateTimeKind.Utc).AddTicks(611));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 12, 18, 56, 429, DateTimeKind.Utc).AddTicks(612));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 12, 18, 56, 429, DateTimeKind.Utc).AddTicks(614));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 12, 18, 56, 429, DateTimeKind.Utc).AddTicks(616));

            migrationBuilder.UpdateData(
                table: "TicketCategories",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 24, 12, 18, 56, 429, DateTimeKind.Utc).AddTicks(618));

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachments_UploadedBy",
                table: "TicketAttachments",
                column: "UploadedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_TicketMessages_MessageId",
                table: "TicketAttachments",
                column: "MessageId",
                principalTable: "TicketMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_Users_UploadedBy",
                table: "TicketAttachments",
                column: "UploadedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_TicketMessages_MessageId",
                table: "TicketAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_Users_UploadedBy",
                table: "TicketAttachments");

            migrationBuilder.DropIndex(
                name: "IX_TicketAttachments_UploadedBy",
                table: "TicketAttachments");

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "TicketAttachments",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "TicketAttachments",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "TicketAttachments",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "UploaderUserId",
                table: "TicketAttachments",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

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

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachments_UploaderUserId",
                table: "TicketAttachments",
                column: "UploaderUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_TicketMessages_MessageId",
                table: "TicketAttachments",
                column: "MessageId",
                principalTable: "TicketMessages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_Users_UploaderUserId",
                table: "TicketAttachments",
                column: "UploaderUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
