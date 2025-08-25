using Microsoft.Data.Sqlite;
using System.Data;

namespace SimuladorCredito.Repositories;

public class SimulacaoContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly string _databasePath;

    public SimulacaoContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration["DataBase:DbSimulacaoLocal:ConnectionString"] ?? "";
        _connectionString = Path.Combine(Directory.GetCurrentDirectory(), _connectionString);
        _databasePath = GetDatabasePath(_connectionString);

        EnsureDatabaseExists();
    }

    public IDbConnection CreateConnection()
        => new SqliteConnection($"Data Source={_connectionString}");

    private void EnsureDatabaseExists()
    {
        if (!File.Exists(_connectionString))
        {
            // Cria o arquivo do banco de dados
            using (var connection = new SqliteConnection($"Data Source={_connectionString}"))
            {
                connection.Open();
                CreateTables(connection);
            }
        }
    }

    private void CreateTables(SqliteConnection connection)
    {
        var createTableQuery = @"
        CREATE TABLE IF NOT EXISTS RespostaSimutacao (
            idSimulacao INTEGER PRIMARY KEY AUTOINCREMENT,
            codigoProduto INTEGER NOT NULL,
            descricaoProduto TEXT NOT NULL,
            taxaJuros REAL NOT NULL,
            dataSimulacao TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS ResultadoSimulacao (
            idResultado INTEGER PRIMARY KEY AUTOINCREMENT,
            idSimulacao INTEGER NOT NULL,
            tipo TEXT NOT NULL,
            FOREIGN KEY (idSimulacao) REFERENCES RespostaSimutacao(idSimulacao)
        );

        CREATE TABLE IF NOT EXISTS Parcela (
            idParcela INTEGER PRIMARY KEY AUTOINCREMENT,
            idResultado INTEGER NOT NULL,
            numero INTEGER NOT NULL,
            valorAmortizacao REAL NOT NULL,
            valorJuros REAL NOT NULL,
            valorPrestacao REAL NOT NULL,
            FOREIGN KEY (idResultado) REFERENCES ResultadoSimulacao(idResultado)
        );
    ";

        using (var command = connection.CreateCommand())
        {
            command.CommandText = createTableQuery;
            command.ExecuteNonQuery();
        }
    }

    private string GetDatabasePath(string connectionString)
    {
        var dataSourcePrefix = "Data Source=";
        var startIndex = connectionString.IndexOf(dataSourcePrefix) + dataSourcePrefix.Length;
        return connectionString.Substring(startIndex).Trim();
    }
}

