using Mongo.Services.OrderAPI.Models;

namespace Mongo.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public Task<bool> AddOrder(OrderHeader orderHeader)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            throw new NotImplementedException();
        }
    }
}
