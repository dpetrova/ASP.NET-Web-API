using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreateVideoBindingModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int LocationId { get; set; }
    }
}