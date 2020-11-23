using System;
namespace JobsityChatroom.Common.Models
{
    public class StockInfo
    {
        public string Symbol { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
    }

    public class StockInfoResponse
    {
        public bool Success { get; set; }
        public StockInfo StockInfo { get; set; }
    }
}
