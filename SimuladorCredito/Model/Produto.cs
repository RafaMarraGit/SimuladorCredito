namespace SimuladorCredito.Model;

public class Produto
{
    public int CO_PRODUTO { get; set; }
    public string NO_PRODUTO { get; set; }
    public decimal PC_TAXA_JUROS { get; set; }
    public int NU_MINIMO_MESES { get; set; }
    public int? NU_MAXIMO_MESES { get; set; }
    public decimal VR_MINIMO { get; set; }
    public decimal? VR_MAXIMO { get; set; }
}