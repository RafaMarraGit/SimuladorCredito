namespace SimuladorCredito.DTO.Responses;

public class ResultadoSimulacao
{
    public long idResultado { get; set; }
    public string tipo { get; set; }
    public List<Parcela> parcelas { get; set; }
}
