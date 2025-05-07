using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Delivery.src.Models
{
    public class Pagamento
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        public string Metodo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
        public string Status { get; set; }

        public string? CodigoTransacao { get; set; } // Para registrar ID da transação externa
        public string? Detalhes { get; set; } // JSON, mensagem de retorno, etc.
    }
}