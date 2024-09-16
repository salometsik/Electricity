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
    public class RetrieveDataService(ILogger<RetrieveDataService> _logger, HttpClient _httpClient) : IRetrieveDataService
    {
        public async Task<List<ElectricityDataModel>> GetElectricityData()
        {
            var csvUrls = await GetCsvFileLinksFromPage();
            if (csvUrls.Count == 0)
                return new List<ElectricityDataModel>();

            var csvContents = await DownloadCsvFiles(csvUrls);
            if (csvContents.Count == 0)
                return new List<ElectricityDataModel>();

            return await FilterCsvData(csvContents);
        }

        async Task<List<ElectricityDataModel>> FilterCsvData(List<string> csvContents)
        {
            var allFilteredData = new List<ElectricityDataModel>();
            try
            {
                foreach (var csvData in csvContents)
                {
                    var records = ParseCsv(csvData);
                    if (records != null)
                        allFilteredData.AddRange(records
                        .Where(d => d.Type == "Butas")
                        .ToList());
                }
                return FilterByDate(allFilteredData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get electricity data");
                return new List<ElectricityDataModel>();
            }
        }
        List<ElectricityDataModel> ParseCsv(string csvData)
        {
            try
            {
                using var reader = new StringReader(csvData);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
                csv.Context.RegisterClassMap<ElectricityDataMap>();
                return csv.GetRecords<ElectricityDataModel>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing CSV data");
                return null;
            }
        }
        List<ElectricityDataModel> FilterByDate(List<ElectricityDataModel> allFilteredData)
        {
            if (allFilteredData.Count == 0)
                return allFilteredData;

                return allFilteredData
                    .OrderByDescending(x => x.Date)
                    .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 1))
                    .Take(2)
                    .SelectMany(group => group)
                    .ToList();
        }
        async Task<List<string>> DownloadCsvFiles(List<string> csvUrls)
        {
            try
            {
                var csvContents = new List<string>();
                foreach (var url in csvUrls)
                {
                    var csvData = await _httpClient.GetStringAsync(url);
                    csvContents.Add(csvData);
                }
                return csvContents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get csv contents");
                return new List<string>();
            }
        }
        async Task<List<string>> GetCsvFileLinksFromPage()
        {
            var baseUrl = "https://data.gov.lt";
            try
            {
                var response = await _httpClient.GetAsync(baseUrl +
                    "/dataset/siame-duomenu-rinkinyje-pateikiami-atsitiktinai-parinktu-1000-buitiniu-vartotoju-automatizuotos-apskaitos-elektriniu-valandiniai-duomenys");

                if (!response.IsSuccessStatusCode)
                    return new List<string>();

                var htmlContent = await response.Content.ReadAsStringAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                var csvLinks = htmlDoc.DocumentNode
                                .SelectNodes("//a[contains(@href, '.csv')]")
                                ?.Select(node => node.GetAttributeValue("href", ""))
                                .Where(link => !string.IsNullOrEmpty(link))
                                .Select(link => baseUrl + link)
                                .ToList();

                if (csvLinks == null || (csvLinks != null && csvLinks.Count == 0))
                {
                    _logger.LogError("Could not get csv links");
                    return new List<string>();
                }

                return csvLinks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting csv file links");
                return new List<string>();
            }
        }
    }
}
