using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CreateRestaurantBindingModel
    {
        [Required]
        [Index(IsUnique = true)]
        [MinLength(1)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public int TownId { get; set; }
    }
}