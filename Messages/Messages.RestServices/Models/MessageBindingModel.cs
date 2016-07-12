using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Messages.RestServices.Models
{
    using System.ComponentModel.DataAnnotations;

    public class MessageBindingModel
    {
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Text { get; set; }
    }
}