using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionVentes.Migrations
{
    /// <inheritdoc />
    public partial class MigrationVentes1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LineltemTotal",
                table: "LigneOrders",
                newName: "LineItemTotal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LineItemTotal",
                table: "LigneOrders",
                newName: "LineltemTotal");
        }
    }
}
