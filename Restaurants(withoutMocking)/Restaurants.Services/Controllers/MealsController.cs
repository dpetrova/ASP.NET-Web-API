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
    public class MealsController : BaseApiController
    {

        // GET /api/restaurants/{id}/meals
        [AllowAnonymous]
        [HttpGet]
        [Route("api/restaurants/{id}/meals")]
        public IHttpActionResult GetMealsByRestaurantId(int id)
        {
            var restaurant = this.Context.Restaurants.Find(id);
            if (restaurant == null)
            {
                return this.BadRequest("Invalid restaurant id: " + id);
            }

            var meals = this.Context.Meals
                .Where(m => m.RestaurantId == id)
                .OrderBy(m => m.Type.Order)
                .ThenBy(m => m.Name)
                .Select(MealViewModel.Create)
                .ToList();

            return this.Ok(meals);
        }

        
        // POST /api/meals
        [HttpPost]
        [Route("api/meals/")]
        public IHttpActionResult CreateNewMeal(CreateMealBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var restaurant = this.Context.Restaurants.Find(model.RestaurantId);
            if (restaurant == null)
            {
                return this.BadRequest("Invalid restaurant id: " + model.RestaurantId);
            }

            var type = this.Context.MealTypes.Find(model.TypeId);
            if (type == null)
            {
                return this.BadRequest("Invalid meal type id: " + model.TypeId);
            }

            //check for duplicated meals names
            if (restaurant.Meals.Any(m => m.Name == model.Name))
            {
                return this.Conflict();
            }

            //check if logged user is restaurant owner
            var loggedUserId = this.User.Identity.GetUserId();
            if (loggedUserId != restaurant.OwnerId)
            {
                return this.Unauthorized();
            }

            var newMeal = new Meal
            {
                Name = model.Name,
                Price = model.Price,
                RestaurantId = restaurant.Id,
                TypeId = type.Id
            };

            this.Context.Meals.Add(newMeal);
            this.Context.SaveChanges();

            var newMealFromDb = this.Context.Meals
                .Where(m => m.Id == newMeal.Id)
                .Select(MealViewModel.Create)
                .FirstOrDefault();

            return this.CreatedAtRoute(
                "DefaultApi",
                new {controller = "meals", id = newMeal.Id},
                newMealFromDb
                );
        }


        // PUT /api/meals/{id}
        [HttpPut]
        [Route("api/meals/{id}")]
        public IHttpActionResult EditExistingMeal(int id, EditMealBindingModel model)
        {
            var meal = this.Context.Meals.Find(id);
            if (meal == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            //check for valid meal type
            if (!this.Context.MealTypes.Select(mt => mt.Id).Contains(model.TypeId))
            {
                return this.BadRequest("Invalid meal type");
            }        

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var restaurant = meal.Restaurant;
            var loggedUserId = this.User.Identity.GetUserId();
            if (loggedUserId != restaurant.OwnerId)
            {
                return this.Unauthorized();
            }

            meal.Name = model.Name;
            meal.Price = model.Price;
            meal.TypeId = model.TypeId;
            this.Context.SaveChanges();

            var editedMealFromDb = this.Context.Meals
                .Where(m => m.Id == meal.Id)
                .Select(MealViewModel.Create)
                .FirstOrDefault();

            return this.Ok(editedMealFromDb);
        }


        // DELETE /api/meals/{id}
        [HttpDelete]
        [Route("api/meals/{id}")]
        public IHttpActionResult DeleteMeal(int id)
        {
            var meal = this.Context.Meals.Find(id);
            if (meal == null)
            {
                return this.NotFound();
            }

            var restaurant = meal.Restaurant;
            var loggedUserId = this.User.Identity.GetUserId();
            if (loggedUserId != restaurant.OwnerId)
            {
                return this.Unauthorized();
            }

            this.Context.Meals.Remove(meal);
            this.Context.SaveChanges();

            return this.Ok(
                new
                {
                    Message = string.Format("Meal #{0} deleted", meal.Id)
                }
            );

        }

    }
}
