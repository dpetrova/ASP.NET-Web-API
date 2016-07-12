using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class AddVideoBindingModel
    {
        [Required]
        public int VideoId { get; set; }
    }
}