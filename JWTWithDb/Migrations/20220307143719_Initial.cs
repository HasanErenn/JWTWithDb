using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTWithDb.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "UserDtos");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordSalt",
                table: "UserDtos",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "UserDtos",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "PasswordSalt",
                table: "UserDtos",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "PasswordHash",
                table: "UserDtos",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "UserDtos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
