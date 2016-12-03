using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BizMall.Migrations
{
    public partial class DeleteNullReferenceConstraintFromImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Goods_GoodId",
                table: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "GoodId",
                table: "Images",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Goods_GoodId",
                table: "Images",
                column: "GoodId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Goods_GoodId",
                table: "Images");

            migrationBuilder.AlterColumn<int>(
                name: "GoodId",
                table: "Images",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Goods_GoodId",
                table: "Images",
                column: "GoodId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
