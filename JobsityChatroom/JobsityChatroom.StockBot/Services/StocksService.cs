using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using JobsityChatroom.Common.Models;

namespace JobsityChatroom.StockBot.Services
{
    public class StocksService
    {
        public async Task<StockInfoResponse> GetStockInfo(string stockCode)
        {
            var response = new StockInfoResponse();
            try
            {
                var stockUrl = $"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";

                using var client = new HttpClient();
                var csvStream = await client.GetStreamAsync(stockUrl);

                response.StockInfo = GetStockInfo(csvStream);
                response.Success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error getting stock info {0}: {1}", stockCode, e.Message);
            }

            return response;
        }

        private StockInfo GetStockInfo(Stream csvStream)
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<StockInfo>();
            return records.FirstOrDefault();
        }
    }
}
