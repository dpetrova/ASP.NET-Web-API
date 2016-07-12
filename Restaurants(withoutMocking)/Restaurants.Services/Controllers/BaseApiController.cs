using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurants.Services.Controllers
{
    using System.Web.Http;
    using Data;

    public class BaseApiController : ApiController
    {
        public BaseApiController() : this(new RestaurantsContext())
        {
        }

        public BaseApiController(RestaurantsContext context)
        {
            this.Context = context;
        }

        protected RestaurantsContext Context { get; set; }
    }
}