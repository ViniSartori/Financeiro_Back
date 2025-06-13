using CrudDashboard.Dto;
using CrudDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudDashboard.Services.Movimentacoes
{
    public interface IMovimentacoesInterface
    {
        Task<ResponseMovimentacoes<List<MovimentacoesDto>>> BuscarMovimentacoes();

        Task<ResponseMovimentacoes<List<MovimentacoesDto>>> BuscarMovimentacoesNumeroConta(string movimentacoesNumeroConta, int? mes = null, int? ano = null);

        Task<ResponseMovimentacoes<MovimentacoesCadastrarDto>> CriarMovimentacoes(MovimentacoesCadastrarDto movimentacoesCadastrarDto);

        Task<ResponseMovimentacoes<MovimentacoesConferidoDto>> EditarMovimentacoes(MovimentacoesConferidoDto movimentacoesConferidoDto);

        Task<ResponseMovimentacoes<List<MovimentacoesDto>>> DeletarMovimentacoes(int Id);

        Task<SaldoDto> ObterSaldos(string numeroConta, int? mes = null, int? ano = null);
    }
}
