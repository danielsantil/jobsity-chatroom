using System;
using System.Threading.Tasks;
using JobsityChatroom.StockBot.Services;
using Xunit;

namespace JobsityChatroom.UnitTest
{
    public class StockServiceTest
    {
        private readonly StocksService stockService;

        public StockServiceTest()
        {
            stockService = new StocksService();
        }

        [Fact]
        public async Task ValidStockInfo()
        {
            var response = await stockService.GetStockInfo("aapl.us");
            Assert.True(response.Success);
            Assert.Equal("aapl.us", response.StockInfo.Symbol.ToLower());
        }

        [Fact]
        public async Task StockInfo_Error()
        {
            var response = await stockService.GetStockInfo("hello");
            Assert.False(response.Success);
        }
    }
}
