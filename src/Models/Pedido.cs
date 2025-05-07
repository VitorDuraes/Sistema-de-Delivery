using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Delivery.src.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public string Status { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public int? EntregadorId { get; set; }
        public Entregador Entregador { get; set; }
        public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();




    }
}