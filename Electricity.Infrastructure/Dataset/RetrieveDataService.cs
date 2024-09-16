using CsvHelper;
using CsvHelper.Configuration;
using Electricity.Application.Contracts.Infrastructure;
using Electricity.Application.Models;
using Electricity.Infrastructure.Helpers;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Electricity.Infrastructure.Dataset
{
    public class RetrieveDataService(ILogger<RetrieveDataService> _logger) : IRetrieveDataService
    {
        public async Task<List<ElectricityDataModel>> GetElectricityData()
        {

            var csvUrls = await GetCsvFileLinksFromPage();
            if (csvUrls.Count == 0)
                return [];

            var csvContents = await DownloadCsvFiles(csvUrls);
            if (csvContents.Count == 0)
                return [];

            return await FilterCsvData(csvContents);
        }

        async Task<List<ElectricityDataModel>> FilterCsvData(List<string> csvContents)
        {
            var allFilteredData = new List<ElectricityDataModel>();
            try
            {
                foreach (var csvData in csvContents)
                {
                    using var reader = new StringReader(csvData);
                    using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
                    csv.Context.RegisterClassMap<ElectricityDataMap>();

                    var records = csv.GetRecords<ElectricityDataModel>().ToList();

                    allFilteredData.AddRange(records
                        .Where(d => d.Type == "Butas")
                        .ToList());
                }
                if (allFilteredData.Count == 0)
                    return [];

                if (allFilteredData.Any(x => x.Date >= DateTime.UtcNow.AddMonths(-2)))
                    allFilteredData = allFilteredData.Where(x => x.Date >= DateTime.UtcNow.AddMonths(-2)).ToList();
                else
                {
                    var grouped = allFilteredData
                        .OrderByDescending(x => x.Date)
                        .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                        .Take(2)
                        .ToList();
                    allFilteredData.AddRange(grouped
                                    .SelectMany(group => group.Select(i => new ElectricityDataModel
                                    {
                                        Tinklas = i.Tinklas,
                                        ElectricityConsumption = i.ElectricityConsumption,
                                        Date = i.Date
                                    })));
                }

                return allFilteredData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get electricity data");
                return [];
            }
        }

        async Task<List<string>> DownloadCsvFiles(List<string> csvUrls)
        {
            try
            {
                using var client = new HttpClient();
                var csvContents = new List<string>();
                foreach (var url in csvUrls)
                {
                    var csvData = await client.GetStringAsync(url);
                    csvContents.Add(csvData);
                }
                return csvContents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get csv contents");
                return [];
            }
        }

        async Task<List<string>> GetCsvFileLinksFromPage()
        {
            var baseUrl = "https://data.gov.lt";
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(baseUrl +
                    "/dataset/siame-duomenu-rinkinyje-pateikiami-atsitiktinai-parinktu-1000-buitiniu-vartotoju-automatizuotos-apskaitos-elektriniu-valandiniai-duomenys");

                if (!response.IsSuccessStatusCode)
                    return [];

                var htmlContent = await response.Content.ReadAsStringAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                var csvLinks = htmlDoc.DocumentNode
                                .SelectNodes("//a[contains(@href, '.csv')]")
                                ?.Select(node => node.GetAttributeValue("href", ""))
                                .Where(link => !string.IsNullOrEmpty(link))
                                .ToList();

                if (csvLinks == null)
                {
                    _logger.LogError("Could not get csv links");
                    return [];
                }

                var absoluteCsvLinks = csvLinks
                                    .Select(x => baseUrl + x)
                                    .ToList();
                return absoluteCsvLinks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting csv file links");
                return [];
            }
        }
    }
}
