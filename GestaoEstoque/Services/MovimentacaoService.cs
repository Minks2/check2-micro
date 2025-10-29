using GestaoEstoque.Data;
using GestaoEstoque.DTOs;
using GestaoEstoque.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoEstoque.Services
{
    // Usaremos na Etapa 3
    public class ValidacaoNegocioException : Exception
    {
        public ValidacaoNegocioException(string message) : base(message) { }
    }


    public class MovimentacaoService
    {
        private readonly ApplicationDbContext _context;

        public MovimentacaoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MovimentacaoEstoque> RegistrarEntradaAsync(MovimentacaoDto dto)
        {
            // Validar quantidade positiva
            if (dto.Quantidade <= 0)
            {
                throw new ValidacaoNegocioException("A quantidade da movimentação deve ser positiva.");
            }

            var produto = await _context.Produtos.FindAsync(dto.ProdutoSKU);
            if (produto == null)
            {
                throw new ValidacaoNegocioException("Produto não encontrado.");
            }

            // Validar data de validade para perecíveis
            if (produto.Categoria == CategoriaProduto.PERECIVEL)
            {
                if (string.IsNullOrEmpty(dto.Lote) || dto.DataValidade == null)
                {
                    throw new ValidacaoNegocioException("Produtos perecíveis devem ter Lote e Data de Validade na entrada.");
                }
                if (dto.DataValidade.Value < DateTime.Today)
                {
                    throw new ValidacaoNegocioException("Não é permitido dar entrada em produtos já vencidos.");
                }
            }

            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoSKU = dto.ProdutoSKU,
                Tipo = TipoMovimentacao.ENTRADA,
                Quantidade = dto.Quantidade,
                DataMovimentacao = DateTime.UtcNow,
                Lote = dto.Lote,
                DataValidade = dto.DataValidade
            };

            // Atualizar saldo do produto automaticamente
            produto.QuantidadeAtual += dto.Quantidade;

            _context.Movimentacoes.Add(movimentacao);
            await _context.SaveChangesAsync();

            return movimentacao;
        }

        public async Task<MovimentacaoEstoque> RegistrarSaidaAsync(MovimentacaoDto dto)
        {
            // Validar quantidade positiva
            if (dto.Quantidade <= 0)
            {
                throw new ValidacaoNegocioException("A quantidade da movimentação deve ser positiva.");
            }

            var produto = await _context.Produtos.FindAsync(dto.ProdutoSKU);
            if (produto == null)
            {
                throw new ValidacaoNegocioException("Produto não encontrado.");
            }

            // Verificar estoque suficiente para saídas
            if (produto.QuantidadeAtual < dto.Quantidade)
            {
                throw new ValidacaoNegocioException($"Estoque insuficiente. Disponível: {produto.QuantidadeAtual}, Tentativa de saída: {dto.Quantidade}");
            }

            // Regra da Etapa 6: Não permitir saída de perecível se houver lote vencido (simplificado)
            // Esta é uma interpretação: se algum lote de entrada está vencido, trava a saída do produto.
            if (produto.Categoria == CategoriaProduto.PERECIVEL)
            {
                var temLoteVencido = await _context.Movimentacoes
                    .Where(m => m.ProdutoSKU == dto.ProdutoSKU &&
                                m.Tipo == TipoMovimentacao.ENTRADA &&
                                m.DataValidade.HasValue &&
                                m.DataValidade.Value < DateTime.Today)
                    .AnyAsync();

                // Nota: Esta regra não é perfeita (não sabe se o lote vencido já saiu),
                // mas atende à validação solicitada "não pode ter movimentação após data de validade".
                if (temLoteVencido)
                {
                    throw new ValidacaoNegocioException("Não é possível realizar a saída: existem lotes vencidos deste produto em estoque.");
                }
            }

            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoSKU = dto.ProdutoSKU,
                Tipo = TipoMovimentacao.SAIDA,
                Quantidade = dto.Quantidade,
                DataMovimentacao = DateTime.UtcNow,
                Lote = dto.Lote, // Opcional na saída, pode ser usado para rastreio
                DataValidade = dto.DataValidade
            };

            // Atualizar saldo do produto automaticamente
            produto.QuantidadeAtual -= dto.Quantidade;

            _context.Movimentacoes.Add(movimentacao);
            await _context.SaveChangesAsync();

            return movimentacao;
        }
    }
}