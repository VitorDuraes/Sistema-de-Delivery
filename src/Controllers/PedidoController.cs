using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_de_Delivery.src.Data;
using Sistema_de_Delivery.src.DTOs;
using Sistema_de_Delivery.src.Models;
using Sistema_de_Delivery.src.Services;

namespace Sistema_de_Delivery.src.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de pedidos no sistema de delivery.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {

        private readonly DeliveryContext _context;
        private readonly PedidoPdfService _pdfService;
        public PedidoController(DeliveryContext context, PedidoPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }
        /// <summary>
        /// Retorna todos os pedidos com informações de cliente e itens.
        /// </summary>
        [HttpGet]
        public IActionResult GetPedidos()
        {
            var pedidos = _context.Pedidos.Include(p => p.Cliente)
                                           .Include(p => p.Itens)
                                           .ThenInclude(ip => ip.Produto)
                                           .ToList();
            return Ok(pedidos);
        }
        /// <summary>
        /// Retorna os pedidos de um cliente específico.
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        [HttpGet("por-cliente/{clienteId}")]
        public IActionResult GetPedidosPorCliente(int clienteId)
        {
            var pedidos = _context.Pedidos
                .Where(p => p.ClienteId == clienteId)
                .ToList();

            return Ok(pedidos);
        }
        /// <summary>
        /// Retorna os pedidos atribuídos a um entregador específico.
        /// </summary>
        /// <param name="entregadorId">ID do entregador</param>

        [HttpGet("por-entregador/{entregadorId}")]
        public IActionResult GetPedidosPorEntregador(int entregadorId)
        {
            var pedidos = _context.Pedidos
                .Where(p => p.EntregadorId == entregadorId)
                .ToList();

            return Ok(pedidos);
        }

        /// <summary>
        /// Retorna pedidos filtrados por status.
        /// </summary>
        /// <param name="status">Status do pedido</param>
        [HttpGet("por-status/{status}")]
        public IActionResult GetPedidosPorStatus(string status)
        {
            var pedidos = _context.Pedidos
                .Where(p => p.Status.ToLower() == status.ToLower())
                .ToList();

            return Ok(pedidos);
        }

        /// <summary>
        /// Retorna um resumo da quantidade de pedidos por status.
        /// </summary>
        [HttpGet("dashboard/resumo-status")]
        public IActionResult GetResumoStatus()
        {
            var resumo = _context.Pedidos
                .GroupBy(p => p.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Quantidade = g.Count()
                })
                .ToDictionary(g => g.Status, g => g.Quantidade);

            return Ok(resumo);
        }

        /// <summary>
        /// Exporta os pedidos em formato CSV.
        /// </summary>
        [HttpGet("exportar/csv")]
        public IActionResult ExportarPedidosCsv()
        {
            var pedidos = _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Itens)
                .ToList();

            var linhas = new List<string>
    {
        "Id,DataPedido,Status,Cliente,QuantidadeItens"
    };

            foreach (var pedido in pedidos)
            {
                var linha = $"{pedido.Id},{pedido.DataPedido:yyyy-MM-dd},{pedido.Status},{pedido.Cliente.Nome},{pedido.Itens.Count}";
                linhas.Add(linha);
            }

            var csv = string.Join("\n", linhas);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

            return File(bytes, "text/csv", "relatorio_pedidos.csv");
        }

        /// <summary>
        /// Exporta os pedidos em formato PDF.
        /// </summary>
        [HttpGet("exportar/pdf")]
        public IActionResult ExportarPedidosPdf()
        {
            var pdfBytes = _pdfService.GerarRelatorio();
            return File(pdfBytes, "application/pdf", "relatorio_pedidos.pdf");
        }

        /// <summary>
        /// Cria um novo pedido com os itens informados.
        /// </summary>
        /// <param name="pedidoDTO">Dados do pedido</param>
        [HttpPost]
        public IActionResult CreatePedido([FromBody] PedidoDTO pedidoDTO)
        {
            var cliente = _context.Clientes.Find(pedidoDTO.ClienteId);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }

            var pedido = new Pedido
            {
                DataPedido = DateTime.Now,
                Status = "Pendente",
                ClienteId = pedidoDTO.ClienteId,
                Itens = new List<ItemPedido>()
            };

            foreach (var item in pedidoDTO.Itens)
            {
                var produto = _context.Produtos.Find(item.ProdutoId);
                if (produto == null)
                {
                    return BadRequest($"Produto com ID {item.ProdutoId} não encontrado.");
                }

                pedido.Itens.Add(new ItemPedido
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                });
            }

            _context.Pedidos.Add(pedido);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetPedidos), new { id = pedido.Id }, pedido);
        }
        /// <summary>
        /// Atribui um entregador a um pedido e atualiza o status.
        /// </summary>
        /// <param name="pedidoId">ID do pedido</param>
        /// <param name="entregadorId">ID do entregador</param>
        [HttpPut("{pedidoId}/atribuir-entregador/{entregadorId}")]
        public IActionResult AtribuirEntregador(int pedidoId, int entregadorId)
        {
            var pedido = _context.Pedidos.FirstOrDefault(p => p.Id == pedidoId);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            var entregador = _context.Entregadores.FirstOrDefault(e => e.Id == entregadorId);
            if (entregador == null)
            {
                return NotFound("Entregador não encontrado.");
            }
            if (entregador.Status != "Disponível")
            {
                return BadRequest("Entregador não está disponível.");
            }

            pedido.EntregadorId = entregadorId;
            pedido.Status = "Saiu para Entrega";
            entregador.Status = "Indisponível";
            _context.SaveChanges();
            return Ok("Entregador atribuído com sucesso.");
        }

        /// <summary>
        /// Atualiza o status de um pedido.
        /// </summary>
        /// <param name="pedidoId">ID do pedido</param>
        /// <param name="novoStatus">Novo status desejado</param>
        [HttpPut("{id}/Atualizar-status")]
        public IActionResult AtualizarStatus(int pedidoId, [FromBody] string novoStatus)
        {
            var pedido = _context.Pedidos.FirstOrDefault(p => p.Id == pedidoId);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }
            var statusPermitidos = new List<string>
            {
                "Recebido",
                "Em preparo",
                "Saiu para entrega",
                "Entregue",
                "Cancelado"
                 };

            if (!statusPermitidos.Contains(novoStatus))
            {
                return BadRequest("Status inválido.");
            }
            pedido.Status = novoStatus;
            _context.SaveChanges();
            return Ok($"Status do pedido atualizado para: {novoStatus}");
        }


    }
}