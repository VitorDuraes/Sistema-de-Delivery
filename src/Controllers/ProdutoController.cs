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
    public class ProdutoController : ControllerBase
    {
        private readonly DeliveryContext _context;
        public ProdutoController(DeliveryContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retorna todos os produtos disponíveis.
        /// </summary>
        [HttpGet]
        public IActionResult GetProdutos()
        {
            var produtos = _context.Produtos.ToList();
            return Ok(produtos);
        }

        /// <summary>
        /// Retorna um produto específico pelo ID.
        /// </summary>
        /// <param name="id">ID do produto</param>
        [HttpGet("{id}")]
        public IActionResult GetProdutoById(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        /// <param name="produto">Objeto produto a ser criado</param>
        [HttpPost]
        public IActionResult CreateProduto([FromBody] Produto produto)
        {
            _context.Produtos.Add(produto);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProdutoById), new { id = produto.Id }, produto);
        }

        /// <summary>
        /// Atualiza os dados de um produto existente.
        /// </summary>
        /// <param name="id">ID do produto a ser atualizado</param>
        /// <param name="produto">Objeto com os novos dados do produto</param>
        [HttpPut("{id}")]
        public IActionResult UpdateProduto(int id, [FromBody] Produto produto)
        {
            var existingProduto = _context.Produtos.Find(id);
            if (existingProduto == null)
            {
                return NotFound();
            }
            existingProduto.Nome = produto.Nome;
            existingProduto.Preco = produto.Preco;
            existingProduto.Descricao = produto.Descricao;

            _context.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Remove um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto a ser removido</param>
        [HttpDelete("{id}")]
        public IActionResult DeleteProduto(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                return NotFound();
            }
            _context.Produtos.Remove(produto);
            _context.SaveChanges();
            return NoContent();
        }

    }
}