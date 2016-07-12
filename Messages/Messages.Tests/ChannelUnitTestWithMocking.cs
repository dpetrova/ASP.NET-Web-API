using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.Routing;
    using Data.Models;
    using Data.UnitOfWork;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mocks;
    using Models;
    using RestServices.Controllers;
    using RestServices.Models;

    [TestClass]
    public class ChannelUnitTestWithMocking
    {
        [TestMethod]
        public void GetChannelById_MissingChannel_ShouldReturn404NotFound()
        {
            //Arrange -> create mock
            //създаваме фалшиви имплементации на IRepositories и IUnitOfWork с цел а не бутаме по базата, а да тестваме в паметта
            //предимства: по-бързо, не зависим от SQL-server; тестовете са по-обособени (тестваме само едно парче код)
            IMessagesUnitOfWork mockedUnitOfWork = new MessagesDataMock();
            var channelsMock = mockedUnitOfWork.Channels;
            channelsMock.Add(new Channel()
            {
                Id = 5,
                Name = "Channel #5"
            });
            channelsMock.Add(new Channel()
            {
                Id = 6,
                Name = "Channel #6"
            });

            //Act -> invoke ChannelsController
            var channelController = new ChannelsController(mockedUnitOfWork);
            this.SetupControllerForTesting(channelController, "channels");
            var httpResult = channelController.GetChannel(12).ExecuteAsync(new CancellationToken()).Result;

            //Assert -> 404 NotFound
            Assert.AreEqual(HttpStatusCode.NotFound, httpResult.StatusCode);
        }


        [TestMethod]
        public void GetChannelById_ExistingChannel_ShouldReturn200OK()
        {
            //Arrange -> create mock
            //създаваме фалшиви имплементации на IRepositories и IUnitOfWork с цел а не бутаме по базата, а да тестваме в паметта
            //предимства: по-бързо, не зависим от SQL-server; тестовете са по-обособени (тестваме само едно парче код)
            IMessagesUnitOfWork mockedUnitOfWork = new MessagesDataMock();
            var channelsMock = mockedUnitOfWork.Channels;
            channelsMock.Add(new Channel()
            {
                Id = 5,
                Name = "Channel #5"
            });
            channelsMock.Add(new Channel()
            {
                Id = 6,
                Name = "Channel #6"
            });

            //Act -> invoke ChannelsController
            var channelController = new ChannelsController(mockedUnitOfWork);
            this.SetupControllerForTesting(channelController, "channels");
            var httpResult = channelController.GetChannel(6).ExecuteAsync(new CancellationToken()).Result;

            //Assert -> 200 OK
            Assert.AreEqual(HttpStatusCode.OK, httpResult.StatusCode);
            var channel = httpResult.Content.ReadAsAsync<ChannelModel>().Result;
            Assert.AreEqual(6, channel.Id);
            Assert.AreEqual("Channel #6", channel.Name);
        }


        //metod for setup controller and prepare it for testing
        private void SetupControllerForTesting(ApiController controller, string controllerName)
        {
            string serverUrl = "http://sample-url.com";

            // Setup the Request object of the controller
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(serverUrl)
            };
            controller.Request = request;

            // Setup the configuration of the controller
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            controller.Configuration = config;

            // Apply the routes to the controller
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary
                {
                    { "controller", controllerName }
                });
        }
    }
}
