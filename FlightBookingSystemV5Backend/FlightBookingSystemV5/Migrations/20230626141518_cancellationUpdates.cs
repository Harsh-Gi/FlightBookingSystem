using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightBookingSystemV5.Migrations
{
    public partial class cancellationUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BClassCurrSeatNo",
                table: "JourneyDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancelledSeatNos",
                table: "JourneyDetails",
                type: "VARCHAR(MAX)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EClassCurrSeatNo",
                table: "JourneyDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BClassCurrSeatNo",
                table: "JourneyDetails");

            migrationBuilder.DropColumn(
                name: "CancelledSeatNos",
                table: "JourneyDetails");

            migrationBuilder.DropColumn(
                name: "EClassCurrSeatNo",
                table: "JourneyDetails");
        }
    }
}
