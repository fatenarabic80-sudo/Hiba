using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeritageMarket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryLandmarkImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LandmarkImageUrl",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LandmarkImageUrl",
                table: "Countries");
        }
    }
}
