using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.ViewModels
{
    using System.Linq.Expressions;
    using Videos.Models;

    public class VideoViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public virtual LocationViewModel Location { get; set; }

        public string Author { get; set; }

        public static Expression<Func<Video, VideoViewModel>> Create
        {
            get
            {
                return v => new VideoViewModel
                {
                    Id = v.Id,
                    Title = v.Title,
                    Status = v.Status.ToString(),
                    Location = new LocationViewModel
                    {
                        Country = v.Location.Country,
                        City = v.Location.City
                    },
                    Author = v.Owner.UserName
                };
            }
        }
    }
}