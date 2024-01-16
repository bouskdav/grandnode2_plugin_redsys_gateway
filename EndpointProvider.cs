using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Payments.RedSys
{
    public partial class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //PaymentInfo
            endpointRouteBuilder.MapControllerRoute("Plugin.RedSys",
                 "Plugins/PaymentRedSys/PaymentInfo",
                 new { controller = "PaymentRedSys", action = "PaymentInfo", area = "" }
            );

            // RedSys callback
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.RedSys.InternalCallback",
                 "Plugins/PaymentRedSys/InternalCallback",
                 new { controller = "PaymentRedSys", action = "InternalCallback" }
            );

            // RedSys payment status
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.RedSys.PaymentCallback",
                 "Plugins/PaymentRedSys/PaymentCallback",
                 new { controller = "PaymentRedSys", action = "PaymentCallback" }
            );

            //Cancel
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.RedSys.CancelOrder",
                 "Plugins/PaymentRedSys/CancelOrder",
                 new { controller = "PaymentRedSys", action = "CancelOrder" }
            );
        }
        public int Priority => 0;

    }
}
