using System;
namespace JobsityChatroom.WebAPI.MQ
{
    public interface IStockMessageConsumer
    {
        void Consume();
    }
}
