using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoEstoque.Models
{
    public class Produto
    {
        [Key]
        [StringLength(100)]
        public string CodigoSKU { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public CategoriaProduto Categoria { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }

        [Required]
        public int QuantidadeMinimaEmEstoque { get; set; }

        // Campo crucial implícito pelas regras de negócio (Etapa 5)
        public int QuantidadeAtual { get; set; } = 0;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Propriedade de navegação
        public virtual ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
    }
}