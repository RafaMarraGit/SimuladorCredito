using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System.IO;

namespace SimuladorCredito.Repositories;

public class DbHackaThonContext
{
    private readonly IConfiguration _configuration;
    private readonly string _sqlServerConnectionString;
    private readonly string _sqliteDatabasePath;

    public DbHackaThonContext(IConfiguration configuration)
    {
        _configuration = configuration;

        var password = _configuration["DataBase:DbHackaThon:Password"];
        _sqlServerConnectionString = _configuration["DataBase:DbHackaThon:ConnectionString"]?.Replace("{password}", password) ?? "";

        // Define o caminho do banco local "hackthon.db"
        _sqliteDatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "Repositories", "hackthon.db");

        EnsureLocalDatabaseExists();
    }

    public IDbConnection CreateConnection()
    {
        try
        {
            var connection = new SqlConnection(_sqlServerConnectionString);
            connection.Open(); // Testa a conexão com o SQL Server
            return connection;
        }
        catch
        {
            // Caso a conexão com o SQL Server falhe, usa o banco SQLite local
            return new SqliteConnection($"Data Source={_sqliteDatabasePath}");
        }
    }

    private void EnsureLocalDatabaseExists()
    {
        if (!File.Exists(_sqliteDatabasePath))
        {
            using (var connection = new SqliteConnection($"Data Source={_sqliteDatabasePath}"))
            {
                connection.Open();
                CreateTables(connection);
                SeedData(connection);
            }
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
    }
}
