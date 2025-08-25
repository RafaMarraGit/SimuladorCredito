namespace SimuladorCredito.DTO.Responses;

public class SimulacaoPorDiaResponse
{
    public int CodigoProduto { get; set; }
    public string NomeProduto { get; set; }
    public List<SimulacaoDia> Simulacoes { get; set; }
}

public class SimulacaoDia
{
    public DateTime Data { get; set; }
    public decimal ValorSimulado { get; set; }

}
