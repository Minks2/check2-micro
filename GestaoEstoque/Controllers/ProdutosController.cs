using GestaoEstoque.DTOs;
using GestaoEstoque.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoService _produtoService;

        public ProdutosController(ProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarProduto([FromBody] ProdutoCreateDto dto)
        {
            var produto = await _produtoService.CriarProdutoAsync(dto);
            return CreatedAtAction(nameof(CriarProduto), new { sku = produto.CodigoSKU }, produto);
        }
    }
}