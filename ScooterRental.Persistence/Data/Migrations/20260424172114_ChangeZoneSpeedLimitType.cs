using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScooterRental.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeZoneSpeedLimitType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "SpeedLimitKmH",
                table: "Zones",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SpeedLimitKmH",
                table: "Zones",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
