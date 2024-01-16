using Grand.Domain.Configuration;

namespace Payments.RedSys
{
    public class RedSysPaymentSettings : ISettings
    {
        public bool UseSandbox { get; set; }

        public string MerchantId { get; set; }
        public string RedSysSecret { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }
        /// <summary>
        /// Additional fee
        /// </summary>
        public double AdditionalFee { get; set; }

        public bool PassProductNamesAndTotals { get; set; }

        public int DisplayOrder { get; set; }
    }
}
