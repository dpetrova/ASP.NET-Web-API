using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BugTracker.RestServices.Controllers
{
    using Data.Models;
    using Data.UnitOfWork;
    using Microsoft.AspNet.Identity;
    using Models;

    public class CommentsController : BaseApiController
    {
        public CommentsController(IBugTrackerData data) : base(data)
        {
        }

        // GET: api/comments
        [HttpGet]
        [Route("api/comments")]
        public IHttpActionResult GetComments()
        {
            var comments = this.Context.Comments.All()
                .OrderByDescending(c => c.DateCreated)
                .ThenByDescending(c => c.Id)
                .Select(c => new 
                {
                    Id = c.Id,
                    Text = c.Text,
                    Author = c.Author != null ? c.Author.UserName : null,
                    DateCreated = c.DateCreated,
                    Bug = new
                    {
                        Id = c.Bug.Id,
                        Title = c.Bug.Title
                    }
                });
            return this.Ok(comments);
        }


        // GET: api/bugs/{id}/comments
        [HttpGet]
        [Route("api/bugs/{id}/comments")]
        public IHttpActionResult GetBugComments(int id)
        {
            var bug = this.Context.Bugs.Find(id);
            if (bug == null)
            {
                return this.NotFound();
            }

            var comments = bug.Comments
                .OrderByDescending(c => c.DateCreated)
                .ThenByDescending(c => c.Id)
                .Select(c => new CommentViewModel()
                {
                    Id = c.Id,
                    Text = c.Text,
                    Author = c.Author != null ? c.Author.UserName : null,
                    DateCreated = c.DateCreated
                });

            return Ok(comments);
        }


        // POST: api/bugs/{id}/comments
        [HttpPost]
        [Route("api/bugs/{id}/comments")]
        public IHttpActionResult AddComment(int id, CommentBindingModel commentInputData)
        {
            if (commentInputData == null)
            {
                return this.BadRequest("Missing comment data.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var bug = this.Context.Bugs.Find(id);
            if (bug == null)
            {
                return this.NotFound();
            }

            var loggedUserId = User.Identity.GetUserId();
            //check if this user has not been deleted and his access_token is used from before
            var user = this.Context.Users.Find(loggedUserId);

            var comment = new Comment()
            {
                Text = commentInputData.Text,
                Author = user,
                DateCreated = DateTime.Now
            };

            bug.Comments.Add(comment);
            this.Context.SaveChanges();

            if (user != null)
            {
                return this.Ok(new
                {
                    comment.Id,
                    Author = user.UserName,
                    Message = "User comment added for bug #" + bug.Id
                });
            }
            return this.Ok(new
            {
                Id = comment.Id,
                Message = "Added anonymous comment for bug #" + bug.Id
            });
        }
    }
}
