using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Tests.UnitTests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using Data.Models;
    using Data.Repositories;
    using Data.UnitOfWork;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using RestServices.Controllers;
    using RestServices.Models;

    [TestClass]
    public class BugUnitTests
    {
        //[TestMethod]
        //public void EditingExistingBug_ShouldChangeBugWithOnlySentData()
        //{
        //    //Arrange
        //    var fakeBugs = new List<Bug>
        //    {
        //        new Bug()
        //        {
        //            Id = 1,
        //            Title = "Bug #1",
        //            Description = "Bug description",
        //            DateCreated = DateTime.Now
        //        },
        //        new Bug()
        //        {
        //            Id = 2,
        //            Title = "Bug #2",
        //            Description = "Bug description",
        //            DateCreated = DateTime.Now
        //        }
        //    };

        //    var fakeRepo = new FakeBugsRepository(fakeBugs);

        //    var fakeUnitOfWork = new FakeUnitOfWork(fakeRepo);

        //    var newTitle = "Changed " + DateTime.Now.Ticks;
        //    var model = new EditBugBindingModel()
        //    {
        //        Title = newTitle
        //    };

        //    var oldDescription = fakeBugs[0].Description;
        //    var oldStatus = fakeBugs[0].Status;

        //    //Act
        //    var controller = new BugsController(fakeUnitOfWork);
        //    SetupController(controller);

        //    var response = controller.EditBug(1, model).ExecuteAsync(CancellationToken.None).Result;

        //    //Assert
        //    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        //    Assert.AreEqual(1, fakeUnitOfWork.SaveChangesCallCount); //assert that SaveChanges() is called only once
        //    Assert.AreEqual(oldDescription, fakeBugs[0].Description);
        //    Assert.AreEqual(oldStatus, fakeBugs[0].Status);            Assert.AreEqual(newTitle, fakeBugs[0].Title);
        //}


        //with Moq
        [TestMethod]
        public void EditingExistingBug_ShouldChangeBugWithOnlySentData()
        {
            //Arrange
            var fakeBugs = new List<Bug>
            {
                new Bug()
                {
                    Id = 1,
                    Title = "Bug #1",
                    Description = "Bug description",
                    DateCreated = DateTime.Now
                },
                new Bug()
                {
                    Id = 2,
                    Title = "Bug #2",
                    Description = "Bug description",
                    DateCreated = DateTime.Now
                }
            };

            var mockRepository = new Mock<IRepository<Bug>>();
            mockRepository.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeBugs.FirstOrDefault(b => b.Id == id);
                });

            var mockUnitOfWork = new Mock<IBugTrackerData>();
            mockUnitOfWork.Setup(u => u.Bugs)
                .Returns(mockRepository.Object);

            var newTitle = "Changed " + DateTime.Now.Ticks;
            var model = new EditBugBindingModel()
            {
                Title = newTitle
            };

            var oldDescription = fakeBugs[0].Description;
            var oldStatus = fakeBugs[0].Status;

            //Act
            var controller = new BugsController(mockUnitOfWork.Object);
            SetupController(controller);

            var response = controller.EditBug(1, model).ExecuteAsync(CancellationToken.None).Result;

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockUnitOfWork.Verify(c => c.SaveChanges(), Times.Once); //assert that SaveChanges() is called only once
            Assert.AreEqual(oldDescription, fakeBugs[0].Description);
            Assert.AreEqual(oldStatus, fakeBugs[0].Status); 
            Assert.AreEqual(newTitle, fakeBugs[0].Title);
        }



        [TestMethod]
        public void EditingNonExistingBug_ShouldReturn404NotFound()
        {
            //Arrange
            var fakeBugs = new List<Bug>
            {
                new Bug()
                {
                    Id = 1,
                    Title = "Bug #1",
                    Description = "Bug description",
                    DateCreated = DateTime.Now
                },
                new Bug()
                {
                    Id = 2,
                    Title = "Bug #2",
                    Description = "Bug description",
                    DateCreated = DateTime.Now
                }
            };

            var fakeRepo = new FakeBugsRepository(fakeBugs);

            var fakeUnitOfWork = new FakeUnitOfWork(fakeRepo);

            var newTitle = "Changed " + DateTime.Now.Ticks;
            var model = new EditBugBindingModel()
            {
                Title = newTitle
            };
           
            //Act
            var controller = new BugsController(fakeUnitOfWork);
            SetupController(controller);

            var response = controller.EditBug(-1, model).ExecuteAsync(CancellationToken.None).Result;

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual(0, fakeUnitOfWork.SaveChangesCallCount); //assert that SaveChanges() is never called
        }


        [TestMethod]
        public void EditingExistingBug_WithEmptyModel_ShouldReturn400BadRequest()
        {
            //Arrange
            var fakeBugs = new List<Bug>
            {
                new Bug()
                {
                    Id = 1,
                    Title = "Bug #1",
                    Description = "Bug description",
                    DateCreated = DateTime.Now
                },
                new Bug()
                {
                    Id = 2,
                    Title = "Bug #2",
                    Description = "Bug description",
                    DateCreated = DateTime.Now
                }
            };

            var fakeRepo = new FakeBugsRepository(fakeBugs);

            var fakeUnitOfWork = new FakeUnitOfWork(fakeRepo);

            EditBugBindingModel model = null;
            

            //Act
            var controller = new BugsController(fakeUnitOfWork);
            SetupController(controller);

            var response = controller.EditBug(1, model).ExecuteAsync(CancellationToken.None).Result;

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual(0, fakeUnitOfWork.SaveChangesCallCount); //assert that SaveChanges() is never called
        }

        private void SetupController(ApiController controller)
        {
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
        }
    }
}
