using ExchangeRatesApi.Controllers;
using ExchangeRatesApi.Data.Models;
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
            Assert.Equal(excMessage, badRequestResult.Value);
        }
    }
}
