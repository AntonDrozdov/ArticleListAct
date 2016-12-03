using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BizMall.Migrations
{
    public partial class ManyToManyreferenceGoodImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Goods_GoodId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_GoodId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "GoodId",
                table: "Images");

            migrationBuilder.CreateTable(
                name: "RelGoodImage",
                columns: table => new
                {
                    ImageId = table.Column<int>(nullable: false),
                    GoodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelGoodImage", x => new { x.ImageId, x.GoodId });
                    table.ForeignKey(
                        name: "FK_RelGoodImage_Goods_GoodId",
                        column: x => x.GoodId,
                        principalTable: "Goods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelGoodImage_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelGoodImage_GoodId",
                table: "RelGoodImage",
                column: "GoodId");

            migrationBuilder.CreateIndex(
                name: "IX_RelGoodImage_ImageId",
                table: "RelGoodImage",
                column: "ImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelGoodImage");

            migrationBuilder.AddColumn<int>(
                name: "GoodId",
                table: "Images",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_GoodId",
                table: "Images",
                column: "GoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Goods_GoodId",
                table: "Images",
                column: "GoodId",
                principalTable: "Goods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
