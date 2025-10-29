using GestaoEstoque.Models;
using System.ComponentModel.DataAnnotations;

namespace GestaoEstoque.DTOs
{
	public class ProdutoCreateDto
	{
		[Required]
		public string CodigoSKU { get; set; } = string.Empty;
		[Required]
		public string Nome { get; set; } = string.Empty;
		[Required]
		public CategoriaProduto Categoria { get; set; }
		[Required]
		public decimal PrecoUnitario { get; set; }
		[Required]
		public int QuantidadeMinimaEmEstoque { get; set; }
	}
}