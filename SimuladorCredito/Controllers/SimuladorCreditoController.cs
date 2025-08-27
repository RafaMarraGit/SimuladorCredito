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
        private readonly EventHubStreamingService _eventHubStreamingService;
        private readonly ILogger<SimuladorCreditoController> _logger;

        public SimuladorCreditoController(
            ProdutosStaticService produtosStaticService,
            CalculoSimulacaoService calculoSimulacaoService,
            SimulacaoRepository simulacaoRepository,
            EventHubStreamingService eventHubStreamingService,
            ILogger<SimuladorCreditoController> logger
            )
        {
            _produtosStaticService = produtosStaticService;
            _calculoSimulacaoService = calculoSimulacaoService;
            _simulacaoRepository = simulacaoRepository;
            _eventHubStreamingService = eventHubStreamingService;
            _logger = logger;
        }


        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<RespostaSimulacao>> SimularCredito(RequisicaoSimulacao requisicao)
        {
            try
            {

                var produtoValido = _produtosStaticService.List()
                    .FirstOrDefault(p =>
                                requisicao.valorDesejado >= p.VR_MINIMO &&
                                (p.VR_MAXIMO == null || requisicao.valorDesejado <= p.VR_MAXIMO) &&
                                requisicao.prazo >= p.NU_MINIMO_MESES &&
                                (p.NU_MAXIMO_MESES == null || requisicao.prazo <= p.NU_MAXIMO_MESES)
                            );

                if (produtoValido == null)
                    return BadRequest("Nenhum produto disponível para os parâmetros informados.");


                var resposta = _calculoSimulacaoService.GerarRespostaSimulacao(requisicao, produtoValido);


                resposta.idSimulacao = await _simulacaoRepository.InserirSimulacaoAsync(resposta);

                _ = Task.Run(async () =>
                {
                    await _eventHubStreamingService.EnviarSimulacaoAsync(resposta);
                });

                return Ok(resposta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao simular crédito.");
                return StatusCode(500, new { erro = "Erro interno ao processar a simulação.", detalhes = ex.Message });
            }

        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<RespostaSimulacao>>> ListarSimulacoes(
            [FromQuery] int pagina = 1,
            [FromQuery] int quantidadePorPagina = 10,
            [FromQuery] bool parcelas = false)
        {

            try
            {

                var simulacoes = await _simulacaoRepository.ObterSimulacoesPaginadasAsync(pagina, quantidadePorPagina, parcelas);
           
                if (simulacoes == null || !simulacoes.Any()) return NoContent();

                return Ok(simulacoes);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Erro interno ao listar simulações.");
                return StatusCode(500, new { erro = "Erro interno ao listar simulações.", detalhes = ex.Message });
            }
        }

        [HttpGet("AgrupadoPorDia")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> ListarSimulacoesAgrupadasPorDia(
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            try
            {

                var resultado = await _simulacaoRepository.ObterValoresSimuladosPorDiaAsync(dataInicio, dataFim);

                if (resultado == null || !resultado.Any()) return NoContent();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao listar simulações agrupadas por dia.");
                return StatusCode(500, new { erro = "Erro interno ao listar simulações agrupadas por dia.", detalhes = ex.Message });
            }
        }


    }
}
