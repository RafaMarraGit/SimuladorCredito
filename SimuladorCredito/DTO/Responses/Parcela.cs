namespace SimuladorCredito.DTO.Responses;

public class Parcela
{
    public int numero { get; set; }
    public decimal valorAmortizacao { get; set; }
    public decimal valorJuros { get; set; }
    public decimal valorPrestacao { get; set; }
}
