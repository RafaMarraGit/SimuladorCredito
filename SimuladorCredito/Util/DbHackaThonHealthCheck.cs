using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimuladorCredito.Repositories;

namespace SimuladorCredito.Util;

public class DbHackaThonHealthCheck : IHealthCheck
{
    private readonly DbHackaThonContext _dbContext;

    public DbHackaThonHealthCheck(DbHackaThonContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _dbContext.CreateConnection();
            return HealthCheckResult.Healthy("Conexão com o banco HackaThon está saudável.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao conectar ao banco HackaThon.", ex);
        }
    }
}
