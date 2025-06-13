using AutoMapper;
using CrudDashboard.Dto;
using CrudDashboard.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CrudDashboard.Services.Movimentacoes
{
    public class MovimentacoesService : IMovimentacoesInterface
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        public MovimentacoesService(IConfiguration configuration, IMapper mapper)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new ArgumentNullException("DefaultConnection", "A string de conexão não foi encontrada.");
            _mapper = mapper;
        }

        public async Task<ResponseMovimentacoes<List<MovimentacoesDto>>> BuscarMovimentacoes()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Movimentacoes";
                var movimentacoes = await connection.QueryAsync<Movimentacao>(query);

                var movimentacoesDto = _mapper.Map<List<MovimentacoesDto>>(movimentacoes);

                return new ResponseMovimentacoes<List<MovimentacoesDto>>
                {
                    Dados = movimentacoesDto,
                    Mensagem = "Dados recuperados com sucesso",
                    Success = true
                };
            }
        }

        public async Task<ResponseMovimentacoes<List<MovimentacoesDto>>> BuscarMovimentacoesNumeroConta(
    string movimentacoesNumeroConta, int? mes = null, int? ano = null)
        {
            ResponseMovimentacoes<List<MovimentacoesDto>> response = new ResponseMovimentacoes<List<MovimentacoesDto>>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Construir a query dinamicamente
                var query = "SELECT * FROM Movimentacoes WHERE NumeroConta = @NumeroConta";

                if (mes.HasValue)
                    query += " AND strftime('%m', Data) = @Mes"; // Filtra pelo mês

                if (ano.HasValue)
                    query += " AND strftime('%Y', Data) = @Ano"; // Filtra pelo ano


                query += " ORDER BY strftime('%d', Data)"; // Ordena pelo dia

                var parametros = new
                {
                    NumeroConta = movimentacoesNumeroConta,
                    Mes = mes?.ToString("D2"), // Formato 'MM' (ex: '01', '02', etc.)
                    Ano = ano?.ToString()      // Formato 'YYYY'
                };

                var movimentacoes = await connection.QueryAsync<Movimentacao>(query, parametros);

                if (movimentacoes == null || !movimentacoes.Any())
                {
                    response.Mensagem = "Nenhuma movimentação localizada!";
                    response.Status = false;
                    return response;
                }

                // Mapear os objetos para uma lista de DTOs
                var movimentacoesDto = _mapper.Map<List<MovimentacoesDto>>(movimentacoes);

                response.Dados = movimentacoesDto;
                response.Mensagem = "Movimentações localizadas com sucesso.";
                response.Status = true;
            }

            return response;
        }

        
        public async Task<ResponseMovimentacoes<MovimentacoesCadastrarDto>> CriarMovimentacoes(MovimentacoesCadastrarDto movimentacoesCadastrarDto)
        {
            ResponseMovimentacoes<MovimentacoesCadastrarDto> response = new ResponseMovimentacoes<MovimentacoesCadastrarDto>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Verifica se a conta existe e está ativa
                var conta = await connection.QueryFirstOrDefaultAsync<Conta>(
                    "SELECT * FROM Contas WHERE NumeroConta = @NumeroConta",
                    new { NumeroConta = movimentacoesCadastrarDto.NumeroConta });

                if (conta == null || !conta.Ativo)
                {
                    return new ResponseMovimentacoes<MovimentacoesCadastrarDto>
                    {
                        Status = false,
                        Mensagem = "A conta não existe ou está inativa. Não é possível criar movimentações para esta conta."
                    };
                }

                // Insere a movimentação
                var query = "INSERT INTO Movimentacoes (NumeroConta, Data, Historico, Valor) VALUES (@NumeroConta, @Data, @Historico, @Valor)";
                var movimentacoesInseridas = await connection.ExecuteAsync(query, new
                {
                    NumeroConta = movimentacoesCadastrarDto.NumeroConta,
                    Data = movimentacoesCadastrarDto.Data,
                    Historico = movimentacoesCadastrarDto.Historico,
                    Valor = movimentacoesCadastrarDto.Valor
                });

                if (movimentacoesInseridas == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao registrar a movimentação!";
                    response.Status = false;
                    return response;
                }

                // Recupera a movimentação recém-inserida pelo último ID
                var movimentacaoCriada = await connection.QueryFirstOrDefaultAsync<Movimentacao>(
                    "SELECT * FROM Movimentacoes WHERE Id = last_insert_rowid()");

                if (movimentacaoCriada == null)
                {
                    response.Mensagem = "Erro ao recuperar a movimentação recém-criada.";
                    response.Status = false;
                    return response;
                }

                // Mapeia o registro para o DTO
                var movimentacaoMapeada = _mapper.Map<MovimentacoesCadastrarDto>(movimentacaoCriada);

                response.Dados = movimentacaoMapeada;
                response.Mensagem = "Movimentação criada com sucesso!";
                response.Status = true;
            }

            return response;
        }



        private static async Task<IEnumerable<Movimentacao>> ListarMovimentacoes(SqliteConnection connection)
        {
            return await connection.QueryAsync<Movimentacao>("SELECT * FROM Movimentacoes WHERE NumeroConta  = @NumeroConta");
        }

        public async Task<ResponseMovimentacoes<MovimentacoesConferidoDto>> EditarMovimentacoes(MovimentacoesConferidoDto movimentacoesConferidoDto)
        {
            ResponseMovimentacoes<MovimentacoesConferidoDto> response = new ResponseMovimentacoes<MovimentacoesConferidoDto>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "UPDATE Movimentacoes SET Data = @Data, Historico = @Historico, Valor = @Valor, Conferido = @Conferido WHERE Id = @Id;";
                var movimentacaoAlterado = await connection.ExecuteAsync(query, new
                {
                    Id = movimentacoesConferidoDto.Id,
                    Data = movimentacoesConferidoDto.Data,
                    Historico = movimentacoesConferidoDto.Historico,
                    Valor = movimentacoesConferidoDto.Valor,
                    Conferido = movimentacoesConferidoDto.Conferido,
                });

                if (movimentacaoAlterado == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao realizar a alteração!";
                    response.Status = false;
                    return response;
                }

                // Busca os dados atualizados
                var movimentacoesAtualizado = await connection.QueryFirstOrDefaultAsync<MovimentacoesConferidoDto>(
                    "SELECT * FROM Movimentacoes WHERE Id = @Id", new { Id = movimentacoesConferidoDto.Id });

                if (movimentacoesAtualizado == null)
                {
                    response.Mensagem = "Erro ao buscar a movimentação atualizada!";
                    response.Status = false;
                    return response;
                }

                response.Dados = movimentacoesAtualizado;
                response.Mensagem = "Movimentação atualizada com sucesso!";
                response.Status = true;
            }
            return response;
        }


        public async Task<ResponseMovimentacoes<List<MovimentacoesDto>>> DeletarMovimentacoes(int Id)
        {
            ResponseMovimentacoes<List<MovimentacoesDto>> response = new ResponseMovimentacoes<List<MovimentacoesDto>>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                if (Id <= 0)
                {
                    return new ResponseMovimentacoes<List<MovimentacoesDto>>
                    {
                        Mensagem = "ID inválido!",
                        Status = false
                    };
                }

                var query = "DELETE FROM movimentacoes WHERE Id = @Id";
                var movimentacaoDeletado = await connection.ExecuteAsync(query, new { Id = Id });

                if (movimentacaoDeletado == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao realizar um registro!";
                    response.Status = false;
                    return response;
                }

                Console.WriteLine($"Executando query para deletar usuário com Id : {Id}");

                
                var queryListar = "SELECT * FROM movimentacoes";
                var movimentacao = await connection.QueryAsync<Movimentacao>(queryListar);

                // Mapear para o DTO
                var movimentacoesMapeados = _mapper.Map<List<MovimentacoesDto>>(movimentacao);

                response.Dados = movimentacoesMapeados;
                response.Mensagem = "Movimentacao deletado com sucesso.";
                response.Status = true;
             };

            return response;
        }

        public async Task<SaldoDto> ObterSaldos(string numeroConta, int? mes = null, int? ano = null)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"SELECT COALESCE(SUM(valor), 0) AS SaldoAtual 
                FROM movimentacoes 
                WHERE numeroConta = @NumeroConta
                AND Conferido = 1
                AND DATE(Data) <= DATE('now');

                SELECT 
                    COALESCE(SUM(CASE WHEN valor > 0 THEN valor ELSE 0 END), 0) AS TotalEntradas,
                    COALESCE(SUM(CASE WHEN valor < 0 THEN valor ELSE 0 END), 0) AS TotalSaidas,
                    COALESCE(SUM(valor), 0) AS SaldoProjetado
                    FROM movimentacoes
                        WHERE numeroConta = @NumeroConta";


            if (mes.HasValue)
                query += " AND strftime('%m', Data) = @Mes"; // Filtra pelo mês

            if (ano.HasValue)
                query += " AND strftime('%Y', Data) = @Ano"; // Filtra pelo ano

            
            var parametros = new
            {
                NumeroConta = numeroConta,
                Mes = mes?.ToString("D2"), // Garante que sempre será string no formato 'MM'
                Ano = ano?.ToString() // Garante que sempre será string no formato 'YYYY'

            };
            
            using var multi = await connection.QueryMultipleAsync(query, parametros);

            var saldoAtual = await multi.ReadSingleAsync<decimal>();
            var saldos = await multi.ReadSingleAsync<SaldoDto>();

            return new SaldoDto
            {
                SaldoAtual = saldoAtual,
                TotalEntradas = saldos.TotalEntradas,
                TotalSaidas = saldos.TotalSaidas,
                SaldoProjetado = saldos.SaldoProjetado
            };
        }

    }
}
