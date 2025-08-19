using Microsoft.Data.Sqlite;
using System.Data;

namespace SimuladorCredito.Repositories;

public class DapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly string _databasePath;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration["DataBase:ConnectionStrings"];
        _databasePath = GetDatabasePath(_connectionString);

        EnsureDatabaseExists();
    }

    public IDbConnection CreateConnection()
        => new SqliteConnection(_connectionString);

    private void EnsureDatabaseExists()
    {
        if (!File.Exists(_databasePath))
        {
            // Cria o arquivo do banco de dados
            using (var connection = new SqliteConnection(_connectionString))
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
                idSimulacao INTEGER PRIMARY KEY,
                codigoProduto INTEGER NOT NULL,
                descricaoProduto TEXT NOT NULL,
                taxaJuros REAL NOT NULL
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

