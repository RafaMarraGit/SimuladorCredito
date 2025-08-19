namespace SimuladorCredito.DTO.Responses;

public class RespostaSimutacao
{
    public int idSimulacao { get; set; }
    public int codigoProduto { get; set; }
    public string descricaoProduto { get; set; }
    public double taxaJuros { get; set; }
    public List<ResultadoSimulacao> resultadoSimulacao { get; set; }
}