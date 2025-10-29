using GestaoEstoque.Data;
using GestaoEstoque.DTOs;
using GestaoEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoEstoque.Services
{
    public class RelatorioService
    {
        private readonly ApplicationDbContext _context;
        private readonly ProdutoService _produtoService;

        public RelatorioService(ApplicationDbContext context, ProdutoService produtoService)
        {
            _context = context;
            _produtoService = produtoService;
        }

        // Calcular valor total do estoque (quantidade � pre�o)
        public async Task<decimal> CalcularValorTotalEstoqueAsync()
        {
            return await _context.Produtos
                .SumAsync(p => p.QuantidadeAtual * p.PrecoUnitario);
        }

        // Listar produtos que vencer�o em at� 7 dias
        public async Task<IEnumerable<Produto>> ListarProdutosProximosVencimentoAsync(int dias = 7)
        {
            var dataLimite = DateTime.Today.AddDays(dias);

            // Pega SKUs de movimenta��es de ENTRADA que vencem no per�odo
            var skus = await _context.Movimentacoes
                .Where(m => m.Produto.Categoria == CategoriaProduto.PERECIVEL &&
                            m.Tipo == TipoMovimentacao.ENTRADA &&
                            m.DataValidade.HasValue &&
                            m.DataValidade.Value >= DateTime.Today &&
                            m.DataValidade.Value <= dataLimite)
                .Select(m => m.ProdutoSKU)
                .Distinct()
                .ToListAsync();

            // Retorna os produtos completos
            return await _context.Produtos
                .Where(p => skus.Contains(p.CodigoSKU))
                .ToListAsync();
        }

        // Identificar produtos com estoque abaixo do m�nimo
        public async Task<IEnumerable<RelatorioEstoqueMinimoDto>> ListarEstoqueAbaixoMinimoAsync()
        {
            // Reutiliza o m�todo do ProdutoService
            return await _produtoService.VerificarEstoqueMinimoAsync();
        }
    }
}