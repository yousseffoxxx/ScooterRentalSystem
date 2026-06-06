using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScooterRental.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewAndAlarmFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdRejectionReason",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParkingPhotoStatus",
                table: "Rides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ParkingRejectionReason",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdRejectionReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ParkingPhotoStatus",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "ParkingRejectionReason",
                table: "Rides");
        }
    }
}
