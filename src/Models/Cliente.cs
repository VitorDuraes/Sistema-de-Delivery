using System.ComponentModel.DataAnnotations;

namespace Sistema_de_Delivery.src.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Telefone { get; set; }

        [Required]
        public string Endereco { get; set; }

        // A senha pode ser incluída para autenticação (hash e sal devem ser usados em produção)
        [Required]
        [MinLength(6)]
        public string Senha { get; set; }
    }
}
