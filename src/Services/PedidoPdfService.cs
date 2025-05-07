using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Sistema_de_Delivery.src.Data;

namespace Sistema_de_Delivery.src.Services
{
    public class PedidoPdfService
    {
        private readonly DeliveryContext _context;

        public PedidoPdfService(DeliveryContext context)
        {
            _context = context;
        }

        public byte[] GerarRelatorio()
        {
            var pedidos = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                .ToList();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Relatório de Pedidos").FontSize(20).Bold().AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // ID
                            columns.RelativeColumn(2);  // Data
                            columns.RelativeColumn(2);  // Status
                            columns.RelativeColumn(3);  // Cliente
                            columns.ConstantColumn(50); // Itens
                        });

                        // Cabeçalho
                        table.Header(header =>
                        {
                            header.Cell().Text("ID").Bold();
                            header.Cell().Text("Data").Bold();
                            header.Cell().Text("Status").Bold();
                            header.Cell().Text("Cliente").Bold();
                            header.Cell().Text("Itens").Bold();
                        });

                        // Linhas
                        foreach (var pedido in pedidos)
                        {
                            table.Cell().Text(pedido.Id.ToString());
                            table.Cell().Text(pedido.DataPedido.ToShortDateString());
                            table.Cell().Text(pedido.Status);
                            table.Cell().Text(pedido.Cliente.Nome);
                            table.Cell().Text(pedido.Itens.Count.ToString());
                        }
                    });
                });
            });

            return pdf.GeneratePdf();
        }
    }
}