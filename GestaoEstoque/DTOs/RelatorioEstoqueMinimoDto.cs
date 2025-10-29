namespace GestaoEstoque.DTOs
{
    public class RelatorioEstoqueMinimoDto
    {
        public string CodigoSKU { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int QuantidadeAtual { get; set; }
        public int QuantidadeMinima { get; set; }
        public string Alerta => $"Estoque baixo! Atual: {QuantidadeAtual}, Mínimo: {QuantidadeMinima}";
    }
}