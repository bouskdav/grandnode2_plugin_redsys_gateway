using RedSysNET.Domain.Enums;
using Grand.Domain.Payments;

namespace Payments.RedSys
{
    /// <summary>
    /// Represents paypal helper
    /// </summary>
    public static class RedSysHelper
    {
        public const string TRANSACTION_NUMBER = "comgate_transaction_number";

        public static string RedSysUrl => "https://payments.comgate.cz/v1.0/";

        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="paymentStatus">RedSys payment status</param>
        /// <param name="pendingReason">RedSys pending reason</param>
        /// <returns>Payment status</returns>
        public static PaymentStatus GetPaymentStatus(string paymentStatus, string pendingReason)
        {
            var result = PaymentStatus.Pending;

            if (paymentStatus == null)
                paymentStatus = string.Empty;

            if (pendingReason == null)
                pendingReason = string.Empty;

            switch (paymentStatus.ToLowerInvariant())
            {
                case "pending":
                    switch (pendingReason.ToLowerInvariant())
                    {
                        case "authorization":
                            result = PaymentStatus.Authorized;
                            break;
                        default:
                            result = PaymentStatus.Pending;
                            break;
                    }
                    break;
                case "processed":
                case "completed":
                case "canceled_reversal":
                    result = PaymentStatus.Paid;
                    break;
                case "denied":
                case "expired":
                case "failed":
                case "voided":
                    result = PaymentStatus.Voided;
                    break;
                case "refunded":
                case "reversed":
                    result = PaymentStatus.Refunded;
                    break;
                default:
                    break;
            }
            return result;
        }
        
        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="paymentStatus">RedSys payment status</param>
        /// <param name="pendingReason">RedSys pending reason</param>
        /// <returns>Payment status</returns>
        public static PaymentStatus GetPaymentStatus(PaymentState paymentStatus)
        {
            return paymentStatus switch {
                PaymentState.PENDING => PaymentStatus.Pending,
                PaymentState.PAID => PaymentStatus.Paid,
                PaymentState.CANCELLED => PaymentStatus.Voided,
                PaymentState.AUTHORIZED => PaymentStatus.Authorized,
                _ => PaymentStatus.Pending,
            };
        }
    }
}

