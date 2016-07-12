using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restaurants.Services.Controllers
{
    using System.Web.Http;
    using Data;
    using Data.UnitOfWork;

    public class BaseApiController : ApiController
    {
        //public BaseApiController() : this(new RestaurantsData(new RestaurantsContext()))
        //{
        //}

        public BaseApiController(IRestaurantsData data)
        {
            this.Data = data;
        }

        public IRestaurantsData Data { get; set; }
    }
}