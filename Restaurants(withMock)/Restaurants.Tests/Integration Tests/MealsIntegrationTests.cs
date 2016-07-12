using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Restaurants.Tests.Integration_Tests
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Data;
    using EntityFramework.Extensions;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Testing;
    using Models;
    using Owin;
    using Restauranteur.Models;
    using Services;
    using Services.Models.ViewModels;

    [TestClass]
    public class MealsIntegrationTests
    {
        private static TestServer server;
        private static HttpClient client;

        private const string TestUserUsername = "motikarq";
        private const string TestUserPassword = "123456";

        private string accessToken;

        private string AccessToken
        {
            get
            {
                if (this.accessToken == null)
                {
                    var loginResponse = this.Login();
                    if (!loginResponse.IsSuccessStatusCode)
                    {
                        Assert.Fail("Unable to login: " + loginResponse.ReasonPhrase);
                    }

                    var loginData = loginResponse.Content
                        .ReadAsAsync<LoginDto>().Result;

                    this.accessToken = loginData.Access_Token;
                }

                return this.accessToken;
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // Create in-memory test server
            server = TestServer.Create(appBuilder =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);

                var startup = new Startup();
                startup.Configuration(appBuilder);

                appBuilder.UseWebApi(config);
            });

            client = server.HttpClient;

            Seed();
        }


        [AssemblyCleanup]
        public static void Cleanup()
        {
            if (server != null)
            {
                server.Dispose();
            }

            //CleanDatabase();
        }


        public static void Seed()
        {
            var context = new RestaurantsContext();

            if (!context.Users.Any())
            {
                SeedUsers(context);
            }

            if (!context.Meals.Any())
            {
                var mealTypes = SeedMealTypes(context);

                SeedMeals(context, mealTypes);
            }
        }

        private static void SeedMeals(RestaurantsContext context, List<MealType> mealTypes)
        {
            var meals = new List<Meal>()
            {
                new Meal()
                {
                    Name = "Chorba",
                    Price = 5.40m,
                    Restaurant = new Restaurant()
                    {
                        Name = "Pri baba",
                        Owner = context.Users.FirstOrDefault(u => u.UserName == TestUserUsername),
                        Town = new Town() {Name = "Bracigovo"}
                    },
                    Type = mealTypes.First()
                },

                new Meal()
                {
                    Name = "Mish-mash",
                    Price = 6.00m,
                    Restaurant = new Restaurant()
                    {
                        Name = "Old tavern",
                        Owner = context.Users.FirstOrDefault(u => u.UserName == TestUserUsername),
                        Town = new Town() {Name = "Tryavna"}
                    },
                    Type = mealTypes.Last()
                }
            };

            foreach (var meal in meals)
            {
                context.Meals.Add(meal);
            }

            context.SaveChanges();
        }

        private static List<MealType> SeedMealTypes(RestaurantsContext context)
        {
            var mealTypes = new List<MealType>()
            {
                new MealType() {Name = "Type #1", Order = 10},
                new MealType() {Name = "Type #2", Order = 20}
            };

            foreach (var mealType in mealTypes)
            {
                context.MealTypes.Add(mealType);
            }

            context.SaveChanges();
            return mealTypes;
        }

        private static void SeedUsers(RestaurantsContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);

            var user = new ApplicationUser()
            {
                UserName = TestUserUsername,
                Email = string.Format("{0}@gmail.com", TestUserUsername)
            };

            var userResult = userManager
                .CreateAsync(user, TestUserPassword).Result;
            if (!userResult.Succeeded)
            {
                Assert.Fail(string.Join("\n", userResult.Errors));
            }
        }


        private static void CleanDatabase()
        {
            var context = new RestaurantsContext();

            context.MealTypes.Delete();
            context.Meals.Delete();
            context.Users.Delete();

            context.SaveChanges();
        }


        [TestMethod]
        public void EditOwnRestaurantMeal_WithNewCorrectData_ShouldChangeMeal_And_Return200OK()
        {
            // Arrange
            var context = new RestaurantsContext();
            //get some own meal
            var ownMeal = context.Meals
                .First(m => m.Restaurant.Owner.UserName == TestUserUsername);

            var newType = context.MealTypes.First(t => t.Id != ownMeal.TypeId);
            var newName = "New name";
            var newPrice = 99.99m;

            // Act
            this.SetAuthorizationHeaders(true);
            var response = this.SendEditMealRequest(ownMeal.Id, newName, newPrice, newType.Id);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var meal = response.Content.ReadAsAsync<MealViewModel>().Result;
            Assert.AreEqual(newName, meal.Name);
            Assert.AreEqual(newPrice, meal.Price);
            Assert.AreEqual(newType.Name, meal.Type);
        }


        [TestMethod]
        public void EditOwnRestaurantMeal_WithInvalidMealTypeId_ShouldReturn400BadRequest()
        {
            // Arrange
            var context = new RestaurantsContext();
            var ownMeal = context.Meals
                .First(m => m.Restaurant.Owner.UserName == TestUserUsername);

            var invalidNewTypeId = -1;
            var newName = "New name";
            var newPrice = 99.99m;

            // Act
            this.SetAuthorizationHeaders(true);

            var response = this.SendEditMealRequest(ownMeal.Id, newName, newPrice, invalidNewTypeId);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [TestMethod]
        public void EditNonExistingMeal_ShouldReturn404NotFound()
        {
            // Arrange
            var context = new RestaurantsContext();

            var newType = context.MealTypes.FirstOrDefault();
            var newName = "New name";
            var newPrice = 99.99m;

            // Act
            this.SetAuthorizationHeaders(true);
            var response = this.SendEditMealRequest(-1, newName, newPrice, newType.Id);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


        [TestMethod]
        public void EditRestaurantMeal_WithoutAccessToken_ShouldReturn401Unauthorized()
        {
            // Arrange
            var context = new RestaurantsContext();
            var ownMeal = context.Meals
                .First(m => m.Restaurant.Owner.UserName == TestUserUsername);

            var newType = context.MealTypes.First(t => t.Id != ownMeal.TypeId);
            var newName = "New name";
            var newPrice = 99.99m;

            // Act
            this.SetAuthorizationHeaders(false);

            var response = this.SendEditMealRequest(ownMeal.Id, newName, newPrice, newType.Id);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        private HttpResponseMessage Login()
        {
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", TestUserUsername),
                new KeyValuePair<string, string>("password", TestUserPassword),
                new KeyValuePair<string, string>("grant_type", "password")
            });

            var response = client.PostAsync("api/account/login", loginData).Result;

            return response;
        }


        private void SetAuthorizationHeaders(bool isLogged)
        {
            client.DefaultRequestHeaders.Remove("Authorization");
            if (isLogged)
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.AccessToken);
            }
        }


        private HttpResponseMessage SendEditMealRequest(int mealId, string name, decimal price, int typeId)
        {
            var model = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", name),
                new KeyValuePair<string, string>("price", price.ToString()),
                new KeyValuePair<string, string>("typeId", typeId.ToString())
            });

            return client.PutAsync("api/meals/" + mealId, model).Result;
        }

    }
}
