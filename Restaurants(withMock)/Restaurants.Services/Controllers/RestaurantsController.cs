using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Restaurants.Services.Controllers
{
    using Data;
    using Data.UnitOfWork;
    using Microsoft.AspNet.Identity;
    using Models.BindingModels;
    using Models.ViewModels;
    using Restaurants.Models;

    [Authorize]
    public class RestaurantsController : BaseApiController
    {
        public RestaurantsController(IRestaurantsData data)
            : base(data)
        {
        }

        // GET /api/restaurants?townId={townId}
        [AllowAnonymous]
        [HttpGet]
        [Route("api/restaurants/")]
        public IHttpActionResult GetRestaurantsByTownId([FromUri]int townId)
        {
            var town = this.Data.Towns.Find(townId);
            if (town == null)
            {
                return this.BadRequest("Invalid town id: " + townId);
            }
            
            var restaurants = this.Data.Restaurants.All()
                .Where(r => r.TownId == townId)
                .OrderByDescending(r => r.Ratings.Average(rating => rating.Stars))
                .ThenBy(r => r.Name)
                .Select(RestaurantSimpleViewModel.Create)
                .ToList();

            return this.Ok(restaurants);
        }


        // POST /api/restaurants
        [HttpPost]
        [Route("api/restaurants/")]
        public IHttpActionResult CreateNewRestaurant(CreateRestaurantBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var town = this.Data.Towns.Find(model.TownId);

            if (town == null)
            {
                return this.BadRequest("Invalid town id: " + model.TownId);
            }

            //check for duplicated restaurants names
            if (this.Data.Restaurants.All().Any(r => r.Name == model.Name))
            {
                return this.Conflict();
            }
            
            var loggedUserId = this.User.Identity.GetUserId();

            //if (loggedUserId == null)
            //{
            //    this.Unauthorized();
            //}
            
            var newRestaurant = new Restaurant
            {
                Name = model.Name,
                TownId = model.TownId,
                OwnerId = loggedUserId
            };

            this.Data.Restaurants.Add(newRestaurant);
            this.Data.SaveChanges();

            var newRestaurantFromDb = this.Data.Restaurants.All()
                .Where(r => r.Id == newRestaurant.Id)
                .Select(RestaurantSimpleViewModel.Create)
                .FirstOrDefault();

            return this.CreatedAtRoute(
                "DefaultApi",
                new { controller = "restaurants", id = newRestaurant.Id },
                newRestaurantFromDb);
        }


        // POST /api/restaurants/{id}/rate
        [HttpPost]
        [Route("api/restaurants/{id}/rate")]
        public IHttpActionResult RateRestaurant(int id, RateRestaurantBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var restaurant = this.Data.Restaurants.Find(id);

            if (restaurant == null)
            {
                return this.NotFound();
            }

            var loggedUserId = this.User.Identity.GetUserId();

            //check if user is owner of the restaurant
            if (restaurant.OwnerId == loggedUserId)
            {
                return this.BadRequest("The owner cannot rate his own restaurant");
            }

            //check if user rated already
            if (this.Data.Ratings.All().Any(r => r.UserId == loggedUserId && r.RestaurantId == restaurant.Id))
            {
                var rate = this.Data.Ratings.All()
                    .FirstOrDefault(r => r.UserId == loggedUserId && r.RestaurantId == restaurant.Id);
                rate.Stars = model.Stars;
            }
            else
            {
                var newRate = new Rating
                {
                    Stars = model.Stars,
                    UserId = loggedUserId,
                    RestaurantId = restaurant.Id
                };

                this.Data.Ratings.Add(newRate);
            }
            
            this.Data.SaveChanges();

            return this.Ok();
        }
    }
}
