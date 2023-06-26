using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightBookingSystemV5.Migrations
{
    public partial class cancellationSeatNos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CancelledSeatNos",
                table: "JourneyDetails",
                newName: "ECancelledSeatNos");

            migrationBuilder.AddColumn<string>(
                name: "BCancelledSeatNos",
                table: "JourneyDetails",
                type: "VARCHAR(MAX)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BCancelledSeatNos",
                table: "JourneyDetails");

            migrationBuilder.RenameColumn(
                name: "ECancelledSeatNos",
                table: "JourneyDetails",
                newName: "CancelledSeatNos");
        }
    }
}
