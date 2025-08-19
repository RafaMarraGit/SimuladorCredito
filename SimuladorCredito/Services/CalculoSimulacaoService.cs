using SimuladorCredito.DTO.Responses;

namespace SimuladorCredito.Services;

public class CalculoSimulacaoService
{

    public List<Parcela> CalcularSAC(decimal valor, int prazo, decimal taxa)
    {
        var parcelas = new List<Parcela>();
        decimal amortizacao = Math.Round(valor / prazo, 2);
        for (int i = 1; i <= prazo; i++)
        {
            decimal saldoDevedor = valor - amortizacao * (i - 1);
            decimal juros = Math.Round(saldoDevedor * taxa, 2);
            decimal prestacao = Math.Round(amortizacao + juros, 2);
            parcelas.Add(new Parcela
            {
                numero = i,
                valorAmortizacao = amortizacao,
                valorJuros = juros,
                valorPrestacao = prestacao
            });
        }
        return parcelas;
    }

    public List<Parcela> CalcularPRICE(decimal valor, int prazo, decimal taxa)
    {
        var parcelas = new List<Parcela>();
        decimal fator = (decimal)Math.Pow((double)(1 + taxa), prazo);
        decimal prestacao = Math.Round(valor * (taxa * fator) / (fator - 1), 2);
        decimal saldoDevedor = valor;
        for (int i = 1; i <= prazo; i++)
        {
            decimal juros = Math.Round(saldoDevedor * taxa, 2);
            decimal amortizacao = Math.Round(prestacao - juros, 2);
            saldoDevedor -= amortizacao;
            parcelas.Add(new Parcela
            {
                numero = i,
                valorAmortizacao = amortizacao,
                valorJuros = juros,
                valorPrestacao = prestacao
            });
        }
        return parcelas;
    }
}
