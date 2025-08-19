using System.Data;
using Microsoft.Data.SqlClient;

namespace SimuladorCredito.Repositories;

public class DbHackaThonContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DbHackaThonContext(IConfiguration configuration)
    {
        _configuration = configuration;

        var password = _configuration["DataBase:DbHackaThon:Password"];
        _connectionString = _configuration["DataBase:DbHackaThon:ConnectionString"] ?? "";

        _connectionString?.Replace("{password}", password);
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
