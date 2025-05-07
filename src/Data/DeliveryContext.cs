using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sistema_de_Delivery.src.Data
{
    public class DeliveryContext : DbContext
    {
        public DeliveryContext(DbContextOptions<DeliveryContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.ItemPedido>()
                .HasKey(ip => new { ip.PedidoId, ip.ProdutoId });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Models.Cliente> Clientes { get; set; }
        public DbSet<Models.ClienteCadastro> ClientesCadastro { get; set; }
        public DbSet<Models.Produto> Produtos { get; set; }
        public DbSet<Models.Pedido> Pedidos { get; set; }
        public DbSet<Models.ItemPedido> ItensPedido { get; set; }
        public DbSet<Models.Entregador> Entregadores { get; set; }
        public DbSet<Models.Pagamento> Pagamentos { get; set; }


    }
}