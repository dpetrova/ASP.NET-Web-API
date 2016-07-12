using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Tests
{
    using System.Net;
    using System.Net.Http;
    using Data;
    using Data.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;

    [TestClass]
    public class ChannelIntegrationTests
    {
        [TestMethod]
        public void DeleteChannel_NotExisting_ShoulReturn404NotFound()
        {
            // Arrange -> clean the database
            TestingEngine.CleanDatabase();
            var nonExistingChannelId = 7;

            // Act -> delete the channel
            var httpDeleteResponse = TestingEngine.HttpClient.DeleteAsync(
                "/api/channels/" + nonExistingChannelId).Result;

            // Assert -> HTTP status code is 404 (Not Found)
            Assert.AreEqual(HttpStatusCode.NotFound, httpDeleteResponse.StatusCode);
        }


        [TestMethod]
        public void DeleteChannel_Existing_ShoulReturn200OK()
        {
            // Arrange -> create a channel
            TestingEngine.CleanDatabase();
            var channel = new Channel()
            {
                Name = "New channel"
            };
            var db = new MessagesDbContext();
            db.Channels.Add(channel);
            db.SaveChanges();

            // Act -> delete the channel
            var httpDeleteResponse = TestingEngine.HttpClient.DeleteAsync(
                "/api/channels/" + channel.Id).Result;

            // Assert -> HTTP status code is 200 (OK)
            Assert.AreEqual(HttpStatusCode.OK, httpDeleteResponse.StatusCode);
            var dict = httpDeleteResponse.Content.ReadAsAsync<Dictionary<string, string>>().Result; //deserialize JSON
            Assert.IsNotNull(dict["Message"]);
            Assert.AreEqual(0, db.Channels.Count());
        }



        [TestMethod]
        public void DeleteChannel_ExistingNonEmpty_ShoulReturn409Conflict()
        {
            // Arrange -> create a channel
            TestingEngine.CleanDatabase();
            var channel = new Channel()
            {
                Name = "New channel",
                ChannelMessages = new List<ChannelMessage>()
                {
                    new ChannelMessage()
                    {
                        Id = 111,
                        Text = "sample text",
                        DateSent = DateTime.Now,
                        Sender = null
                    }
                }
            };
            var db = new MessagesDbContext();
            db.Channels.Add(channel);
            db.SaveChanges();

            // Act -> delete the channel
            var httpDeleteResponse = TestingEngine.HttpClient.DeleteAsync(
                "/api/channels/" + channel.Id).Result;

            // Assert -> HTTP status code is 409 (Conflict)
            Assert.AreEqual(HttpStatusCode.Conflict, httpDeleteResponse.StatusCode);
            Assert.AreEqual(1, db.Channels.Count());
        }
    }
}
