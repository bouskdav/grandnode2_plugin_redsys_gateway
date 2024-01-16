using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Payments.RedSys.Models
{
    public class ConfigurationModel : BaseModel
    {
        public string StoreScope { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.RedSys.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.RedSys.Fields.MerchantId")]
        public string MerchantId { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.RedSys.Fields.RedSysSecret")]
        public string RedSysSecret { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.RedSys.Fields.AdditionalFee")]
        public double AdditionalFee { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.RedSys.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.RedSys.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.RedSys.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }



    }
}