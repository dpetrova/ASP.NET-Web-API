using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BugTracker.RestServices.Controllers
{
    using Data;

    public class BaseApiController : ApiController
    {
        public BaseApiController()
        {
            this.Context = new BugTrackerDbContext();
        }

        protected BugTrackerDbContext Context { get; set; }
    }
}
