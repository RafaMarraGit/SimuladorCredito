using Dapper;
using Microsoft.AspNetCore.Mvc;
using SimuladorCredito.DTO.Requests;
using SimuladorCredito.DTO.Responses;
using SimuladorCredito.Model;
using SimuladorCredito.Repositories;
using SimuladorCredito.Services;
using SimuladorCredito.Services.Cache;
using System.Net.Mime;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimuladorCredito.Controllers
{
    [Route("v1/Simulador")]
    [ApiController]
    public class SimuladorCreditoController : ControllerBase
    {
        private readonly DapperContext _dapperContext;
        private readonly ProdutosStaticService _produtosStaticService;
        private readonly CalculoSimulacaoService _calculoSimulacaoService;

        public SimuladorCreditoController(
            DapperContext dapperContext,
            ProdutosStaticService produtosStaticService,
            CalculoSimulacaoService calculoSimulacaoService
            )
        {
            _dapperContext = dapperContext;
            _produtosStaticService = produtosStaticService;
            _calculoSimulacaoService = calculoSimulacaoService;
        }


        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<RespostaSimutacao>> SimularCredito(RequisicaoSimulacao requisicao)
        {
            var produtoValido = _produtosStaticService.List().FirstOrDefault(p =>
                requisicao.valorDesejado >= p.VR_MINIMO &&
                (p.VR_MAXIMO == null || requisicao.valorDesejado <= p.VR_MAXIMO) &&
                requisicao.prazo >= p.NU_MINIMO_MESES &&
                (p.NU_MAXIMO_MESES == null || requisicao.prazo <= p.NU_MAXIMO_MESES)
            );

            if (produtoValido == null)
                return BadRequest("Nenhum produto disponível para os parâmetros informados.");


            var resposta = new RespostaSimutacao
            {
                idSimulacao = 1,
                codigoProduto = produtoValido.CO_PRODUTO,
                descricaoProduto = produtoValido.NO_PRODUTO,
                taxaJuros = produtoValido.PC_TAXA_JUROS,
                resultadoSimulacao = new List<ResultadoSimulacao>
                {
                    new ResultadoSimulacao
                    {
                        tipo = "SAC",
                        parcelas = _calculoSimulacaoService.CalcularSAC(
                            requisicao.valorDesejado, 
                            requisicao.prazo, 
                            produtoValido.PC_TAXA_JUROS
                        )
                    },
                    new ResultadoSimulacao
                    {
                        tipo = "PRICE",
                        parcelas = _calculoSimulacaoService.CalcularPRICE(
                            requisicao.valorDesejado, 
                            requisicao.prazo, 
                            produtoValido.PC_TAXA_JUROS
                        )
                    }
                }
            };




            return Ok(resposta);
        }

    }
}
