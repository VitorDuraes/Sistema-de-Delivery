using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema_de_Delivery.src.Data;
using Sistema_de_Delivery.src.Models;

namespace Sistema_de_Delivery.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly DeliveryContext _context;
        public ClienteController(DeliveryContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retorna todos os clientes cadastrados.
        /// </summary>
        [HttpGet]
        [Authorize]
        public IActionResult GetClientes()
        {
            var clientes = _context.Clientes.ToList();
            return Ok(clientes);
        }
        /// <summary>
        /// Retorna um cliente pelo ID.
        /// </summary>
        /// <param name="id">ID do cliente</param>
        [HttpGet("{id}")]
        public IActionResult GetClienteById(int id)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return Ok(cliente);
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="cliente">Objeto cliente a ser criado</param>
        [HttpPost]
        public IActionResult CreateCliente([FromBody] Cliente cliente)
        {

            if (cliente == null)
            {
                return BadRequest("Cliente não pode ser nulo.");
            }
            if (string.IsNullOrEmpty(cliente.Nome) || string.IsNullOrEmpty(cliente.Email))
            {
                return BadRequest("Nome e Email são obrigatórios.");
            }

            if (!cliente.Email.Contains("@"))
            {
                return BadRequest("Email inválido.");
            }

            if (cliente.Telefone.Length < 10)
            {
                return BadRequest("Telefone inválido.");
            }

            if (string.IsNullOrEmpty(cliente.Endereco))
            {
                return BadRequest("Endereço é obrigatório.");
            }

            if (_context.Clientes.Any(c => c.Email == cliente.Email))
            {
                return Conflict("Já existe um cliente com este email.");
            }

            if (_context.Clientes.Any(c => c.Telefone == cliente.Telefone))
            {
                return Conflict("Já existe um cliente com este telefone.");
            }

            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetClienteById), new { id = cliente.Id }, cliente);
        }

        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado</param>
        /// <param name="cliente">Objeto cliente com os dados atualizados</param>
        [HttpPut("{id}")]
        public IActionResult UpdateCliente(int id, [FromBody] Cliente cliente)
        {
            var existingCliente = _context.Clientes.Find(id);
            if (existingCliente == null)
            {
                return NotFound();
            }
            existingCliente.Nome = cliente.Nome;
            existingCliente.Email = cliente.Email;
            existingCliente.Telefone = cliente.Telefone;
            existingCliente.Endereco = cliente.Endereco;

            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Remove um cliente pelo ID.
        /// </summary>
        /// <param name="id">ID do cliente a ser removido</param>

        [HttpDelete("{id}")]
        public IActionResult DeleteCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente == null)
            {
                return NotFound();
            }
            _context.Clientes.Remove(cliente);
            _context.SaveChanges();
            return NoContent();
        }

    }
}