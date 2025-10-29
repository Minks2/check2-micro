using GestaoEstoque.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : ControllerBase
    {
        private readonly RelatorioService _relatorioService;

        public RelatoriosController(RelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        [HttpGet("valor-total")]
        public async Task<IActionResult> GetValorTotalEstoque()
        {
            var valor = await _relatorioService.CalcularValorTotalEstoqueAsync();
            return Ok(new { ValorTotalEstoque = valor });
        }

        [HttpGet("proximos-vencimento")]
        public async Task<IActionResult> GetProximosVencimento([FromQuery] int dias = 7)
        {
            var produtos = await _relatorioService.ListarProdutosProximosVencimentoAsync(dias);
            return Ok(produtos);
        }

        [HttpGet("estoque-minimo")]
        public async Task<IActionResult> GetEstoqueAbaixoMinimo()
        {
            var alertas = await _relatorioService.ListarEstoqueAbaixoMinimoAsync();
            return Ok(alertas);
        }
    }
}