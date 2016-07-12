using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class AddTagBindingModel
    {
        [Required]
        public int TagId { get; set; }
    }
}