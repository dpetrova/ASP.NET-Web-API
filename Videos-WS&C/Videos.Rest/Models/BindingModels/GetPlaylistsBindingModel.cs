using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class GetPlaylistsBindingModel
    {
        public int StartPage { get; set; }

        [Range(2, 10)]
        public int Limit { get; set; }

        public int? LocationId { get; set; }

        public bool? IsAdultContent { get; set; }
    }
}