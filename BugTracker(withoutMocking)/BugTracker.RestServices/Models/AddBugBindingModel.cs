using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.RestServices.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AddBugBindingModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
    }
}