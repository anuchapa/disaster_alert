using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisasterAlert.context.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDataTable041226 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RiskLevel",
                table: "RiskReports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegionId",
                table: "Regions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_RegionId",
                table: "Regions",
                column: "RegionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Regions_RegionId",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "RiskLevel",
                table: "RiskReports");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Regions");
        }
    }
}
