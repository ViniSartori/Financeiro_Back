using CrudDashboard.Dto;
using CrudDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudDashboard.Services.Contas
{
    public interface IContaInterface
    {
        Task<ResponseContas<List<ContaDto>>> BuscarConta();

        Task<ResponseContas<ContaDto>> BuscarContaId(int numeroConta);

        Task<ResponseContas<ContaDto>> CriarConta(ContaDto contaDto);

        Task<ResponseContas<ContaDto>> EditarConta(ContaDto contaDto);

        //Task<ResponseContas<List<ContaDto>>> DeletarConta(int numeroConta);

        //Task<ResponseContas<ContaDto>> AlterarStatusConta(int id, bool statusAtivo);
    }
}
