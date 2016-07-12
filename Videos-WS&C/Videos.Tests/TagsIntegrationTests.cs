using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Videos.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Data;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Testing;
    using Models;
    using Owin;
    using Rest;
    using Rest.Models.ViewModels;

    [TestClass]
    public class TagsIntegrationTests
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



        private static void Seed()
        {
            var context = new VideosDbContext();

            if (!context.Users.Any())
            {
                SeedUsers(context);
            }

            if (!context.Tags.Any())
            {
                SeedTags(context);
            }

        }

        private static void SeedUsers(VideosDbContext context)
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

        private static void SeedTags(VideosDbContext context)
        {
            var tags = new List<Tag>()
            {
                new Tag()
                {
                    Name = "Tag #1",
                    IsAdultContent = false,
                    Owner = context.Users.FirstOrDefault(u => u.UserName == TestUserUsername)
                },

                new Tag()
                {
                    Name = "Tag #2",
                    IsAdultContent = false,
                    Owner = context.Users.FirstOrDefault(u => u.UserName == TestUserUsername)
                }
            };

            foreach (var tag in tags)
            {
                context.Tags.Add(tag);
            }

            context.SaveChanges();
        }


        //private static void CleanDatabase()
        //{
        //    var context = new VideosDbContext();

        //    context.Tags.Delete();
        //    context.Users.Delete();

        //    context.SaveChanges();
        //}


        [TestMethod]
        public void EditOwnTag_WithNewCorrectData_ShouldChangeTag_And_Return200OK()
        {
            // Arrange
            var context = new VideosDbContext();
            //get some own meal
            var ownTags = context.Tags
                .First(m => m.Owner.UserName == TestUserUsername);

            var newName = "New name";
            var newStatus = true;

            // Act
            this.SetAuthorizationHeaders(true);
            var response = this.SendEditTagRequest(ownTags.Id, newName, newStatus);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var tag = response.Content.ReadAsAsync<TagViewModel>().Result;
            Assert.AreEqual(newName, tag.Name);
            Assert.AreEqual(newStatus, tag.IsAdultContent);
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


        private HttpResponseMessage SendEditTagRequest(int tagId, string name, bool status)
        {
            var model = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", name),
                new KeyValuePair<string, string>("status", status.ToString()),
            });

            return client.PutAsync("api/tags/" + tagId, model).Result;
        }
    }
}
