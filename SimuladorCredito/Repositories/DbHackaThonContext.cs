using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SimuladorCredito.Repositories;

public class DbHackaThonContext
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbHackaThonContext> _logger;
    private readonly string _sqlServerConnectionString;
    private readonly string _sqliteDatabasePath;

    public DbHackaThonContext(IConfiguration configuration, ILogger<DbHackaThonContext> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var password = _configuration["DataBase:DbHackaThon:Password"];
        _sqlServerConnectionString = _configuration["DataBase:DbHackaThon:ConnectionString"]?.Replace("{password}", password) ?? "";

        _sqliteDatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "hackthon.db");

        _logger.LogInformation("Inicializando DbHackaThonContext...");
        EnsureLocalDatabaseExists();
        _logger.LogInformation("DbHackaThonContext inicializado.");
    }

    public IDbConnection CreateConnection()
    {
        try
        {
            var connection = new SqlConnection(_sqlServerConnectionString);
            connection.Open(); // Testa a conexão com o SQL Server
            _logger.LogInformation("Conexão com SQL Server estabelecida.");
            return connection;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao conectar ao SQL Server. Usando SQLite local.");
            var sqliteConnection = new SqliteConnection($"Data Source={_sqliteDatabasePath}");
            try
            {
                sqliteConnection.Open();
                _logger.LogInformation("Conexão com SQLite local estabelecida.");
            }
            catch (Exception sqliteEx)
            {
                _logger.LogError(sqliteEx, "Falha ao conectar ao SQLite local.");
                throw;
            }
            return sqliteConnection;
        }
    }

    private void EnsureLocalDatabaseExists()
    {
        if (!File.Exists(_sqliteDatabasePath))
        {
            _logger.LogInformation("Criando banco SQLite local...");
            using (var connection = new SqliteConnection($"Data Source={_sqliteDatabasePath}"))
            {
                connection.Open();
                CreateTables(connection);
                SeedData(connection);
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
            CREATE TABLE IF NOT EXISTS PRODUTO (
                CO_PRODUTO INTEGER NOT NULL PRIMARY KEY,
                NO_PRODUTO TEXT NOT NULL,
                PC_TAXA_JUROS REAL NOT NULL,
                NU_MINIMO_MESES INTEGER NOT NULL,
                NU_MAXIMO_MESES INTEGER NULL,
                VR_MINIMO REAL NOT NULL,
                VR_MAXIMO REAL NULL
            );
        ";

        using (var command = connection.CreateCommand())
        {
            command.CommandText = createTableQuery;
            command.ExecuteNonQuery();
        }
        _logger.LogInformation("Tabela PRODUTO criada/verificada.");
    }

    private void SeedData(SqliteConnection connection)
    {
        var insertDataQuery = @"
            INSERT INTO PRODUTO (CO_PRODUTO, NO_PRODUTO, PC_TAXA_JUROS, NU_MINIMO_MESES, NU_MAXIMO_MESES, VR_MINIMO, VR_MAXIMO) VALUES 
            (1, 'Produto 1', 0.017900000, 0, 24, 200.00, 10000.00),
            (2, 'Produto 2', 0.017500000, 25, 48, 10001.00, 100000.00),
            (3, 'Produto 3', 0.018200000, 49, 96, 100000.01, 1000000.00),
            (4, 'Produto 4', 0.015100000, 96, NULL, 1000000.01, NULL);
        ";

        using (var command = connection.CreateCommand())
        {
            command.CommandText = insertDataQuery;
            command.ExecuteNonQuery();
        }
        _logger.LogInformation("Dados de PRODUTO inseridos.");
    }
}
