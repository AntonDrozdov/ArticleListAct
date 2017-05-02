using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BizMall.Migrations
{
    public partial class AddMetaTagsToArticleModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "metaDescription",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "metaKeyWords",
                table: "Articles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metaDescription",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "metaKeyWords",
                table: "Articles");
        }
    }
}
