using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class GetOrdersBindingModel
    {
        public int StartPage { get; set; }

        [Range(2, 10)]
        public int Limit { get; set; }

        public int? MealId { get; set; }
    }
}