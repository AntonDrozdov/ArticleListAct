using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BizMall.Migrations
{
    public partial class AddCompanyImageMToMRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelCompanyImage",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false),
                    ImageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelCompanyImage", x => new { x.CompanyId, x.ImageId });
                    table.ForeignKey(
                        name: "FK_RelCompanyImage_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelCompanyImage_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelCompanyImage_CompanyId",
                table: "RelCompanyImage",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RelCompanyImage_ImageId",
                table: "RelCompanyImage",
                column: "ImageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelCompanyImage");
        }
    }
}
