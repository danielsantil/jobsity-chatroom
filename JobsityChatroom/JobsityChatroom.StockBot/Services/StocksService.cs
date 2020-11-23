using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;

namespace JobsityChatroom.StockBot.Services
{
    public class StocksService
    {
        public async Task<StockInfo> GetStockInfo(string stockCode)
        {
            StockInfo response = null;
            try
            {
                var stockUrl = $"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";

                using var client = new HttpClient();
                var csvStream = await client.GetStreamAsync(stockUrl);

                response = GetStockInfo(csvStream);
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

    public class StockInfo
    {
        public string Symbol { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
    }
}
