using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrudDashboard.Models;
using Microsoft.AspNetCore.Identity;
using Dapper;
using CrudDashboard.Dto;
using CrudDashboard.Services.Movimentacoes;
using System.Runtime.InteropServices;

namespace CrudDashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MovimentacoesController : ControllerBase
    {
        private readonly IMovimentacoesInterface _movimentacaoService;

        public MovimentacoesController(IMovimentacoesInterface movimentacaoService)
        {
            _movimentacaoService = movimentacaoService;
        }

        [HttpGet]

        public async Task<IActionResult> BuscarMovimentacoes()
        {
            var movimentacoes = await _movimentacaoService.BuscarMovimentacoes();
            if (movimentacoes.Status == false)
            {
                return NotFound(movimentacoes);
            }

            return Ok(movimentacoes);
        }

        [HttpGet("MovimentacaoId")]
        public async Task<IActionResult> BuscarMovimentacoesNumeroConta(string movimentacoesNumeroConta, int? mes = null, int? ano = null)
        {
            var movimentacoes = await _movimentacaoService.BuscarMovimentacoesNumeroConta(movimentacoesNumeroConta);

            if (movimentacoes.Status == false)
            {
                return NotFound(movimentacoes);
            }

            // Filtra os dados por mês e ano se os valores forem informados
            if (mes.HasValue && ano.HasValue)
            {
                movimentacoes.Dados = movimentacoes.Dados
                    .Where(m => m.Data.Month == mes.Value && m.Data.Year == ano.Value)
                    .ToList();
            }

            return Ok(movimentacoes);
        }


        [HttpPost("cadastrar")]

        public async Task<IActionResult> CriarMovimentacoes([FromBody] MovimentacoesCadastrarDto movimentacoesCadastrarDto)
        {

            var movimentacoes = await _movimentacaoService.CriarMovimentacoes(movimentacoesCadastrarDto);



            if (movimentacoes.Status == false)
            {
                return BadRequest(movimentacoes);
            }

            return Ok(movimentacoes);
        }

        [HttpPut("editar")]

        public async Task<IActionResult> EditarMovimentacoes(MovimentacoesConferidoDto movimentacoesConferidoDto)
        {
            var movimentacoes = await _movimentacaoService.EditarMovimentacoes(movimentacoesConferidoDto);
            if (movimentacoes.Status == false)
            {
                return BadRequest(movimentacoes);
            }

            return Ok(movimentacoes);
        }

        [HttpDelete("remover")]
        public async Task<IActionResult> DeletarMovimentacoes(int Id)
        {
            var response = await _movimentacaoService.DeletarMovimentacoes(Id);

            if (!response.Status)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("saldos")]
        public async Task<IActionResult> ObterSaldos([FromQuery] string numeroConta, [FromQuery] int? ano = null, [FromQuery] int? mes = null)
        {
            var saldos = await _movimentacaoService.ObterSaldos(numeroConta, mes, ano);
            return Ok(saldos);
        }

    }
}