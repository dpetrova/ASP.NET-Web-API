using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.ViewModels
{
    using System.Linq.Expressions;
    using Restaurants.Models;

    public class RestaurantSimpleViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double? AverageRating { get; set; }

        public virtual TownSimpleViewModel Town { get; set; }

        public static Expression<Func<Restaurant, RestaurantSimpleViewModel>> Create
        {
            get
            {
                return r => new RestaurantSimpleViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    AverageRating = r.Ratings.Average(rating => rating.Stars),
                    Town = new TownSimpleViewModel
                    {
                        Id = r.Town.Id,
                        Name = r.Town.Name
                    }
                };
            }
        }
    }
}