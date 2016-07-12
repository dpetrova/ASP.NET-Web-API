using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.ViewModels
{
    using System.Linq.Expressions;
    using Videos.Models;

    public class PlaylistDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public virtual IList<string> Tags { get; set; }

        public virtual IList<string> Videos { get; set; }


    }
}