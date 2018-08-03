using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace population.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actuals",
                columns: table => new
                {
                    State = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActualHouseholds = table.Column<int>(nullable: false),
                    ActualPopulation = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actuals", x => x.State);
                });

            migrationBuilder.CreateTable(
                name: "Estimates",
                columns: table => new
                {
                    State = table.Column<int>(nullable: false),
                    Districts = table.Column<int>(nullable: false),
                    EstimateHoseholds = table.Column<int>(nullable: false),
                    EstimatesPopulation = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estimates", x => new { x.State, x.Districts });
                    table.UniqueConstraint("AK_Estimates_Districts_State", x => new { x.Districts, x.State });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actuals");

            migrationBuilder.DropTable(
                name: "Estimates");
        }
    }
}
