using AutoMapper;
using CrudDashboard.Dto;
using CrudDashboard.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using CrudDashboard.Services.Contas;

namespace CrudDashboard.Services
{
    public class ContaService : IContaInterface
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        public ContaService(IConfiguration configuration, IMapper mapper)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("DefaultConnection", "A string de conexão não foi encontrada.");
            _mapper = mapper;
        }

        public async Task<ResponseContas<List<ContaDto>>> BuscarConta()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Contas";
                var conta = await connection.QueryAsync<Conta>(query);

                var contaDto = _mapper.Map<List<ContaDto>>(conta);

                return new ResponseContas<List<ContaDto>>
                {
                    Dados = contaDto,
                    Mensagem = "Dados recuperados com sucesso",
                    Success = true
                };
            }
        }

        public async Task<ResponseContas<ContaDto>> BuscarContaId(int Id)
        {
            ResponseContas<ContaDto> response = new ResponseContas<ContaDto>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM contas WHERE Id = @Id";
                var conta = await connection.QueryFirstOrDefaultAsync<Conta>(query, new { Id = Id });

                if (conta == null)
                {
                    response.Mensagem = "Nenhum usuário localizado!";
                    response.Status = false;
                    return response;
                }

                // Mapear o objeto único
                var contaDto = _mapper.Map<ContaDto>(conta);

                response.Dados = contaDto;
                response.Mensagem = "Conta localizado com sucesso";
                response.Status = true;
            }

            return response;
        }

        public async Task<ResponseContas<ContaDto>> CriarConta(ContaDto contaDto)
        {
            ResponseContas<ContaDto> response = new ResponseContas<ContaDto>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queryInsert = "INSERT INTO contas (Banco, Agencia, NumeroConta) VALUES (@Banco, @Agencia, @NumeroConta); SELECT last_insert_rowid();";
                var contaId = await connection.ExecuteScalarAsync<int>(queryInsert, new
                {
                    Banco = contaDto.Banco,
                    Agencia = contaDto.Agencia,
                    NumeroConta = contaDto.NumeroConta,
                });

                if (contaId == 0)
                {
                    response.Mensagem = "Erro ao criar a conta!";
                    response.Status = false;
                    return response; // Retorno em caso de falha
                }

                var contaCriada = new Conta
                {
                    Id = contaId,
                    Banco = contaDto.Banco,
                    Agencia = contaDto.Agencia,
                    NumeroConta = contaDto.NumeroConta
                };

                response.Dados = _mapper.Map<ContaDto>(contaCriada);
                response.Mensagem = "Conta criada com sucesso!";
                response.Status = true;

                return response; // Retorno em caso de sucesso
            }
        }



        private static async Task<IEnumerable<Conta>> ListarConta(SqliteConnection connection)
        {
            return await connection.QueryAsync<Conta>("SELECT * FROM Contas");
        }

        public async Task<ResponseContas<ContaDto>> EditarConta(ContaDto contaDto)
        {
            ResponseContas<ContaDto> response = new ResponseContas<ContaDto>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "UPDATE Contas SET Banco = @Banco, Agencia = @Agencia, NumeroConta = @NumeroConta, Ativo = @Ativo WHERE Id = @Id";
                var contaAlterado = await connection.ExecuteAsync(query, new
                {
                    Id = contaDto.Id,
                    Banco = contaDto.Banco,
                    Agencia = contaDto.Agencia,
                    NumeroConta = contaDto.NumeroConta,
                    Ativo = contaDto.Ativo
                });

                if (contaAlterado == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao realizar a alteração!";
                    response.Status = false;
                    return response;
                }

                // Recupera o usuário atualizado
                var querySelect = "SELECT * FROM contas WHERE Id = @Id";
                var contaAtualizada = await connection.QueryFirstOrDefaultAsync<Conta>(querySelect, new { Id = contaDto.Id }); // Adicionada a cláusula WHERE e ajustado o parâmetro

                if (contaAtualizada == null)
                {
                    response.Mensagem = "Erro ao buscar o usuário atualizado!";
                    response.Status = false;
                    return response;
                }

                // Mapeia para o DTO
                var contaMapeado = _mapper.Map<ContaDto>(contaAtualizada);

                response.Dados = contaMapeado;
                response.Mensagem = "Conta atualizado com sucesso!";
                response.Status = true;
            }
            return response;
        }

        //public async Task<ResponseContas<List<ContaDto>>> DeletarConta(int Id)
        //{
        //    ResponseContas<List<ContaDto>> response = new ResponseContas<List<ContaDto>>();

        //    using (var connection = new SqliteConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        if (Id <= 0)
        //        {
        //            return new ResponseContas<List<ContaDto>>
        //            {
        //                Mensagem = "Numero Conta inválido!",
        //                Status = false
        //            };
        //        }

        //        var query = "DELETE FROM contas WHERE Id = @Id";
        //        var contaDeletado = await connection.ExecuteAsync(query, new { Id = Id });

        //        if (contaDeletado == 0)
        //        {
        //            response.Mensagem = "Ocorreu um erro ao realizar um registro!";
        //            response.Status = false;
        //            return response;
        //        }

        //        Console.WriteLine($"Executando query para deletar conta com NumeroConta: {Id}");

        //        // Retornar a lista atualizada de usuários
        //        var queryListar = "SELECT * FROM contas";
        //        var conta = await connection.QueryAsync<Conta>(queryListar);

        //        // Mapear para o DTO
        //        var contaMapeados = _mapper.Map<List<ContaDto>>(conta);

        //        response.Dados = contaMapeados;
        //        response.Mensagem = "Conta deletado com sucesso.";
        //        response.Status = true;
        //        };

        //    return response;

        //}

        //    public async Task<ResponseContas<ContaDto>> AlterarStatusConta(int id, bool statusAtivo)
        //    {
        //        ResponseContas<ContaDto> response = new ResponseContas<ContaDto>();

        //        using (var connection = new SqliteConnection(_connectionString))
        //        {
        //            await connection.OpenAsync();

        //            // Atualiza o status da conta (ativo ou inativo)
        //            var query = "UPDATE contas SET Ativo = @Ativo WHERE Id = @Id";
        //            var resultado = await connection.ExecuteAsync(query, new { Ativo = statusAtivo, Id = id });

        //            if (resultado == 0)
        //            {
        //                response.Mensagem = "Erro ao alterar o status da conta.";
        //                response.Status = false;
        //                return response;
        //            }

        //            // Recupera os dados atualizados para retornar
        //            var contaAtualizada = await connection.QueryFirstOrDefaultAsync<Conta>("SELECT * FROM contas WHERE Id = @Id", new { Id = id });

        //            if (contaAtualizada == null)
        //            {
        //                response.Mensagem = "Conta não encontrada após a alteração.";
        //                response.Status = false;
        //                return response;
        //            }

        //            response.Dados = _mapper.Map<ContaDto>(contaAtualizada);
        //            response.Mensagem = statusAtivo ? "Conta ativada com sucesso!" : "Conta inativada com sucesso!";
        //            response.Status = true;
        //        }

        //        return response;
        //    }

        }
    }





