using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Delivery.src.DTOs
{
    public class PedidoDTO
    {
        public int ClienteId { get; set; }
        public List<ItemPedidoDTO> Itens { get; set; }
    }
}