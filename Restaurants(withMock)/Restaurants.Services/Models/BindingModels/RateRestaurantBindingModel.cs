using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class RateRestaurantBindingModel
    {
        [Range(0, 10)]
        [Required]
        public int Stars { get; set; }
    }
}