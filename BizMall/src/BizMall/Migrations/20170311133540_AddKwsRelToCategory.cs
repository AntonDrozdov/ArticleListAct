using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BizMall.Migrations
{
    public partial class AddKwsRelToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_KWs_CategoryId",
                table: "KWs",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_KWs_Categories_CategoryId",
                table: "KWs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KWs_Categories_CategoryId",
                table: "KWs");

            migrationBuilder.DropIndex(
                name: "IX_KWs_CategoryId",
                table: "KWs");
        }
    }
}
