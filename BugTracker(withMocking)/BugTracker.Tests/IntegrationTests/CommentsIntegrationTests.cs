namespace BugTracker.Tests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Data;
    using Data.Models;
    using Microsoft.Owin.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Owin;
    using RestServices;
    using RestServices.Models;

    [TestClass]
    public class CommentsIntegrationTests
    {
        private static TestServer server;
        private static HttpClient client;

        [AssemblyInitialize]
        public static void InitializeTests(TestContext context)
        {
            server = TestServer.Create(appBuilder =>
            {
                var httpConfig = new HttpConfiguration();
                WebApiConfig.Register(httpConfig);

                var webAppStartup = new Startup();
                webAppStartup.Configuration(appBuilder);

                appBuilder.UseWebApi(httpConfig);
            });

            client = server.HttpClient;
            Seed();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (server != null)
            {
                server.Dispose();
            }
        }

        private static void Seed()
        {
            var context = new BugTrackerDbContext();
            if (!context.Bugs.Any())
            {
                context.Bugs.Add(new Bug
                {
                    Title = "Bug #1",
                    Description = "First bug",
                    DateCreated = DateTime.Now,
                    Comments = new List<Comment>()
                    {
                        new Comment()
                        {
                            Id = 1,
                            Text = "text",
                            AuthorId = "1",
                            BugId = 1,
                            DateCreated = DateTime.Now
                        }
                    }
                    });

                context.SaveChanges();
            }
        }


        [TestMethod]
        public void GetBugComments_ShouldReturn200OK_ExistingBug()
        {
            var context = new BugTrackerDbContext();
            var existingBug = context.Bugs.FirstOrDefault();
            if (existingBug == null)
            {
                Assert.Fail("cannot perform test - no bugs in DB");
            }

            var endpoint = string.Format("api/bugs/{0}/comments", existingBug.Id);
            var response = client.GetAsync(endpoint).Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var comments = response.Content.ReadAsAsync<List<CommentViewModel>>().Result;
            foreach (var comment in comments)
            {
                Assert.IsNotNull(comment.Text);
                Assert.AreNotEqual(0, comment.Id);
                Assert.AreEqual(1, comment.Id);
                Assert.AreEqual("text", comment.Text);
            }
        }


        [TestMethod]
        public void GetBugComments_ShouldReturn404NotFound_NonExistingBug()
        {
            var endpoint = string.Format("api/bugs/{0}/comments", -1);
            var response = client.GetAsync(endpoint).Result;
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
