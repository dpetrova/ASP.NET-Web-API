using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreateTagBindingModel
    {
        [Required]
        public string Name { get; set; }

        public bool IsAdultContent { get; set; }
    }
}