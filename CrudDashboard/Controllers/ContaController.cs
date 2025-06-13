using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrudDashboard.Models;
using Microsoft.AspNetCore.Identity;
using Dapper;
using CrudDashboard.Dto;
using CrudDashboard.Services.Movimentacoes;
using CrudDashboard.Services.Contas;

namespace CrudDashboard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly IContaInterface _contaService;

        public ContaController(IContaInterface contaService)
        {
            _contaService = contaService;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarConta()
        {
            var conta = await _contaService.BuscarConta();
            return conta.Status ? Ok(conta) : NotFound(conta);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarContaId(int id)
        {
            var conta = await _contaService.BuscarContaId(id);
            return conta.Status ? Ok(conta) : NotFound(conta);
        }

        [HttpPost]
        public async Task<IActionResult> CriarConta(ContaDto contaDto)
        {
            var conta = await _contaService.CriarConta(contaDto);
            return conta.Status
                ? CreatedAtAction(nameof(BuscarContaId), new { id = conta.Dados.Id }, conta)
                : BadRequest(conta);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarConta(int id, ContaDto contaDto)
        {
            contaDto.Id = id;
            var conta = await _contaService.EditarConta(contaDto);
            return conta.Status ? Ok(conta) : BadRequest(conta);
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeletarConta(int id)
        //{
        //    var response = await _contaService.DeletarConta(id);
        //    return response.Status ? Ok(response) : BadRequest(response);
        //}

        //[HttpPatch("alterar-status/{id}")]
        //public async Task<IActionResult> AlterarStatusConta(int id, bool statusAtivo)
        //{
        //    var response = await _contaService.AlterarStatusConta(id, statusAtivo);
        //    if (!response.Status)
        //    {
        //        return BadRequest(response);
        //    }

        //    return Ok(response);
        //}
    }
}