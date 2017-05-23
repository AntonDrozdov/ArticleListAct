using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BizMall.Migrations
{
    public partial class KwCatMetaTagsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "metaDescription",
                table: "KWs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metaKeyWords",
                table: "KWs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metaDescription",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metaKeyWords",
                table: "Categories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metaDescription",
                table: "KWs");

            migrationBuilder.DropColumn(
                name: "metaKeyWords",
                table: "KWs");

            migrationBuilder.DropColumn(
                name: "metaDescription",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "metaKeyWords",
                table: "Categories");
        }
    }
}
