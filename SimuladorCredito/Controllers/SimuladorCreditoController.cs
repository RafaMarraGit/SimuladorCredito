using Microsoft.AspNetCore.Mvc;
using SimuladorCredito.DTO.Requests;
using SimuladorCredito.DTO.Responses;
using SimuladorCredito.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimuladorCredito.Controllers
{
    [Route("v1/Simulador")]
    [ApiController]
    public class SimuladorCreditoController : ControllerBase
    {
        private readonly DapperContext _dapperContext;

        public SimuladorCreditoController(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }


        [HttpPost]
        public async Task<ActionResult> SimularCredito(RequisicaoSimulacao requisicao)
        {
            if (requisicao == null || requisicao.valorDesejado <= 0 || requisicao.prazo <= 0)
            {
                return BadRequest("Requisição inválida.");
            }
            
            var resposta = new RespostaSimutacao
            {
                idSimulacao = 1,
                codigoProduto = 123,
                descricaoProduto = "Crédito Pessoal",
                taxaJuros = 1.5,
                resultadoSimulacao = new List<ResultadoSimulacao>
                {
                    new ResultadoSimulacao
                    {
                        tipo = "Parcelado",
                        parcelas = new List<Parcela>
                        {
                            new Parcela { numero = 1, valorAmortizacao = 100, valorJuros = 10, valorPrestacao = 110 },
                            new Parcela { numero = 2, valorAmortizacao = 100, valorJuros = 9, valorPrestacao = 109 }
                        }
                    }
                }
            };
            return Ok(resposta);
        }

    }
}
