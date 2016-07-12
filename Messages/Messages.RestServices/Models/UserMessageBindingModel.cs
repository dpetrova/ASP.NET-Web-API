using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Messages.RestServices.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserMessageBindingModel
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string Recipient { get; set; }
    }
}