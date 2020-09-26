using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pluz.Sample.Migrations
{
    public partial class AddDemoProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DemoProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExtraProperties = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierId = table.Column<Guid>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    ProductCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DemoProductLocalizableEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CultureName = table.Column<string>(maxLength: 32, nullable: false, defaultValue: "False"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoProductLocalizableEntries", x => new { x.Id, x.CultureName });
                    table.ForeignKey(
                        name: "FK_DemoProductLocalizableEntries_DemoProducts_Id",
                        column: x => x.Id,
                        principalTable: "DemoProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemoProductLocalizableEntries");

            migrationBuilder.DropTable(
                name: "DemoProducts");
        }
    }
}
