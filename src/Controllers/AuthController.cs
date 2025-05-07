using Microsoft.AspNetCore.Mvc;
using Sistema_de_Delivery.src.Data;
using Sistema_de_Delivery.src.Models;
using Sistema_de_Delivery.src.Services;
using System.Linq;
using BCrypt.Net;
using Sistema_de_Delivery.src.DTOs;

namespace Sistema_de_Delivery.src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly DeliveryContext _context;

        // Injeção de dependência
        public AuthController(JwtService jwtService, DeliveryContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        /// <summary>
        /// Realiza o cadastro de um novo cliente.
        /// </summary>
        /// <param name="request">Objeto contendo as informações do cliente a ser cadastrado.</param>
        /// <returns>Retorna uma mensagem de sucesso ou erro ao realizar o cadastro.</returns>
        [HttpPost("Cadastro")]
        public IActionResult Cadastro([FromBody] ClienteCadastro request)
        {
            if (request == null)
                return BadRequest("Dados inválidos");

            // Verifica se o e-mail já existe
            if (_context.Clientes.Any(c => c.Email == request.Email))
                return BadRequest("O e-mail já está em uso");

            // Criação do cliente
            var cliente = new Cliente
            {
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                Endereco = request.Endereco,
                Senha = BCrypt.Net.BCrypt.HashPassword(request.Senha)
                // A senha pode ser armazenada de forma segura usando hash em vez de plain-text
            };

            // Salvar o cliente no banco de dados
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            return Ok(new { message = "Cadastro realizado com sucesso!" });
        }

        /// <summary>
        /// Realiza o login de um cliente e gera o token JWT.
        /// </summary>
        /// <param name="login">Objeto contendo o e-mail e senha do cliente para autenticação.</param>
        /// <returns>Retorna o token JWT gerado se as credenciais forem válidas.</returns>
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            if (login == null)
                return BadRequest("Credenciais inválidas");

            // Buscar o cliente pelo e-mail
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == login.Email);

            if (cliente == null)
                return Unauthorized("E-mail ou senha inválidos");

            // Verificar se a senha fornecida corresponde ao hash armazenado no banco
            if (!BCrypt.Net.BCrypt.Verify(login.Senha, cliente.Senha))
                return Unauthorized("E-mail ou senha inválidos");

            // Gerar o token JWT
            var token = _jwtService.GenerateJwtToken(cliente);

            return Ok(new { token });
        }
    }
}
