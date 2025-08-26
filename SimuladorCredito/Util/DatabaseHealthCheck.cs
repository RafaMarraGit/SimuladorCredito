using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimuladorCredito.Repositories;

namespace SimuladorCredito.Util;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly SimulacaoContext _dbContext;

    public DatabaseHealthCheck(SimulacaoContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Tenta abrir uma conexão com o banco
            _dbContext.CreateConnection();
            return HealthCheckResult.Healthy("Conexão com o banco de dados está saudável.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao conectar ao banco de dados.", ex);
        }
    }

   

}
