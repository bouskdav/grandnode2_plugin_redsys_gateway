using Grand.Domain.Data;
using Grand.Domain.Orders;

namespace Payments.RedSys.Services
{
    public class RedSysPaymentService : IRedSysPaymentService
    {
        private readonly IRepository<Order> _orderRepository;

        public RedSysPaymentService(
            IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Task<Order> GetOrderByRedSysTransactionId(string transactionId)
        {
            var order = _orderRepository.Table.FirstOrDefault(i => i.UserFields.Any(f => f.Key == RedSysHelper.TRANSACTION_NUMBER && f.Value == transactionId));

            return Task.FromResult(order);
        }
    }
}
