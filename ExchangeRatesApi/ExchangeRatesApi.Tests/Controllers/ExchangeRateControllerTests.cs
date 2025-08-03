using ExchangeRatesApi.Controllers;
using ExchangeRatesApi.DTOs;
using ExchangeRatesApi.Exceptions;
using ExchangeRatesApi.Features.ExchangeRate.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExchangeRatesApi.Tests.Controllers
{
    public class ExchangeRateControllerTests
    {
        private readonly Mock<IMediator> _mediator;

        public ExchangeRateControllerTests()
        {
            _mediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetCurrencyList_ReturnsOk_WithCurrencies()
        {
            var expectedList = new List<CurrencyDto> { new CurrencyDto { Code = "USD", Name = "dolar amerykański" } };

            _mediator
                .Setup(m => m.Send(It.IsAny<GetCurrencyList>(), default))
                .ReturnsAsync(expectedList);

            var controller = new ExchangeRateController(_mediator.Object);

            var result = await controller.GetCurrencyList("NBP");
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedList = Assert.IsType<List<CurrencyDto>>(okResult.Value);
            Assert.True(returnedList.Any());
            Assert.Equal(expectedList.First().Code, returnedList.First().Code);
            Assert.Equal(expectedList.First().Name, returnedList.First().Name);
        }

        [Fact]
        public async Task GetCurrencyList_ReturnsNoContent_WhenListIsEmpty()
        {
            _mediator
                .Setup(m => m.Send(It.IsAny<GetCurrencyList>(), default))
                .ReturnsAsync(new List<CurrencyDto>());

            var controller = new ExchangeRateController(_mediator.Object);

            var result = await controller.GetCurrencyList("NBP");
            
            Assert.NotNull(result);
            var noContentResult = Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetCurrencyList_ReturnsBadReduest_WhenThrowException()
        {
            var excMessage = "BadReq Exc";
            _mediator
                .Setup(m => m.Send(It.IsAny<GetCurrencyList>(), default))
                .ThrowsAsync(new BadRequestException(excMessage));

            var controller = new ExchangeRateController(_mediator.Object);

            var result = await controller.GetCurrencyList("NBP");
            Assert.NotNull(result);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var returnedExc = Assert.IsType<string>(badRequestResult.Value);
            Assert.Contains(excMessage, badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task GetExchangeRateApiList_ReturnsOk_WithData()
        {
            var expectedList = new List<ExchangeRateApiTypeDto> { new ExchangeRateApiTypeDto { Code = "NBP", Name = "Narodowy Bank Polski" } };

            _mediator
                .Setup(m => m.Send(It.IsAny<GetExchangeRateApiList>(), default))
                .ReturnsAsync(expectedList);

            var controller = new ExchangeRateController(_mediator.Object);

            var result = await controller.GetExchangeRateApiList();
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedList = Assert.IsType<List<ExchangeRateApiTypeDto>>(okResult.Value);
            Assert.True(returnedList.Any());
            Assert.Equal(expectedList.First().Code, returnedList.First().Code);
            Assert.Equal(expectedList.First().Name, returnedList.First().Name);
        }

        [Fact]
        public async Task GetExchangeRateApiList_ReturnsNoContent_WhenListIsEmpty()
        {
            _mediator
                .Setup(m => m.Send(It.IsAny<GetExchangeRateApiList>(), default))
                .ReturnsAsync(new List<ExchangeRateApiTypeDto>());

            var controller = new ExchangeRateController(_mediator.Object);

            var result = await controller.GetExchangeRateApiList();

            Assert.NotNull(result);
            var noContentResult = Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetExchangeRateApiList_ReturnsBadReduest_WhenThrowException()
        {
            var excMessage = "BadReq Exc";
            _mediator
                .Setup(m => m.Send(It.IsAny<GetExchangeRateApiList>(), default))
                .ThrowsAsync(new BadRequestException(excMessage));

            var controller = new ExchangeRateController(_mediator.Object);
            var result = await controller.GetExchangeRateApiList();
            Assert.NotNull(result);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var returnedExc = Assert.IsType<string>(badRequestResult.Value);
            Assert.Contains(excMessage,badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CalculateCurrencyRate_ReturnsOk_WithData()
        {
            var dateFrom = new DateTime(2025, 7, 14);
            var dateTo = new DateTime(2025, 7, 16);
            var sourceCurr = "USD";
            var targetCurr = "EUR";
            var minRate = 0.8556m; 
            var maxRate = 0.8602m; 
            var avgRate = 0.8572m;
            var rateList = new List<RateForDateDto>() 
                {
                    new RateForDateDto { Date = "2025-07-14", Rate = 0.8559m },
                    new RateForDateDto { Date = "2025-07-15", Rate = 0.8556m },
                    new RateForDateDto { Date = "2025-07-16", Rate = 0.8602m },
                };

            var expectedData = new CurrencyRateResponseDto()
                {
                    SourceCurrencyCode = sourceCurr,
                    TargetCurrencyCode = targetCurr,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    MinRate = minRate,
                    MaxRate = maxRate,
                    AvgRate = avgRate,
                    RateList = rateList
                };

            _mediator
                .Setup(m => m.Send(It.IsAny<CalculateCurrencyRate>(), default))
                .ReturnsAsync(expectedData);

            var controller = new ExchangeRateController(_mediator.Object);

            var result = await controller.CalculateCurrencyRate(
                                "NBP", sourceCurr, targetCurr, 
                                dateFrom, dateTo, default );

            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedDate = Assert.IsType<CurrencyRateResponseDto>(okResult.Value);
            Assert.NotNull(returnedDate);
            Assert.Equal(returnedDate.SourceCurrencyCode, returnedDate.SourceCurrencyCode);
            Assert.Equal(returnedDate.MinRate, returnedDate.MinRate);
            Assert.Equal(returnedDate.MaxRate, returnedDate.MaxRate);
            Assert.Equal(returnedDate.AvgRate, returnedDate.AvgRate);
            Assert.Equal(returnedDate.RateList.Count, returnedDate.RateList.Count);
        }

        [Fact]
        public async Task CalculateCurrencyRate_ReturnsNoContent_WhenListIsEmpty()
        {
            var dateFrom = new DateTime(2025, 7, 14);
            var dateTo = new DateTime(2025, 7, 16);
            var sourceCurr = "USD";
            var targetCurr = "EUR";

            _mediator
                .Setup(m => m.Send(It.IsAny<CalculateCurrencyRate>(), default))
                .ReturnsAsync((CurrencyRateResponseDto?)null);

            var controller = new ExchangeRateController(_mediator.Object);

            var result = await controller.CalculateCurrencyRate(
                                "NBP", sourceCurr, targetCurr,
                                dateFrom, dateTo, default);

            Assert.NotNull(result);
            var noContentResult = Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task CalculateCurrencyRate_ReturnsBadReduest_WhenThrowException()
        {
            var dateFrom = new DateTime(2025, 7, 14);
            var dateTo = new DateTime(2025, 7, 16);
            var sourceCurr = "USD";
            var targetCurr = "EUR";
            var excMessage = "BadReq Exc";
            _mediator
                .Setup(m => m.Send(It.IsAny<CalculateCurrencyRate>(), default))
                .ThrowsAsync(new BadRequestException(excMessage));

            var controller = new ExchangeRateController(_mediator.Object);
            var result = await controller.CalculateCurrencyRate(
                                "NBP", sourceCurr, targetCurr,
                                dateFrom, dateTo, default);
            Assert.NotNull(result);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var returnedExc = Assert.IsType<string>(badRequestResult.Value);
            Assert.Contains(excMessage, badRequestResult.Value.ToString());
        }
    }
}
