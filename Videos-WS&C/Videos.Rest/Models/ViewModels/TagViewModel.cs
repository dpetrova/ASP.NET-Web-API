using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.ViewModels
{
    using System.Linq.Expressions;
    using Videos.Models;

    public class TagViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string IsAdultContent { get; set; }

        public static Expression<Func<Tag, TagViewModel>> Create
        {
            get
            {
                return t => new TagViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    IsAdultContent = t.IsAdultContent.ToString()
                };
            }
        }
    }
}