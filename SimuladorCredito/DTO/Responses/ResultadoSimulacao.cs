namespace SimuladorCredito.DTO.Responses;

public class ResultadoSimulacao
{
    public string tipo { get; set; }
    public List<Parcela> parcelas { get; set; }
}
