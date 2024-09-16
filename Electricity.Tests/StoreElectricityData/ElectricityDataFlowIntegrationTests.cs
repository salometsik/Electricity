using Electricity.Application.Contracts.Infrastructure;
using Electricity.Application.Contracts.Persistence;
using Electricity.Application.Features.ElectricityUsage.Commands;
using Electricity.Application.Models;
using Electricity.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Electricity.Tests.Integration
{
    public class ElectricityDataFlowIntegrationTests
    {
        readonly ServiceProvider _serviceProvider;
        readonly Mock<IRetrieveDataService> _mockRetrieveDataService;

        public ElectricityDataFlowIntegrationTests()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<ElectricityDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));

            _mockRetrieveDataService = new Mock<IRetrieveDataService>();

            serviceCollection.AddScoped(mockRetrieveDataService => _mockRetrieveDataService.Object);
            serviceCollection.AddScoped<IRepository, ElectricityRepository>();
            serviceCollection.AddScoped<StoreElectricityDataCommandHandler>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task FullFlow_SavesDataCorrectly_WhenAllServicesAreFunctional()
        {
            var mockCsvData = new List<ElectricityDataModel>
            {
                new ElectricityDataModel { Tinklas = "Region1", ElectricityConsumption = 100.5M, Date = DateTime.UtcNow.AddMonths(-1) },
                new ElectricityDataModel { Tinklas = "Region1", ElectricityConsumption = 200.3M, Date = DateTime.UtcNow.AddMonths(-1) },
                new ElectricityDataModel { Tinklas = "Region2", ElectricityConsumption = 50.7M, Date = DateTime.UtcNow.AddMonths(-1) }
            };

            _mockRetrieveDataService.Setup(s => s.GetElectricityData())
                .ReturnsAsync(mockCsvData);

            var handler = _serviceProvider.GetRequiredService<StoreElectricityDataCommandHandler>();

            CancellationToken cancellationToken = new CancellationToken();
            var result = await handler.Handle(new StoreElectricityDataCommand(), cancellationToken);

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                var savedData = await repository.GetAllAsync();

                Assert.NotNull(savedData);
                Assert.Equal(2, savedData.Count());

                var region1 = savedData.FirstOrDefault(d => d.Tinklas == "Region1");
                Assert.NotNull(region1);
                Assert.Equal(300.8M, region1.TotalConsumption);
                Assert.Equal(2, region1.DataCount);

                var region2 = savedData.FirstOrDefault(d => d.Tinklas == "Region2");
                Assert.NotNull(region2);
                Assert.Equal(50.7M, region2.TotalConsumption);
                Assert.Equal(1, region2.DataCount);
            }
        }
    }
}
