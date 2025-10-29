using GestaoEstoque.Models;
using System.ComponentModel.DataAnnotations;

namespace GestaoEstoque.DTOs
{
    public class MovimentacaoDto
    {
        [Required]
        public string ProdutoSKU { get; set; } = string.Empty;
        [Required]
        public int Quantidade { get; set; }

        // Opcional para SAIDA, Obrigatório para ENTRADA de PERECIVEL
        public string? Lote { get; set; }
        public DateTime? DataValidade { get; set; }
    }
}