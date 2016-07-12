using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.ViewModels
{
    using System.Linq.Expressions;
    using Videos.Models;

    public class LocationViewModel
    {
        //public int Id { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public static Expression<Func<Location, LocationViewModel>> Create
        {
            get
            {
                return l => new LocationViewModel
                {
                    //Id = l.Id,
                    Country = l.Country,
                    City = l.City
                };
            }
        }
    }
}