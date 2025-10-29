using GestaoEstoque.DTOs;
using GestaoEstoque.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentacoesController : ControllerBase
    {
        private readonly MovimentacaoService _movimentacaoService;

        public MovimentacoesController(MovimentacaoService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService;
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] MovimentacaoDto dto)
        {
            var mov = await _movimentacaoService.RegistrarEntradaAsync(dto);
            return Ok(mov);
        }

        [HttpPost("saida")]
        public async Task<IActionResult> RegistrarSaida([FromBody] MovimentacaoDto dto)
        {
            var mov = await _movimentacaoService.RegistrarSaidaAsync(dto);
            return Ok(mov);
        }
    }
}