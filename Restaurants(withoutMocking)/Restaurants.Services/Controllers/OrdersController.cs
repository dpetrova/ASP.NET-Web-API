using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Restaurants.Services.Controllers
{
    using Microsoft.AspNet.Identity;
    using Models.BindingModels;
    using Models.ViewModels;
    using Restaurants.Models;

    [Authorize]
    public class OrdersController : BaseApiController
    {
        // POST /api/meals/{id}/order
        [HttpPost]
        [Route("api/meals/{mealId}/order")]
        public IHttpActionResult CreateNewOrder(int mealId, CreateOrderBindingModel model)
        {
            var meal = this.Context.Meals.Find(mealId);
            if (meal == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var loggedUserId = this.User.Identity.GetUserId();

            var newOrder = new Order
            {
                MealId = meal.Id,
                Quantity = model.Quantity,
                UserId = loggedUserId,
                OrderStatus = OrderStatus.Pending,
                CreatedOn = DateTime.Now
            };

            this.Context.Orders.Add(newOrder);
            this.Context.SaveChanges();

            return this.Ok(new
            {
                Message = string.Format("Order #{0} is pending", meal.Id)
            });
        }


        // GET /api/orders?startPage={start-page}&limit={page-size}&mealId={mealId}
        [HttpGet]
        [Route("api/orders/")]
        public IHttpActionResult GetPendingOrders([FromUri]GetOrdersBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var loggedUserId = this.User.Identity.GetUserId();

            var orders = this.Context.Orders
                .Where(o => o.UserId == loggedUserId && o.OrderStatus == OrderStatus.Pending);

            if (model.MealId.HasValue)
            {
                orders = orders
                    .Where(o => o.MealId == model.MealId);
            }
            
            var pagedOrders = orders
                .OrderByDescending(o => o.CreatedOn)
                .Skip(model.StartPage * model.Limit)
                .Take(model.Limit)
                .Select(OrderViewModel.Create)
                .ToList();

            return this.Ok(pagedOrders);
        }

    }
}
