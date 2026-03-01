using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceManagerAPI.Migrations
{
    /// <inheritdoc />
    public partial class TokenFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenSessions_AspNetUsers_UserId1",
                table: "TokenSessions");

            migrationBuilder.DropIndex(
                name: "IX_TokenSessions_UserId1",
                table: "TokenSessions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "TokenSessions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TokenSessions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_TokenSessions_UserId",
                table: "TokenSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenSessions_AspNetUsers_UserId",
                table: "TokenSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenSessions_AspNetUsers_UserId",
                table: "TokenSessions");

            migrationBuilder.DropIndex(
                name: "IX_TokenSessions_UserId",
                table: "TokenSessions");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "TokenSessions",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "TokenSessions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokenSessions_UserId1",
                table: "TokenSessions",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenSessions_AspNetUsers_UserId1",
                table: "TokenSessions",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
