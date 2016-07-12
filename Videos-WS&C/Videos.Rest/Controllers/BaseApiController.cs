using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Videos.Rest.Controllers
{
    using Data;

    public class BaseApiController : ApiController
    {
        public BaseApiController()
            : this(new VideosDbContext())
        {
        }

        public BaseApiController(VideosDbContext context)
        {
            this.Context = context;
        }

        protected VideosDbContext Context { get; set; }
    }
}
