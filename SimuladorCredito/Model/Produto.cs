namespace SimuladorCredito.Model;

public class Produto
{
    public int CoProduto { get; set; } // CO_PRODUTO
    public string NoProduto { get; set; } // NO_PRODUTO
    public decimal PcTaxaJuros { get; set; } // PC_TAXA_JUROS
    public short NuMinimoMeses { get; set; } // NU_MINIMO_MESES
    public short? NuMaximoMeses { get; set; } // NU_MAXIMO_MESES (nullable)
    public decimal VrMinimo { get; set; } // VR_MINIMO
    public decimal? VrMaximo { get; set; } // VR_MAXIMO (nullable)
}