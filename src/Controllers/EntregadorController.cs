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
    public class EntregadorController : ControllerBase
    {
        private readonly DeliveryContext _context;

        public EntregadorController(DeliveryContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retorna todos os entregadores cadastrados.
        /// </summary>
        [HttpGet]
        public IActionResult GetEntregadores()
        {
            var entregadores = _context.Set<Entregador>().ToList();
            return Ok(entregadores);
        }

        /// <summary>
        /// Retorna um entregador específico pelo ID.
        /// </summary>
        /// <param name="id">ID do entregador</param>
        [HttpGet("{id}")]
        public IActionResult GetEntregadorById(int id)
        {
            var entregador = _context.Set<Entregador>().Find(id);
            if (entregador == null)
            {
                return NotFound();
            }
            return Ok(entregador);
        }

        /// <summary>
        /// Cria um novo entregador.
        /// </summary>
        /// <param name="entregador">Objeto entregador a ser criado</param>
        [HttpPost]
        public IActionResult CreateEntregador([FromBody] Entregador entregador)
        {
            _context.Set<Entregador>().Add(entregador);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetEntregadorById), new { id = entregador.Id }, entregador);
        }

        /// <summary>
        /// Atualiza os dados de um entregador existente.
        /// </summary>
        /// <param name="id">ID do entregador a ser atualizado</param>
        /// <param name="entregador">Objeto com os novos dados do entregador</param>
        [HttpPut("{id}")]
        public IActionResult UpdateEntregador(int id, [FromBody] Entregador entregador)
        {
            var existingEntregador = _context.Set<Entregador>().Find(id);
            if (existingEntregador == null)
            {
                return NotFound();
            }
            existingEntregador.Nome = entregador.Nome;
            existingEntregador.Telefone = entregador.Telefone;
            existingEntregador.Status = entregador.Status;

            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Remove um entregador pelo ID.
        /// </summary>
        /// <param name="id">ID do entregador a ser removido</param>
        [HttpDelete("{id}")]
        public IActionResult DeleteEntregador(int id)
        {
            var entregador = _context.Set<Entregador>().Find(id);
            if (entregador == null)
            {
                return NotFound();
            }
            _context.Set<Entregador>().Remove(entregador);
            _context.SaveChanges();
            return NoContent();
        }

    }
}