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
        private readonly ProdutosStaticService _produtosStaticService;
        private readonly CalculoSimulacaoService _calculoSimulacaoService;
        private readonly SimulacaoRepository _simulacaoRepository;

        public SimuladorCreditoController(
            ProdutosStaticService produtosStaticService,
            CalculoSimulacaoService calculoSimulacaoService,
            SimulacaoRepository simulacaoRepository
            )
        {
            _produtosStaticService = produtosStaticService;
            _calculoSimulacaoService = calculoSimulacaoService;
            _simulacaoRepository = simulacaoRepository;
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

            Task.Run(() => _simulacaoRepository.InserirSimulacaoAsync(resposta));


            return Ok(resposta);
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<RespostaSimutacao>>> ListarSimulacoes(
            [FromQuery] int pagina = 1,
            [FromQuery] int quantidadePorPagina = 10,
            [FromQuery] bool parcelas = false)
        {
            var simulacoes = await _simulacaoRepository.ObterSimulacoesPaginadasAsync(pagina, quantidadePorPagina, parcelas);
            return Ok(simulacoes);
        }

        [HttpGet("AgrupadoPorDia")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> ListarSimulacoesAgrupadasPorDia(
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            var resultado = await _simulacaoRepository.ObterValoresSimuladosPorDiaAsync(dataInicio, dataFim);
            return Ok(resultado);
        }


    }
}
