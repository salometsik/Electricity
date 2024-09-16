using Electricity.Application.Contracts.Infrastructure;
using Electricity.Infrastructure.Dataset;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace Electricity.Tests.StoreElectricityData
{
    public class CsvParserTests
    {
        readonly Mock<HttpClient> _httpClientMock;
        readonly Mock<ILogger<RetrieveDataService>> _loggerMock;
        readonly IRetrieveDataService _service;
        readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        public CsvParserTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClientMock = new Mock<HttpClient>(_httpMessageHandlerMock.Object);
            _loggerMock = new Mock<ILogger<RetrieveDataService>>();
            _service = new RetrieveDataService(_loggerMock.Object, _httpClientMock.Object);
        }

        [Fact]
        public async Task ShouldReturnEmpty_WhenNoCsvLinksFound()
        {
            var emptyHtmlResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("<html></html>")
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(emptyHtmlResponse);

            var result = await _service.GetElectricityData();
            Assert.Empty(result);
        }
        [Fact]
        public async Task ShouldReturnData_WhenCsvLinksArePresent()
        {
            var htmlWithCsvLink = @"
                <html>
                    <body>
                        <a href='/file1.csv'>file1.csv</a>
                    </body>
                </html>";

            var htmlResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(htmlWithCsvLink)
            };

            var csvResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("TINKLAS,OBT_PAVADINIMAS,OBJ_GV_TIPAS,OBJ_NUMERIS,P+,PL_T,P-" +
                    "\nŠiaulių regiono tinklas,Butas,G,10572558,0.9874,2022-05-31 00:00:00,0.0")
            };

            _httpMessageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(htmlResponse)
                .ReturnsAsync(csvResponse);

            var result = await _service.GetElectricityData();

            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.Equal("Šiaulių regiono tinklas", result[0].Tinklas);
        }
        [Fact]
        public async Task ShouldReturnEmpty_WhenNoButasData()
        {
            var htmlWithCsvLink = @"
                <html>
                    <body>
                        <a href='/file1.csv'>file1.csv</a>
                    </body>
                </html>";

            var htmlResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(htmlWithCsvLink)
            };

            var csvResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("TINKLAS,OBT_PAVADINIMAS,OBJ_GV_TIPAS,OBJ_NUMERIS,P+,PL_T,P-" +
                    "\nŠiaulių regiono tinklas,Namas,G,10572558,0.9874,2022-05-31 00:00:00,0.0")
            };

            _httpMessageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(htmlResponse)
                .ReturnsAsync(csvResponse);

            var result = await _service.GetElectricityData();

            Assert.Empty(result);
        }
    }
}
