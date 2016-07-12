using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.ViewModels
{
    using System.Linq.Expressions;
    using Restaurants.Models;

    public class TownSimpleViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static Expression<Func<Town, TownSimpleViewModel>> Create
        {
            get
            {
                return t => new TownSimpleViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                };
            }
        }
    }
}