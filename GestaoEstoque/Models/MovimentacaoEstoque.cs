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

        // Campos para perec�veis
        [StringLength(100)]
        public string? Lote { get; set; } // Nul�vel
        public DateTime? DataValidade { get; set; } // Nul�vel

        // Chave estrangeira para Produto
        [Required]
        public string ProdutoSKU { get; set; } = string.Empty;

        [ForeignKey("ProdutoSKU")]
        public virtual Produto? Produto { get; set; }
    }
}