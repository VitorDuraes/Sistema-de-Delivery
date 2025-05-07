using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Delivery.src.Data;
using Sistema_de_Delivery.src.Models;

namespace Sistema_de_Delivery.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : ControllerBase
    {
        private readonly DeliveryContext _context;

        public PagamentoController(DeliveryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra um novo pagamento no sistema.
        /// </summary>
        /// <param name="pagamento">Objeto que contém os dados do pagamento a ser registrado.</param>
        /// <returns>Retorna o pagamento criado com um código de status 201.</returns>
        [HttpPost]
        public IActionResult RegistrarPagamento([FromBody] Pagamento pagamento)
        {
            _context.Pagamentos.Add(pagamento);
            _context.SaveChanges();
            return CreatedAtAction(nameof(ObterPagamentoPorId), new { id = pagamento.Id }, pagamento);
        }

        /// <summary>
        /// Simula a confirmação de pagamento via Pix, alterando o status do pagamento.
        /// </summary>
        /// <param name="id">ID do pagamento a ser simulado.</param>
        /// <returns>Retorna a mensagem de confirmação do pagamento via Pix.</returns>
        [HttpPost("simular-pix/{id}")]
        public IActionResult SimularPix(int id)
        {
            var pagamento = _context.Pagamentos.Find(id);
            if (pagamento == null)
                return NotFound();

            if (pagamento.Metodo.ToLower() != "pix")
                return BadRequest("Método de pagamento não é Pix");

            pagamento.Status = "Pago";
            pagamento.CodigoTransacao = Guid.NewGuid().ToString();
            pagamento.Detalhes = $"Pagamento confirmado via Pix em {DateTime.Now}";

            _context.SaveChanges();

            return Ok(new
            {
                mensagem = "Pagamento via Pix confirmado!",
                pagamento
            });
        }

        /// <summary>
        /// Lista todos os pagamentos registrados no sistema.
        /// </summary>
        /// <returns>Retorna uma lista com todos os pagamentos.</returns>
        [HttpGet]
        public IActionResult ListarPagamentos()
        {
            var pagamentos = _context.Pagamentos.ToList();
            return Ok(pagamentos);
        }

        /// <summary>
        /// Obtém os detalhes de um pagamento específico pelo ID.
        /// </summary>
        /// <param name="id">ID do pagamento a ser retornado.</param>
        /// <returns>Retorna os detalhes do pagamento encontrado.</returns>
        [HttpGet("{id}")]
        public IActionResult ObterPagamentoPorId(int id)
        {
            var pagamento = _context.Pagamentos.Find(id);
            if (pagamento == null)
                return NotFound();
            return Ok(pagamento);
        }

        /// <summary>
        /// Atualiza o status de um pagamento existente.
        /// </summary>
        /// <param name="id">ID do pagamento a ser atualizado.</param>
        /// <param name="novoStatus">Novo status a ser atribuído ao pagamento.</param>
        /// <returns>Retorna um código de status 204 se a atualização for bem-sucedida.</returns>
        [HttpPut("{id}")]
        public IActionResult AtualizarStatusPagamento(int id, [FromBody] string novoStatus)
        {
            var pagamento = _context.Pagamentos.Find(id);
            if (pagamento == null)
                return NotFound();

            pagamento.Status = novoStatus;
            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Exclui um pagamento existente do sistema.
        /// </summary>
        /// <param name="id">ID do pagamento a ser excluído.</param>
        /// <returns>Retorna um código de status 204 se a exclusão for bem-sucedida.</returns>
        [HttpDelete("{id}")]
        public IActionResult ExcluirPagamento(int id)
        {
            var pagamento = _context.Pagamentos.Find(id);
            if (pagamento == null)
                return NotFound();

            _context.Pagamentos.Remove(pagamento);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
