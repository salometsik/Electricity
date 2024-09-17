using Electricity.Application.Contracts.Infrastructure;
using Electricity.Application.Contracts.Persistence;
using Electricity.Application.Features.ElectricityUsage.Commands;
using Electricity.Application.Models;
using Electricity.Core.Entities;
using Moq;

namespace Electricity.Tests.StoreElectricityData
{
    public class StoreElectricityDataCommandHandlerTests
    {
        readonly Mock<IRetrieveDataService> _retrieveDataServiceMock;
        readonly Mock<IRepository> _repositoryMock;
        readonly StoreElectricityDataCommandHandler _handler;
        public StoreElectricityDataCommandHandlerTests()
        {
            _retrieveDataServiceMock = new Mock<IRetrieveDataService>();
            _repositoryMock = new Mock<IRepository>();

            _handler = new StoreElectricityDataCommandHandler(_retrieveDataServiceMock.Object, _repositoryMock.Object);
        }
        [Fact]
        public async Task ShouldReturnNoContent_WhenCsvFilesAreEmpty()
        {
            _retrieveDataServiceMock.Setup(s => s.GetElectricityData())
                .ReturnsAsync(new List<ElectricityDataModel>());

            CancellationToken cancellationToken = new CancellationToken();
            var result = await _handler.Handle(new StoreElectricityDataCommand(), cancellationToken);

            _retrieveDataServiceMock.Verify(s => s.GetElectricityData(), Times.Once);
            _repositoryMock.Verify(r => r.CreateRangeAsync(It.IsAny<List<ElectricityUsage>>()), Times.Never);
            Assert.True(result.Success);
            Assert.Equal(result.StatusCode, System.Net.HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task ShouldReturnException_WhenRepositoryFails()
        {
            var csvData = new List<ElectricityDataModel>
            {
                new ElectricityDataModel { Tinklas = "Region1", ElectricityConsumption = 100.5M },
                new ElectricityDataModel { Tinklas = "Region1", ElectricityConsumption = 200.3M },
                new ElectricityDataModel { Tinklas = "Region2", ElectricityConsumption = 50.7M }
            };
            _retrieveDataServiceMock.Setup(s => s.GetElectricityData())
                .ReturnsAsync(csvData);

            _repositoryMock.Setup(r => r.CreateRangeAsync(It.IsAny<List<ElectricityUsage>>()))
                .ReturnsAsync(false);

            CancellationToken cancellationToken = new CancellationToken();
            var result = await _handler.Handle(new StoreElectricityDataCommand(), cancellationToken);

            _retrieveDataServiceMock.Verify(s => s.GetElectricityData(), Times.Once);
            _repositoryMock.Verify(r => r.CreateRangeAsync(It.IsAny<List<ElectricityUsage>>()), Times.Once);
            Assert.False(result.Success);
            Assert.Equal(result.StatusCode, System.Net.HttpStatusCode.InternalServerError);
        }
        [Fact]
        public async Task ShouldSuccessfullySaveData_WhenRepositorySucceeds()
        {
            var csvData = new List<ElectricityDataModel>
            {
                new ElectricityDataModel { Tinklas = "Region1", ElectricityConsumption = 100.5M },
                new ElectricityDataModel { Tinklas = "Region1", ElectricityConsumption = 200.3M },
                new ElectricityDataModel { Tinklas = "Region2", ElectricityConsumption = 50.7M }
            };
            _retrieveDataServiceMock.Setup(s => s.GetElectricityData())
                .ReturnsAsync(csvData);

            _repositoryMock.Setup(r => r.CreateRangeAsync(It.IsAny<List<ElectricityUsage>>()))
                .ReturnsAsync(true);

            CancellationToken cancellationToken = new CancellationToken();
            var result = await _handler.Handle(new StoreElectricityDataCommand(), cancellationToken);

            _retrieveDataServiceMock.Verify(s => s.GetElectricityData(), Times.Once);
            _repositoryMock.Verify(r => r.CreateRangeAsync(It.Is<List<ElectricityUsage>>(list => list.Count == 2)), Times.Once);
            Assert.True(result.Success);
            Assert.Equal(result.StatusCode, System.Net.HttpStatusCode.OK);
        }
    }
}
