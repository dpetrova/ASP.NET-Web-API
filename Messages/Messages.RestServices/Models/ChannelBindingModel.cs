using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Messages.RestServices.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ChannelBindingModel
    {
        [Required]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Channel name length should be in range [1..200]")]
        public string Name { get; set; }
    }
}