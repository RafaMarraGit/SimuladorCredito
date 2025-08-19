using Dapper;
using SimuladorCredito.Model;
using SimuladorCredito.Repositories;

namespace SimuladorCredito.Services.Cache;



public class ProdutosStaticService : IDisposable
{
    private static List<Produto> _produtos = new();
    private static readonly object _lock = new();
    private static IServiceScopeFactory? _staticScopeFactory;
    private static Timer? _timer;
    private static bool _initialized;


    public ProdutosStaticService(IServiceScopeFactory scopeFactory)
    {
        
        if (!_initialized)
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    _staticScopeFactory = scopeFactory;
                    Refresh(null); // primeira carga imediata
                    _timer = new Timer(Refresh, null, TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(15));
                    _initialized = true;
                }
            }
        }

        
    }
    private static void Refresh(object? state)
    {

        try
        {
            using var scope = _staticScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DbHackaThonContext>();
            using (var connection = dbContext.CreateConnection())
            {
                _produtos = connection.Query<Produto>("SELECT * FROM PRODUTO;").ToList();
            }
        }
        catch (Exception ex)
        {

           
        }
    }

    public List<Produto> List()
    {
        return Volatile.Read(ref _produtos);
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _timer?.Dispose();
        }
    }
}
