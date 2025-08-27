using Dapper;
using Microsoft.Extensions.Logging;
using SimuladorCredito.DTO.Responses;

namespace SimuladorCredito.Repositories;

public interface ISimulacaoRepository
{
    Task<long> InserirSimulacaoAsync(RespostaSimulacao respostaSimulacao);
    Task<IEnumerable<RespostaSimulacao>> ObterSimulacoesPaginadasAsync(int pagina, int quantidadePorPagina, bool incluirParcelas);
    Task<IEnumerable<object>> ObterValoresSimuladosPorDiaAsync(DateTime? dataInicio, DateTime? dataFim);
}


public class SimulacaoRepository : ISimulacaoRepository
{
    private readonly SimulacaoContext _simulacaoContext;
    private readonly ILogger<SimulacaoRepository> _logger;

    public SimulacaoRepository(SimulacaoContext simulacaoContext, ILogger<SimulacaoRepository> logger)
    {
        _simulacaoContext = simulacaoContext;
        _logger = logger;
    }


    public async Task<Int64> InserirSimulacaoAsync(RespostaSimulacao respostaSimulacao)
    {
        try
        {
            using var connection = _simulacaoContext.CreateConnection();

            // Insere Simulacao
            var simulacaoId = await connection.ExecuteScalarAsync<long>(
                @"INSERT INTO RespostaSimulacao (CodigoProduto, DescricaoProduto, TaxaJuros, DataSimulacao)
                  VALUES (@CodigoProduto, @DescricaoProduto, @TaxaJuros, @DataSimulacao);
                  SELECT last_insert_rowid();",
                new
                {
                    CodigoProduto = respostaSimulacao.codigoProduto,
                    DescricaoProduto = respostaSimulacao.descricaoProduto,
                    TaxaJuros = respostaSimulacao.taxaJuros,
                    DataSimulacao = DateTime.UtcNow // ou DateTime.Now, conforme necessidade
                });

            foreach (var resultado in respostaSimulacao.resultadoSimulacao)
            {
                // Insere ResultadoSimulacao
                var resultadoId = await connection.ExecuteScalarAsync<long>(
                    @"INSERT INTO ResultadoSimulacao (IdSimulacao, Tipo)
                    VALUES (@IdSimulacao, @Tipo);
                    SELECT last_insert_rowid();",
                    new
                    {
                        IdSimulacao = simulacaoId,
                        Tipo = resultado.tipo
                    });

                // Insere Parcelas
                foreach (var parcela in resultado.parcelas)
                {
                    await connection.ExecuteAsync(
                        @"INSERT INTO Parcela (IdResultado, Numero, 
                        ValorAmortizacao, ValorJuros, ValorPrestacao)
                        VALUES (@IdResultado, @Numero, @ValorAmortizacao, 
                        @ValorJuros, @ValorPrestacao);",
                        new
                        {
                            IdResultado = resultadoId,
                            Numero = parcela.numero,
                            ValorAmortizacao = parcela.valorAmortizacao,
                            ValorJuros = parcela.valorJuros,
                            ValorPrestacao = parcela.valorPrestacao
                        });
                }
            }
                
            return simulacaoId;
        }
        catch (Exception ex)
        {
            _logger.LogError("{message} => {data}", ex.Message, DateTime.UtcNow);
            throw; 
        }
    }


    public async Task<IEnumerable<RespostaSimulacao>> ObterSimulacoesPaginadasAsync(int pagina, int quantidadePorPagina, bool incluirParcelas)
    {
        try
        {

            using var connection = _simulacaoContext.CreateConnection();
            var offset = (pagina - 1) * quantidadePorPagina;
            var sql = @"
                SELECT IdSimulacao as idSimulacao, 
                CodigoProduto as codigoProduto, 
                DescricaoProduto as descricaoProduto, TaxaJuros as taxaJuros, DataSimulacao as dataSimulacao
                FROM RespostaSimulacao
                ORDER BY IdSimulacao DESC
                LIMIT @Quantidade OFFSET @Offset";
            var simulacoes = (await connection.QueryAsync<RespostaSimulacao>(sql, new { Quantidade = quantidadePorPagina, Offset = offset })).ToList();

            if (incluirParcelas)
            {
                foreach (var simulacao in simulacoes)
                {
                    // Busca os resultados da simulação
                    var resultados = (await connection.QueryAsync<ResultadoSimulacao>(
                        @"SELECT IdResultado as idResultado, Tipo as tipo
                        FROM ResultadoSimulacao
                        WHERE IdSimulacao = @IdSimulacao",
                        new { IdSimulacao = simulacao.idSimulacao })).ToList();

                    foreach (var resultado in resultados)
                    {
                        // Busca as parcelas de cada resultado
                        var parcelas = (await connection.QueryAsync<Parcela>(
                            @"SELECT Numero as numero, 
                            ValorAmortizacao as valorAmortizacao, 
                            ValorJuros as valorJuros, ValorPrestacao as valorPrestacao
                            FROM Parcela
                            WHERE IdResultado = @IdResultado",
                            new { IdResultado = resultado.idResultado })).ToList();

                        resultado.parcelas = parcelas;
                    }

                    simulacao.resultadoSimulacao = resultados;
                }
            }

            return simulacoes;
        }
        catch (Exception ex)
        {
            _logger.LogError("{message} => {data}", ex.Message, DateTime.UtcNow);
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<object>> ObterValoresSimuladosPorDiaAsync(DateTime? dataInicio, DateTime? dataFim)
    {
        using var connection = _simulacaoContext.CreateConnection();

        var fim = dataFim ?? DateTime.Today;
        var inicio = dataInicio ?? fim.AddDays(-6);

        var sql = @"
            SELECT 
                date(dataSimulacao) as dataSimulacao, 
                codigoProduto, 
                descricaoProduto, 
                SUM(taxaJuros) as totalTaxaJuros, 
                COUNT(*) as totalSimulacoes
            FROM RespostaSimulacao
            WHERE date(dataSimulacao) BETWEEN date(@Inicio) AND date(@Fim)
            GROUP BY date(dataSimulacao), codigoProduto, descricaoProduto
            ORDER BY date(dataSimulacao) DESC, codigoProduto
        ";

        var result = await connection.QueryAsync(sql, new { Inicio = inicio.ToString("yyyy-MM-dd"), Fim = fim.ToString("yyyy-MM-dd") });
        return result;
    }


}

