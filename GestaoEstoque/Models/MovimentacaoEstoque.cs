using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoEstoque.Models
{
    public class MovimentacaoEstoque
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TipoMovimentacao Tipo { get; set; }

        [Required]
        public int Quantidade { get; set; }

        public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;

        // Campos para perecíveis
        [StringLength(100)]
        public string? Lote { get; set; } // Nulável
        public DateTime? DataValidade { get; set; } // Nulável

        // Chave estrangeira para Produto
        [Required]
        public string ProdutoSKU { get; set; } = string.Empty;

        [ForeignKey("ProdutoSKU")]
        public virtual Produto? Produto { get; set; }
    }
}