using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;
    using Restaurants.Models;

    public class CreateMealBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int LocationId { get; set; }
        
    }
}