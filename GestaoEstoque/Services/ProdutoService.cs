using GestaoEstoque.Data;
using GestaoEstoque.DTOs;
using GestaoEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoEstoque.Services
{
    public class ProdutoService
    {
        private readonly ApplicationDbContext _context;

        public ProdutoService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Cadastro completo de produtos (validação de categoria está na movimentação)
        public async Task<Produto> CriarProdutoAsync(ProdutoCreateDto dto)
        {
            var produto = new Produto
            {
                CodigoSKU = dto.CodigoSKU,
                Nome = dto.Nome,
                Categoria = dto.Categoria,
                PrecoUnitario = dto.PrecoUnitario,
                QuantidadeMinimaEmEstoque = dto.QuantidadeMinimaEmEstoque,
                QuantidadeAtual = 0, // Estoque inicial é zero
                DataCriacao = DateTime.UtcNow
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        // Método para verificar produtos abaixo do estoque mínimo (usado no RelatorioService)
        public async Task<IEnumerable<RelatorioEstoqueMinimoDto>> VerificarEstoqueMinimoAsync()
        {
            return await _context.Produtos
                .Where(p => p.QuantidadeAtual < p.QuantidadeMinimaEmEstoque)
                .Select(p => new RelatorioEstoqueMinimoDto
                {
                    CodigoSKU = p.CodigoSKU,
                    Nome = p.Nome,
                    QuantidadeAtual = p.QuantidadeAtual,
                    QuantidadeMinima = p.QuantidadeMinimaEmEstoque
                })
                .ToListAsync();
        }
    }
}