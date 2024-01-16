using Grand.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.RedSys.Services
{
    public interface IRedSysPaymentService
    {
        Task<Order> GetOrderByRedSysTransactionId(string transactionId);
    }
}
