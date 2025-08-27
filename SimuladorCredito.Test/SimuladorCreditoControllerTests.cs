using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SimuladorCredito.Controllers;
using SimuladorCredito.DTO.Requests;
using SimuladorCredito.DTO.Responses;
using SimuladorCredito.Model;
using SimuladorCredito.Repositories;
using SimuladorCredito.Services;
using SimuladorCredito.Services.Cache;
using Xunit;

public class SimuladorCreditoControllerTests
{
    private readonly Mock<IProdutosStaticService> _mockProdutosStaticService;
    private readonly Mock<ICalculoSimulacaoService> _mockCalculoSimulacaoService;
    private readonly Mock<ISimulacaoRepository> _mockSimulacaoRepository;
    private readonly Mock<IEventHubStreamingService> _mockEventHubStreamingService;
    private readonly Mock<ILogger<SimuladorCreditoController>> _mockLogger;
    private readonly SimuladorCreditoController _controller;

    public SimuladorCreditoControllerTests()
    {
        _mockProdutosStaticService = new Mock<IProdutosStaticService>();
        _mockCalculoSimulacaoService = new Mock<ICalculoSimulacaoService>();
        _mockSimulacaoRepository = new Mock<ISimulacaoRepository>();
        _mockEventHubStreamingService = new Mock<IEventHubStreamingService>();
        _mockLogger = new Mock<ILogger<SimuladorCreditoController>>();

        _controller = new SimuladorCreditoController(
            _mockProdutosStaticService.Object,
            _mockCalculoSimulacaoService.Object,
            _mockSimulacaoRepository.Object,
            _mockEventHubStreamingService.Object,
            _mockLogger.Object
        );

        _mockProdutosStaticService.Setup(s => s.List()).Returns(new List<Produto>
        {
            new Produto { VR_MINIMO = 1000, VR_MAXIMO = 10000, NU_MINIMO_MESES = 6, NU_MAXIMO_MESES = 24 }
        });
    }

    [Fact]
    public async Task SimularCredito_DeveRetornarOk_QuandoSimulacaoForValida()
    {
        // Arrange
        var requisicao = new RequisicaoSimulacao { valorDesejado = 5000, prazo = 12 };
        var produto = new Produto { VR_MINIMO = 1000, VR_MAXIMO = 10000, NU_MINIMO_MESES = 6, NU_MAXIMO_MESES = 24 };
        var respostaSimulacao = new RespostaSimulacao { idSimulacao = 1 };

        _mockProdutosStaticService.Setup(s => s.List()).Returns(new List<Produto> { produto });
        _mockCalculoSimulacaoService.Setup(s => s.GerarRespostaSimulacao(requisicao, produto)).Returns(respostaSimulacao);
        _mockSimulacaoRepository.Setup(r => r.InserirSimulacaoAsync(respostaSimulacao)).ReturnsAsync(1);

        // Act
        var result = await _controller.SimularCredito(requisicao);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var resposta = Assert.IsType<RespostaSimulacao>(okResult.Value);
        Assert.Equal(1, resposta.idSimulacao);
    }

    [Fact]
    public async Task ListarSimulacoes_DeveRetornarNoContent_QuandoNaoHouverSimulacoes()
    {
        // Arrange
        _mockSimulacaoRepository.Setup(r => r.ObterSimulacoesPaginadasAsync(1, 10, false))
            .ReturnsAsync(new List<RespostaSimulacao>());

        // Act
        var result = await _controller.ListarSimulacoes();

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }
}
