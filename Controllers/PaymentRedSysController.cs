using Grand.Business.Core.Commands.Checkout.Orders;
using Grand.Business.Core.Extensions;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Payments;
using Grand.Business.Core.Queries.Checkout.Orders;
using Grand.Business.Core.Interfaces.Common.Logging;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Infrastructure;
using Grand.SharedKernel;
using Grand.Web.Common.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Payments.RedSys.Services;
using System.Globalization;
using RedSysNET.Domain.Models.Responses;
using Grand.Business.Core.Interfaces.Common.Directory;
using Newtonsoft.Json;
using RedSysNET.Domain.Enums;

namespace Payments.RedSys.Controllers
{

    public class PaymentRedSysController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IPaymentTransactionService _paymentTransactionService;

        private readonly PaymentSettings _paymentSettings;
        private readonly IUserFieldService _userFieldService;
        private readonly IRedSysPaymentService _comgatePaymentService;
        private readonly RedSysPaymentSettings _comgatePaymentSettings;

        public PaymentRedSysController(
            IWorkContext workContext,
            IPaymentService paymentService,
            IOrderService orderService,
            ILogger logger,
            IMediator mediator,
            IPaymentTransactionService paymentTransactionService,
            RedSysPaymentSettings comgatePaymentSettings,
            PaymentSettings paymentSettings,
            IUserFieldService userFieldService,
            IRedSysPaymentService comgatePaymentService)
        {
            _workContext = workContext;
            _paymentService = paymentService;
            _orderService = orderService;
            _logger = logger;
            _mediator = mediator;
            _paymentTransactionService = paymentTransactionService;
            _comgatePaymentSettings = comgatePaymentSettings;
            _paymentSettings = paymentSettings;
            _userFieldService = userFieldService;
            _comgatePaymentService = comgatePaymentService;
        }


        private string QueryString(string name)
        {
            if (StringValues.IsNullOrEmpty(HttpContext.Request.Query[name]))
                return default;

            return HttpContext.Request.Query[name].ToString();
        }

        //public async Task<IActionResult> PDTHandler()
        //{
        //    var tx = QueryString("tx");

        //    if (_paymentService.LoadPaymentMethodBySystemName("Payments.RedSys") is not RedSysPaymentProvider processor ||
        //        !processor.IsPaymentMethodActive(_paymentSettings))
        //        throw new GrandException("RedSys  module cannot be loaded");

        //    (var status, var values, var _) = await _comgateHttpClient.GetPdtDetails(tx);

        //    if (status)
        //    {
        //        values.TryGetValue("custom", out var orderNumber);
        //        Guid orderNumberGuid = Guid.Empty;
        //        try
        //        {
        //            orderNumberGuid = new Guid(orderNumber);
        //        }
        //        catch { }
        //        Order order = await _orderService.GetOrderByGuid(orderNumberGuid);
        //        if (order != null)
        //        {
        //            var paymentTransaction = await _paymentTransactionService.GetOrderByGuid(orderNumberGuid);

        //            double mc_gross = 0;
        //            try
        //            {
        //                mc_gross = double.Parse(values["mc_gross"], new CultureInfo("en-US"));
        //            }
        //            catch (Exception exc)
        //            {
        //                _ = _logger.Error("RedSys PDT. Error getting mc_gross", exc);
        //            }

        //            values.TryGetValue("payer_status", out var payer_status);
        //            values.TryGetValue("payment_status", out var payment_status);
        //            values.TryGetValue("pending_reason", out var pending_reason);
        //            values.TryGetValue("mc_currency", out var mc_currency);
        //            values.TryGetValue("txn_id", out var txn_id);
        //            values.TryGetValue("payment_type", out var payment_type);
        //            values.TryGetValue("payer_id", out var payer_id);
        //            values.TryGetValue("receiver_id", out var receiver_id);
        //            values.TryGetValue("invoice", out var invoice);
        //            values.TryGetValue("payment_fee", out var payment_fee);

        //            var sb = new StringBuilder();
        //            sb.AppendLine("Paypal PDT:");
        //            sb.AppendLine("mc_gross: " + mc_gross);
        //            sb.AppendLine("Payer status: " + payer_status);
        //            sb.AppendLine("Payment status: " + payment_status);
        //            sb.AppendLine("Pending reason: " + pending_reason);
        //            sb.AppendLine("mc_currency: " + mc_currency);
        //            sb.AppendLine("txn_id: " + txn_id);
        //            sb.AppendLine("payment_type: " + payment_type);
        //            sb.AppendLine("payer_id: " + payer_id);
        //            sb.AppendLine("receiver_id: " + receiver_id);
        //            sb.AppendLine("invoice: " + invoice);
        //            sb.AppendLine("payment_fee: " + payment_fee);

        //            var newPaymentStatus = RedSysHelper.GetPaymentStatus(payment_status, pending_reason);
        //            sb.AppendLine("New payment status: " + newPaymentStatus);

        //            //order note
        //            await _orderService.InsertOrderNote(new OrderNote {
        //                Note = sb.ToString(),
        //                DisplayToCustomer = false,
        //                CreatedOnUtc = DateTime.UtcNow,
        //                OrderId = order.Id,
        //            });

        //            //load settings for a chosen store scope
        //            //validate order total
        //            if (_comgatePaymentSettings.PdtValidateOrderTotal && !Math.Round(mc_gross, 2).Equals(Math.Round(order.OrderTotal * order.CurrencyRate, 2)))
        //            {
        //                string errorStr = string.Format("RedSys PDT. Returned order total {0} doesn't equal order total {1}. Order# {2}.", mc_gross, order.OrderTotal * order.CurrencyRate, order.OrderNumber);
        //                _ = _logger.Error(errorStr);

        //                //order note
        //                await _orderService.InsertOrderNote(new OrderNote {
        //                    Note = errorStr,
        //                    OrderId = order.Id,
        //                    DisplayToCustomer = false,
        //                    CreatedOnUtc = DateTime.UtcNow
        //                });

        //                return RedirectToAction("Index", "Home", new { area = "" });
        //            }

        //            //mark order as paid
        //            if (newPaymentStatus == PaymentStatus.Paid)
        //            {
        //                if (await _mediator.Send(new CanMarkPaymentTransactionAsPaidQuery() { PaymentTransaction = paymentTransaction }))
        //                {
        //                    paymentTransaction.AuthorizationTransactionId = txn_id;
        //                    await _paymentTransactionService.UpdatePaymentTransaction(paymentTransaction);
        //                    await _mediator.Send(new MarkAsPaidCommand() { PaymentTransaction = paymentTransaction });
        //                }
        //            }
        //        }

        //        return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        //    }
        //    else
        //    {
        //        var custom = QueryString("custom");
        //        Guid orderNumberGuid = Guid.Empty;
        //        try
        //        {
        //            orderNumberGuid = new Guid(custom);
        //        }
        //        catch { }
        //        Order order = await _orderService.GetOrderByGuid(orderNumberGuid);
        //        if (order != null)
        //        {                    
        //            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        //        }
        //        else
        //            return RedirectToAction("Index", "Home", new { area = "" });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> InternalCallback(GetPaymentStatusResponse paymentStatusResponse)
        {
            if (_paymentService.LoadPaymentMethodBySystemName("Payments.RedSys") is not RedSysPaymentProvider processor ||
                !processor.IsPaymentMethodActive(_paymentSettings))
                throw new GrandException("RedSys module cannot be loaded");

            string merchantId = _comgatePaymentSettings.MerchantId;
            string secret = _comgatePaymentSettings.RedSysSecret;

            Order order = await _comgatePaymentService.GetOrderByRedSysTransactionId(paymentStatusResponse.transId);

            // check for order not found
            if (order == null)
            {
                throw new GrandException($"Order not found (transactionId {paymentStatusResponse.transId}).");
            }

            // check secret validity
            if (secret != paymentStatusResponse.secret)
            {
                string message = $"#{paymentStatusResponse.refId} ({paymentStatusResponse.transId}) ({DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}) - MESSAGE: Při návratu z platební brány byl uveden špatný bezepčnostní kód. Pozor, může se jednat o útok! IP: {Request.HttpContext.Connection.RemoteIpAddress}";

                //_objednavkyDataProvider.SetPaymentError(objednavka.ID, paymentStatusResponse.transId, message);
                // TODO: set error

                // Add note
                //order note
                await _orderService.InsertOrderNote(new OrderNote {
                    Note = message,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id,
                });

                throw new GrandException("Wrong secret code.");
            }

            // get form data
            var formDictionary = Request.Form.Keys.ToDictionary(i => i, i => Request.Form[i]);

            //order note
            await _orderService.InsertOrderNote(new OrderNote {
                Note = $"Received response from RedSys: {JsonConvert.SerializeObject(formDictionary)}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = order.Id,
            });
            //order note
            await _orderService.InsertOrderNote(new OrderNote {
                Note = $"RedSys response parsed as: {JsonConvert.SerializeObject(paymentStatusResponse)}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = order.Id,
            });

            var newPaymentStatus = RedSysHelper.GetPaymentStatus(paymentStatusResponse.Status);

            var paymentTransaction = await _paymentTransactionService.GetOrderByGuid(order.OrderGuid);

            switch (newPaymentStatus)
            {
                case PaymentStatus.Pending:
                    break;
                case PaymentStatus.Authorized:
                    if (await _mediator.Send(new CanMarkPaymentTransactionAsAuthorizedQuery() { PaymentTransaction = paymentTransaction }))
                    {
                        await _mediator.Send(new MarkAsAuthorizedCommand() { PaymentTransaction = paymentTransaction });
                    }
                    break;
                case PaymentStatus.Paid:
                    if (await _mediator.Send(new CanMarkPaymentTransactionAsPaidQuery() { PaymentTransaction = paymentTransaction }))
                    {
                        paymentTransaction.AuthorizationTransactionId = paymentStatusResponse.transId;
                        await _paymentTransactionService.UpdatePaymentTransaction(paymentTransaction);

                        await _mediator.Send(new MarkAsPaidCommand() { PaymentTransaction = paymentTransaction });
                    }
                    break;
                case PaymentStatus.Voided:
                    if (await _mediator.Send(new CanVoidOfflineQuery() { PaymentTransaction = paymentTransaction }))
                    {
                        await _mediator.Send(new VoidOfflineCommand() { PaymentTransaction = paymentTransaction });
                    }
                    break;
                default:
                    break;
            };

            return Ok();
        }

        public async Task<IActionResult> PaymentCallback(int id, string transactionId, PaymentState result)
        {
            Order order = await _comgatePaymentService.GetOrderByRedSysTransactionId(transactionId);

            if (order != null)
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            else
                return RedirectToAction("Index", "Home", new { area = "" });
        }

        public async Task<IActionResult> CancelOrder()
        {
            var order = (await _orderService.SearchOrders(storeId: _workContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)).FirstOrDefault();
            if (order != null)
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });

            return RedirectToRoute("HomePage");
        }

        public IActionResult PaymentInfo()
        {
            return View();
        }
    }
}