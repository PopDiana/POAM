using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace POAM.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    IdContract = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Provider = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.IdContract);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    IdEmployee = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(maxLength: 50, nullable: false),
                    Address = table.Column<string>(maxLength: 50, nullable: true),
                    Telephone = table.Column<string>(maxLength: 50, nullable: false),
                    Employment = table.Column<string>(maxLength: 50, nullable: false),
                    Salary = table.Column<double>(nullable: false),
                    PID = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.IdEmployee);
                });

            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    IdOwner = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(maxLength: 50, nullable: false),
                    Telephone = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    isAdmin = table.Column<bool>(nullable: true),
                    PassSalt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => x.IdOwner);
                });

            migrationBuilder.CreateTable(
                name: "Apartment",
                columns: table => new
                {
                    IdApartment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Street = table.Column<string>(maxLength: 50, nullable: false),
                    Building = table.Column<string>(maxLength: 50, nullable: false),
                    FlatNo = table.Column<string>(maxLength: 50, nullable: false),
                    NoTenants = table.Column<byte>(nullable: true),
                    PreviousDebt = table.Column<double>(nullable: true),
                    CurrentDebt = table.Column<double>(nullable: true),
                    TotalDebt = table.Column<double>(nullable: true),
                    IdOwner = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartment", x => x.IdApartment);
                    table.ForeignKey(
                        name: "FK_Apartment_Owner",
                        column: x => x.IdOwner,
                        principalTable: "Owner",
                        principalColumn: "IdOwner",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receipt",
                columns: table => new
                {
                    IdReceipt = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    IdApartment = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipt", x => x.IdReceipt);
                    table.ForeignKey(
                        name: "FK_Receipt_Apartment",
                        column: x => x.IdApartment,
                        principalTable: "Apartment",
                        principalColumn: "IdApartment",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaterConsumption",
                columns: table => new
                {
                    IdWaterConsumption = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WarmWater = table.Column<int>(nullable: true),
                    ColdWater = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    IdApartment = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterConsumption", x => x.IdWaterConsumption);
                    table.ForeignKey(
                        name: "FK_WaterConsumption_Apartment",
                        column: x => x.IdApartment,
                        principalTable: "Apartment",
                        principalColumn: "IdApartment",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartment_IdOwner",
                table: "Apartment",
                column: "IdOwner");

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_IdApartment",
                table: "Receipt",
                column: "IdApartment");

            migrationBuilder.CreateIndex(
                name: "IX_WaterConsumption_IdApartment",
                table: "WaterConsumption",
                column: "IdApartment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contract");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Receipt");

            migrationBuilder.DropTable(
                name: "WaterConsumption");

            migrationBuilder.DropTable(
                name: "Apartment");

            migrationBuilder.DropTable(
                name: "Owner");
        }
    }
}
