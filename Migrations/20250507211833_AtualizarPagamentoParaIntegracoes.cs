using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_de_Delivery.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarPagamentoParaIntegracoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoTransacao",
                table: "Pagamentos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Detalhes",
                table: "Pagamentos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoTransacao",
                table: "Pagamentos");

            migrationBuilder.DropColumn(
                name: "Detalhes",
                table: "Pagamentos");
        }
    }
}
