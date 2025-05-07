using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_Delivery.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmado",
                table: "Pagamentos");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Pagamentos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Pagamentos");

            migrationBuilder.AddColumn<bool>(
                name: "Confirmado",
                table: "Pagamentos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
