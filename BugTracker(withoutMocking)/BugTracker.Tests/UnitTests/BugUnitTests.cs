using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Tests.UnitTests
{
    using System.Net;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RestServices.Controllers;
    using RestServices.Models;

    [TestClass]
    public class BugUnitTests
    {
        [TestMethod]
        public void EditingExistingBug_ShouldChangeBugWithOnlySentData()
        {
            var controller = new BugsController();

            var model = new EditBugBindingModel()
            {
                Title = "Changed " + DateTime.Now.Ticks
            };

            var response = controller.EditBug(3, model).ExecuteAsync(CancellationToken.None).Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
