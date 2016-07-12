using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BugTracker.RestServices.Controllers
{
    using Data;
    using Data.UnitOfWork;

    public class BaseApiController : ApiController
    {
        public BaseApiController() : this(new BugTrackerData(new BugTrackerDbContext()))
        {
        }

        public BaseApiController(IBugTrackerData data)
        {
            this.Context = data;
        }

        protected IBugTrackerData Context { get; set; }
    }
}
