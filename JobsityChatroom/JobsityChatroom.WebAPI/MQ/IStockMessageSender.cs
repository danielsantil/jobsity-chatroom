using System;
namespace JobsityChatroom.WebAPI.MQ
{
    public interface IStockMessageSender
    {
        void Send(string stockCode);
    }
}
