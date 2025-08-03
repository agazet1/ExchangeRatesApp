using ExchangeRatesApi.Data.Models;
using ExchangeRatesApi.DTOs;
using ExchangeRatesApi.Services;
using ExchangeRatesApi.Services.Interfaces;
using Moq;

namespace ExchangeRatesApi.Tests.Services
{
    public class ExchangeRateServiceTests
    {
        private readonly Mock<IExchangeRateApiAdapter> _apiAdapter;
        private readonly Mock<IAppConfigurationService> _appConfigService;
        private readonly ExchangeRateService _exchangeRateService;

        private static readonly List<Currency> _currencyList = new List<Currency>()
                {
                    new Currency() { Code = "CLP", Name="peso chillijskie" },
                    new Currency() { Code = "USD", Name="dolar amerykański" },
                    new Currency() { Code = "CHF", Name="frank szwajcarski" },
                };

        public ExchangeRateServiceTests()
        {
            _apiAdapter = new Mock<IExchangeRateApiAdapter>();
            _appConfigService = new Mock<IAppConfigurationService>();
            _exchangeRateService = new ExchangeRateService(_apiAdapter.Object, _appConfigService.Object);
        }

        [Theory]
        [InlineData(CurrencyOrderEnum.CODE_ASC, "CHF", "USD")]
        [InlineData(CurrencyOrderEnum.CODE_DESC, "USD", "CHF")]
        [InlineData(CurrencyOrderEnum.NAME_ASC, "USD", "CLP")]
        [InlineData(CurrencyOrderEnum.NAME_DESC, "CLP", "USD")]
        public async Task GetCurrencyList_ReturnsOrderedList(CurrencyOrderEnum ord, string firstCode, string lastCode)
        {
            var apiType = new ExchangeRateApiType() { Code = "NBP" };

            _apiAdapter
                .Setup(x => x.GetCurrencyList(It.IsAny<ExchangeRateApiType>(), default))
                .ReturnsAsync(_currencyList);

            _appConfigService
                .Setup(x => x.GetCurrencyOrder())
                .Returns(ord);

            var result = await _exchangeRateService.GetCurrencyList(apiType, default);
            Assert.NotNull(result);
            Assert.Equal(_currencyList.Count, result.Count);
            Assert.Equal(firstCode, result.First().Code);
            Assert.Equal(lastCode, result.Last().Code);
        }

        [Fact]
        public async Task GetApiType_ReturnsApi_WhenApiExists()
        {
            var apiList = new List<ExchangeRateApiType>() { new ExchangeRateApiType() { Code = "NBP", Name = "NBP Api", Url = "https://api.nbp.pl" } };
            _appConfigService
                .Setup(x=> x.GetExchangeRateApiList())
                .Returns(apiList);

            var result = _exchangeRateService.GetApiType("NBP");
            Assert.NotNull(result);
            Assert.Equal(apiList.First().Code, result.Code);
            Assert.Equal(apiList.First().Name, result.Name);
            Assert.Equal(apiList.First().Url, result.Url);
        }

        [Fact]
        public async Task GetApiType_ThrowsExc_WhenApiCodeEmpty()
        {
            var apiList = new List<ExchangeRateApiType>() { new ExchangeRateApiType() { Code = "NBP", Name = "NBP Api", Url = "https://api.nbp.pl" } };
            _appConfigService
                .Setup(x => x.GetExchangeRateApiList())
                .Returns(apiList);

            var exc = Assert.ThrowsAsync<Exception>(async () => _exchangeRateService.GetApiType("")); 
            Assert.NotNull(exc);
            Assert.NotNull(exc.Result);
            Assert.NotEmpty(exc.Result.Message);
        }

        [Fact]
        public async Task GetApiType_ThrowsExc_WhenApiNotFound()
        {
            var apiList = new List<ExchangeRateApiType>() { new ExchangeRateApiType() { Code = "NBP", Name = "NBP Api", Url = "https://api.nbp.pl" } };
            _appConfigService
                .Setup(x => x.GetExchangeRateApiList())
                .Returns(apiList);

            var exc = Assert.ThrowsAsync<Exception>(async () => _exchangeRateService.GetApiType("ABC"));
            Assert.NotNull(exc);
            Assert.NotNull(exc.Result);
            Assert.NotEmpty(exc.Result.Message);
        }

        [Fact]
        public async Task GetApiType_ThrowsExc_WhenApiNoUrl()
        {
            var apiList = new List<ExchangeRateApiType>() { new ExchangeRateApiType() { Code = "NBP", Name = "NBP Api", Url = "" } };
            _appConfigService
                .Setup(x => x.GetExchangeRateApiList())
                .Returns(apiList);

            var exc = Assert.ThrowsAsync<Exception>(async () => _exchangeRateService.GetApiType("NBP"));
            Assert.NotNull(exc);
            Assert.NotNull(exc.Result);
            Assert.NotEmpty(exc.Result.Message);
        }

        [Fact]
        public async Task CalculateCurrencyRate_ReturnsCorrectData()
        {
            var api = new ExchangeRateApiType() { Code = "NBP", Name = "NBP api", Url = "http://api.nbp.pl" };
            var dateFrom = new DateTime(2025, 7, 14);
            var dateTo = new DateTime(2025, 7, 16);
            var sourceCurr = "USD";
            var targetCurr = "EUR";
            var rateUsdList = new List<RateForDate>()
                {
                    new RateForDate { Date = new DateTime(2025,7,14), Rate = 3.6475m },
                    new RateForDate { Date = new DateTime(2025,7,15), Rate = 3.6396m },
                    new RateForDate { Date = new DateTime(2025,7,16), Rate = 3.6631m },
                };
            var rateEurList = new List<RateForDate>()
                {
                    new RateForDate { Date = new DateTime(2025,7,14), Rate = 4.2614m },
                    new RateForDate { Date = new DateTime(2025,7,15), Rate = 4.2538m },
                    new RateForDate { Date = new DateTime(2025,7,16), Rate = 4.2585m },
                };

            var apiList = new List<ExchangeRateApiType>() { new ExchangeRateApiType() { Code = "NBP", Name = "NBP Api", Url = "https://api.nbp.pl" } };
            _appConfigService
                .Setup(x => x.GetDateFormat())
                .Returns("yyyy-MM-dd");

            _apiAdapter
                .Setup(x => x.GetExchangeRatesForDatesList(It.IsAny<ExchangeRateApiType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), default))
                .ReturnsAsync((ExchangeRateApiType api, string s, DateTime dF, DateTime dT, CancellationToken ct) =>
                {
                    if (s == "USD") return rateUsdList;
                    if (s == "EUR") return rateEurList;
                    return new List<RateForDate>();
                });

            var result = await _exchangeRateService.CalculateCurrencyRate(api, sourceCurr, targetCurr, dateFrom, dateTo, default);
            Assert.NotNull(result);
            Assert.NotNull(result.RateList);
            Assert.Equal(3, result.RateList.Count);
            Assert.True(result.MaxRate > 0);
            Assert.True(result.MinRate > 0);
            Assert.True(result.AvgRate > 0);
        }

        [Fact]
        public async Task CalculateCurrencyRate_ReturnsShortRateList_WhenMissingDates()
        {
            var api = new ExchangeRateApiType() { Code = "NBP", Name = "NBP api", Url = "http://api.nbp.pl" };
            var dateFrom = new DateTime(2025, 7, 14);
            var dateTo = new DateTime(2025, 7, 16);
            var sourceCurr = "USD";
            var targetCurr = "EUR";
            var rateUsdList = new List<RateForDate>()
                {
                    new RateForDate { Date = new DateTime(2025,7,14), Rate = 3.6475m },
                    new RateForDate { Date = new DateTime(2025,7,15), Rate = 3.6396m },
                };
            var rateEurList = new List<RateForDate>()
                {
                    new RateForDate { Date = new DateTime(2025,7,14), Rate = 4.2614m },
                    new RateForDate { Date = new DateTime(2025,7,16), Rate = 4.2585m },
                };

            var apiList = new List<ExchangeRateApiType>() { new ExchangeRateApiType() { Code = "NBP", Name = "NBP Api", Url = "https://api.nbp.pl" } };
            _appConfigService
                .Setup(x => x.GetDateFormat())
                .Returns("yyyy-MM-dd");

            _apiAdapter
                .Setup(x => x.GetExchangeRatesForDatesList(It.IsAny<ExchangeRateApiType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), default))
                .ReturnsAsync((ExchangeRateApiType api, string s, DateTime dF, DateTime dT, CancellationToken ct) =>
                {
                    if (s == "USD") return rateUsdList;
                    if (s == "EUR") return rateEurList;
                    return new List<RateForDate>();
                });

            var result = await _exchangeRateService.CalculateCurrencyRate(api, sourceCurr, targetCurr, dateFrom, dateTo, default);
            Assert.NotNull(result);
            Assert.NotNull(result.RateList);
            Assert.Equal(1, result.RateList.Count);
            Assert.True(result.MaxRate > 0);
            Assert.True(result.MinRate > 0);
            Assert.True(result.AvgRate > 0);
        }

        [Theory]
        [InlineData(null, "EUR")]
        [InlineData("USD", null)]
        [InlineData(null, null)]
        public async Task CalculateCurrencyRate_ReturnsNull_WhenNoRates(string? usdRates, string? euroRates)
        {
            var api = new ExchangeRateApiType() { Code = "NBP", Name = "NBP api", Url = "http://api.nbp.pl" };
            var dateFrom = new DateTime(2025, 7, 14);
            var dateTo = new DateTime(2025, 7, 16);
            var sourceCurr = "USD";
            var targetCurr = "EUR";
            var rateUsdList = string.IsNullOrEmpty(usdRates) ? new List<RateForDate>() : new List<RateForDate>()
                {
                    new RateForDate { Date = new DateTime(2025,7,14), Rate = 3.6475m },
                    new RateForDate { Date = new DateTime(2025,7,15), Rate = 3.6396m },
                    new RateForDate { Date = new DateTime(2025,7,16), Rate = 3.6631m },
                };
            var rateEurList = string.IsNullOrEmpty(euroRates) ? new List<RateForDate>() : new List<RateForDate>()
                {
                    new RateForDate { Date = new DateTime(2025,7,14), Rate = 4.2614m },
                    new RateForDate { Date = new DateTime(2025,7,15), Rate = 4.2538m },
                    new RateForDate { Date = new DateTime(2025,7,16), Rate = 4.2585m },
                };

            var apiList = new List<ExchangeRateApiType>() { new ExchangeRateApiType() { Code = "NBP", Name = "NBP Api", Url = "https://api.nbp.pl" } };
            _appConfigService
                .Setup(x => x.GetDateFormat())
                .Returns("yyyy-MM-dd");

            _apiAdapter
                .Setup(x => x.GetExchangeRatesForDatesList(It.IsAny<ExchangeRateApiType>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), default))
                .ReturnsAsync((ExchangeRateApiType api, string s, DateTime dF, DateTime dT, CancellationToken ct) =>
                {
                    if (s == "USD") return rateUsdList;
                    if (s == "EUR") return rateEurList;
                    return new List<RateForDate>();
                });

            var result = await _exchangeRateService.CalculateCurrencyRate(api, sourceCurr, targetCurr, dateFrom, dateTo, default);
            Assert.Null(result);
        }
    }
}
