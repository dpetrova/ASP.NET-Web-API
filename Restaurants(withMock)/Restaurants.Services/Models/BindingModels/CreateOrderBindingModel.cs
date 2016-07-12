using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;
    using Restaurants.Models;

    public class CreateOrderBindingModel
    {
        [Required]
        public int Quantity { get; set; }
        
    }
}