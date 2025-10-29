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
                throw new ValidacaoNegocioException("A quantidade da movimenta��o deve ser positiva.");
            }

            var produto = await _context.Produtos.FindAsync(dto.ProdutoSKU);
            if (produto == null)
            {
                throw new ValidacaoNegocioException("Produto n�o encontrado.");
            }

            // Validar data de validade para perec�veis
            if (produto.Categoria == CategoriaProduto.PERECIVEL)
            {
                if (string.IsNullOrEmpty(dto.Lote) || dto.DataValidade == null)
                {
                    throw new ValidacaoNegocioException("Produtos perec�veis devem ter Lote e Data de Validade na entrada.");
                }
                if (dto.DataValidade.Value < DateTime.Today)
                {
                    throw new ValidacaoNegocioException("N�o � permitido dar entrada em produtos j� vencidos.");
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
                throw new ValidacaoNegocioException("A quantidade da movimenta��o deve ser positiva.");
            }

            var produto = await _context.Produtos.FindAsync(dto.ProdutoSKU);
            if (produto == null)
            {
                throw new ValidacaoNegocioException("Produto n�o encontrado.");
            }

            // Verificar estoque suficiente para sa�das
            if (produto.QuantidadeAtual < dto.Quantidade)
            {
                throw new ValidacaoNegocioException($"Estoque insuficiente. Dispon�vel: {produto.QuantidadeAtual}, Tentativa de sa�da: {dto.Quantidade}");
            }

            // Regra da Etapa 6: N�o permitir sa�da de perec�vel se houver lote vencido (simplificado)
            // Esta � uma interpreta��o: se algum lote de entrada est� vencido, trava a sa�da do produto.
            if (produto.Categoria == CategoriaProduto.PERECIVEL)
            {
                var temLoteVencido = await _context.Movimentacoes
                    .Where(m => m.ProdutoSKU == dto.ProdutoSKU &&
                                m.Tipo == TipoMovimentacao.ENTRADA &&
                                m.DataValidade.HasValue &&
                                m.DataValidade.Value < DateTime.Today)
                    .AnyAsync();

                // Nota: Esta regra n�o � perfeita (n�o sabe se o lote vencido j� saiu),
                // mas atende � valida��o solicitada "n�o pode ter movimenta��o ap�s data de validade".
                if (temLoteVencido)
                {
                    throw new ValidacaoNegocioException("N�o � poss�vel realizar a sa�da: existem lotes vencidos deste produto em estoque.");
                }
            }

            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoSKU = dto.ProdutoSKU,
                Tipo = TipoMovimentacao.SAIDA,
                Quantidade = dto.Quantidade,
                DataMovimentacao = DateTime.UtcNow,
                Lote = dto.Lote, // Opcional na sa�da, pode ser usado para rastreio
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