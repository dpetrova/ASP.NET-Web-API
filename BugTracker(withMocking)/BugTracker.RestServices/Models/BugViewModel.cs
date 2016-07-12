using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.RestServices.Models
{
    using System.Linq.Expressions;
    using Data.Models;

    public class BugViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }

        public string Author { get; set; }

        public DateTime DateCreated { get; set; }

        public static Expression<Func<Bug, BugViewModel>> Create()
        {
            return b => new BugViewModel()
            {
                Id = b.Id,
                Title = b.Title,
                Status = b.Status.ToString(),
                Author = b.Author != null ? b.Author.UserName : null,
                DateCreated = b.DateCreated
            };
        }
    }
}