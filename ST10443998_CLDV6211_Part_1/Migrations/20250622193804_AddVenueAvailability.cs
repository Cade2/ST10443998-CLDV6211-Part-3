using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10443998_CLDV6211_POE.Migrations
{
    /// <inheritdoc />
    public partial class AddVenueAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Venues",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Venues");
        }
    }
}
