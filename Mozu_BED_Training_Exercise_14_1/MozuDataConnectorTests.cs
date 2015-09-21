using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mozu.Api;
using Autofac;
using Mozu.Api.ToolKit.Config;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Mozu_BED_Training_Exercise_14_1
{
    [TestClass]
    public class MozuDataConnectorTests
    {
        private IApiContext _apiContext;
        private IContainer _container;

        [TestInitialize]
        public void Init()
        {
            _container = new Bootstrapper().Bootstrap().Container;
            var appSetting = _container.Resolve<IAppSetting>();
            var tenantId = int.Parse(appSetting.Settings["TenantId"].ToString());
            var siteId = int.Parse(appSetting.Settings["SiteId"].ToString());

            _apiContext = new ApiContext(tenantId, siteId);
        }

        [TestMethod]
        public void Exercise_14_1_Get_Orders()
        {
            //Create an Order resource. This resource is used to get, create, update orders
            var orderResource = new Mozu.Api.Resources.Commerce.OrderResource(_apiContext);

            //Filter orders by statuses
            var acceptedOrders = orderResource.GetOrdersAsync(filter: "Status eq 'Accepted'").Result;
            var closedOrders = orderResource.GetOrdersAsync(filter: "Status eq 'Closed'").Result;

            //Filter orders by acct number
            var orderByCustId = orderResource.GetOrdersAsync(filter: "CustomerAccountId eq '1001'").Result;

            //Filter orders by email
            var orderByEmail = orderResource.GetOrdersAsync(filter: "Email eq 'test@customer.com'").Result;

            //Filter orders by order number
            var existingOrders = orderResource.GetOrdersAsync(filter: "OrderNumber eq '1'").Result;

            //Initialize the Order variable
            Mozu.Api.Contracts.CommerceRuntime.Orders.Order existingOrder = null;
            //Check if an Order was returned
            if (existingOrders.TotalCount > 0)
            {
                //Set the Order to the first occurance in the collection
                existingOrder = existingOrders.Items[0];
            }

            if (existingOrder != null)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Order Status Values: " 
                    + Environment.NewLine +
                    "Status={0}" 
                    + Environment.NewLine + 
                    "FulfillmentStatus={1}" 
                    + Environment.NewLine + 
                    "PaymentStatus={2}" 
                    + Environment.NewLine + 
                    "ReturnStatus={3}",
                   existingOrder.Status,
                   existingOrder.FulfillmentStatus,
                   existingOrder.PaymentStatus,
                   existingOrder.ReturnStatus));


                //Write out payment statuses
                foreach (var existingPayment in existingOrder.Payments)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Payment Status Value[{0}]: Status={1}",
                        existingPayment.Id,
                        existingPayment.Status));

                    //Write out payment interaction statuses
                    foreach (var existingInteraction in existingPayment.Interactions)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Payment Interaction Status Value[{0}]: Status={1}",
                            existingInteraction.Id,
                            existingInteraction.Status));
                    }
                }

                //Write out order package statuses
                foreach (var existingPackage in existingOrder.Packages)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Package Status Value[{0}]: Status={1}",
                        existingPackage.Id,
                        existingPackage.Status));
                }
            }
        }
    }
}
