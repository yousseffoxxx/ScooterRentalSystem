using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScooterRental.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class addbackidphotourl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdPhotoUrl",
                table: "Users",
                newName: "IdFrontPhotoUrl");

            migrationBuilder.AddColumn<string>(
                name: "IdBackPhotoUrl",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdBackPhotoUrl",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IdFrontPhotoUrl",
                table: "Users",
                newName: "IdPhotoUrl");
        }
    }
}
