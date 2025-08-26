using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace SimuladorCredito.Repositories;

public class SimulacaoContext
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SimulacaoContext> _logger;
    private readonly string _connectionString;
    private readonly string _databasePath;

    public SimulacaoContext(IConfiguration configuration, ILogger<SimulacaoContext> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var dbPath = _configuration["DataBase:DbSimulacaoLocal:ConnectionString"] ?? "simulador.db";

        if (!Path.IsPathRooted(dbPath))
        {
            var basePath = AppContext.BaseDirectory;
            dbPath = Path.Combine(basePath, dbPath);
        }
        dbPath = Path.GetFullPath(dbPath);

        _connectionString = $"Data Source={dbPath}";
        _databasePath = dbPath;

        _logger.LogInformation("Inicializando SimulacaoContext...");
        EnsureDatabaseExists();
        _logger.LogInformation("SimulacaoContext inicializado.");
    }

    public IDbConnection CreateConnection()
    {
        try
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            _logger.LogInformation("Conexão com SQLite estabelecida.");
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao conectar ao SQLite.");
            throw;
        }
    }

    private void EnsureDatabaseExists()
    {
        if (!File.Exists(_databasePath))
        {
            _logger.LogInformation("Criando banco SQLite local...");
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                CreateTables(connection);
            }
            _logger.LogInformation("Banco SQLite local criado com sucesso.");
        }
        else
        {
            _logger.LogInformation("Banco SQLite local já existe.");
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
        _logger.LogInformation("Tabelas criadas/verificadas.");
    }

    private string GetDatabasePath(string connectionString)
    {
        var dataSourcePrefix = "Data Source=";
        var startIndex = connectionString.IndexOf(dataSourcePrefix) + dataSourcePrefix.Length;
        return connectionString.Substring(startIndex).Trim();
    }
}
