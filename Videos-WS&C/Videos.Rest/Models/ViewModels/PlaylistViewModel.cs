using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.ViewModels
{
    using System.Linq.Expressions;
    using Videos.Models;

    public class PlaylistViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
       
        public string Author { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<Video> Videos { get; set; }

        public static Expression<Func<Playlist, PlaylistViewModel>> Create
        {
            get
            {
                return v => new PlaylistViewModel
                {
                    Id = v.Id,
                    Name = v.Name,
                    Author = v.Owner.UserName,
                    Tags = v.Tags,
                    Videos = v.Videos
                };
            }
        }
    }
}