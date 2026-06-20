using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScooterRental.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class addDeviceSecretKeycolumntothescootertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceSecretKey",
                table: "Scooters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceSecretKey",
                table: "Scooters");
        }
    }
}
