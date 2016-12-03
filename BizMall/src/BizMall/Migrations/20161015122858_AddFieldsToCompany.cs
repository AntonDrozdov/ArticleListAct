using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using BizMall.Models.CompanyModels;

namespace BizMall.Migrations
{
    public partial class AddFieldsToCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "Companies",
                nullable: false,
                defaultValue: AccountType.Company);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Companies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Companies");
        }
    }
}
