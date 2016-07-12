using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;
    using Videos.Models;

    public class CreatePlaylistBindingModel
    {
        [Required]
        public string Name { get; set; }

        public bool IsAdultContent { get; set; }

        //public virtual ICollection<Tag> Tags { get; set; }

        //public virtual ICollection<Video> Videos { get; set; }
    }
}