using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Tests.Unit_Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.Routing;
    using Data.UnitOfWork;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Moq;
    using Services.Controllers;
    using Services.Models.ViewModels;


    [TestClass]
    public class RestaurantsControllerTests
    {
        private MockContainer mocks;

        [TestInitialize]
        public void InitTest()
        {
            this.mocks = new MockContainer();
            this.mocks.SetupMocks();
        }

        [TestMethod]
        public void GetRestaurants_ShouldReturnAllFromTown_WhenTownIdIsValid()
        {
            // Arrange
            var fakeData = this.mocks.MockData.Object;
            var existingTownId = this.mocks.TownsMock.Object.All()
                .First().Id;

            // Act
            var response = this.SendGetRestaurantsRequest(existingTownId, fakeData);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var restaurantCount = this.mocks.RestaurantsMock.Object
                .All()
                .Count(r => r.TownId == existingTownId);

            var restaurants = response.Content
                .ReadAsAsync<IEnumerable<RestaurantSimpleViewModel>>().Result;
            Assert.AreEqual(restaurantCount, restaurants.Count());

            foreach (var restaurant in restaurants)
            {
                Assert.IsNotNull(restaurant.Name);
                Assert.IsNotNull(restaurant.Town);
                Assert.AreEqual(existingTownId, restaurant.Town.Id);
            }
        }

        [TestMethod]
        public void GetRestaurants_ShouldReturn400BadRequest_WhenTownIdIsInvalid()
        {
            // Arrange
            var fakeData = this.mocks.MockData.Object;
            var invalidId = -1;

            // Act
            var response = this.SendGetRestaurantsRequest(invalidId, fakeData);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);            
        }

        private HttpResponseMessage SendGetRestaurantsRequest(int townId, IRestaurantsData data)
        {
            //var model = new SearchRestaurantsBindingModel { TownId = townId };

            var controller = new RestaurantsController(data);
            this.SetupController(controller);

            var response = controller.GetRestaurantsByTownId(townId)
                .ExecuteAsync(CancellationToken.None).Result;
            return response;
        }

        private void SetupController(ApiController controller)
        {
            controller.Configuration = new HttpConfiguration();
            controller.Request = new HttpRequestMessage();
        }
    }


    //[TestClass]
    //public class RestaurantsControllerTests
    //{
    //    private MockContainer mocks;

    //    [TestInitialize]
    //    public void InitTest()
    //    {
    //        this.mocks = new MockContainer();
    //        mocks.PrepareMocks();
    //    }


    //    [TestMethod]
    //    public void GetRestaurants_ShouldReturnAllFromTown()
    //    {
    //        var fakeRestaurants = this.mocks.RestaurantsMock.Object.All();

    //        var mockContext = new Mock<IRestaurantsData>();

    //        mockContext.Setup(c => c.Restaurants.All())
    //            .Returns(fakeRestaurants);

    //        var restaurantsController = new RestaurantsController(mockContext.Object);

    //        restaurantsController.Request = new HttpRequestMessage();
    //        restaurantsController.Configuration = new HttpConfiguration();

    //        var existingTownId = this.mocks.TownsMock.Object.All()
    //            .First().Id;

    //        var response = restaurantsController.GetRestaurantsByTownId(existingTownId)
    //                                                .ExecuteAsync(CancellationToken.None).Result;

    //        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

    //        var restaurantsResponse = response.Content
    //                .ReadAsAsync<IEnumerable<Restaurant>>()
    //                .Result
    //                .ToList();

    //        var orderedRestaurants = fakeRestaurants
    //            .OrderBy(r => r.Ratings.Average(rating => rating.Stars))
    //            .ThenBy(r => r.Name)
    //            //.Select(RestaurantSimpleViewModel.Create)
    //            .ToList();
            
    //        CollectionAssert.AreEqual(orderedRestaurants, restaurantsResponse);
    //    }
        
    //    private void SetupController(ApiController controller)
    //    {
    //        string serverUrl = "http://sample-url.com";

    //        // Setup the Request object of the controller
    //        var request = new HttpRequestMessage()
    //        {
    //            RequestUri = new Uri(serverUrl)
    //        };
    //        controller.Request = request;

    //        // Setup the configuration of the controller
    //        var config = new HttpConfiguration();
    //        config.Routes.MapHttpRoute(
    //            name: "DefaultApi",
    //            routeTemplate: "api/{controller}/{id}",
    //            defaults: new { id = RouteParameter.Optional });
    //        controller.Configuration = config;

    //        // Apply the routes to the controller
    //        controller.RequestContext.RouteData = new HttpRouteData(
    //            route: new HttpRoute(),
    //            values: new HttpRouteValueDictionary
    //            {
    //                { "controller", "restaurants" }
    //            });
    //    }
    //}
}
