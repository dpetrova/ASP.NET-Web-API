using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BugTracker.RestServices.Controllers
{
    using Data;
    using Data.Models;
    using Data.UnitOfWork;
    using Microsoft.AspNet.Identity;
    using Models;

    public class BugsController : BaseApiController
    {
        public BugsController(IBugTrackerData data) : base(data)
        {
        }
        
        // GET: api/bugs
        [HttpGet]
        public IHttpActionResult GetBugs()
        {
            //var bugs = this.Context.Bugs
            //    .OrderByDescending(b => b.DateCreated)
            //    .ThenByDescending(b => b.Id)
            //    .Select(b => new BugViewModel()
            //    {
            //        Id = b.Id,
            //        Title = b.Title,
            //        Status = b.Status.ToString(),
            //        Author = b.Author != null ? b.Author.UserName : null,
            //        DateCreated = b.DateCreated
            //    });

            var bugs = this.Context.Bugs.All()
                .OrderByDescending(b => b.DateCreated)
                .ThenByDescending(b => b.Id)
                .Select(BugViewModel.Create());

            return this.Ok(bugs);
        }


        // GET: api/bugs/{id}
        [HttpGet]
        [Route("api/bugs/{id}")]
        public IHttpActionResult GetBugById(int id)
        {
            var bug = this.Context.Bugs.All()
                .Where(b => b.Id == id)
                .Select(b => new BugDetailsViewModel()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    Status = b.Status.ToString(),
                    Author = b.Author != null ? b.Author.UserName : null,
                    DateCreated = b.DateCreated,
                    Comments = b.Comments
                        .OrderByDescending(c => c.DateCreated)
                        .ThenByDescending(c => c.Id)
                        .Select(c => new CommentViewModel()
                        {
                            Id = c.Id,
                            Text = c.Text,
                            Author = c.Author != null ? c.Author.UserName : null,
                            DateCreated = c.DateCreated
                        })
                }).FirstOrDefault();

            if (bug == null)
            {
                return this.NotFound();
            }

            return this.Ok(bug);
        }


        // POST: api/bugs
        [HttpPost]
        public IHttpActionResult AddBug(AddBugBindingModel bugInputData)
        {
            //check if input data are not null
            if (bugInputData == null)
            {
                return BadRequest("Missing bug data.");
            }

            //check validation in AddBugBindingModel bugData
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            //check if there is logged user
            var currentUserId = User.Identity.GetUserId();

            //check if database there is such user (not necessary)
            var user = this.Context.Users.All()
                .Where(u => u.Id == currentUserId)
                .Select(u => new
                {
                     u.Id,
                     u.UserName
                })
                .FirstOrDefault();

            //create new Bug
            var bug = new Bug()
            {
                Title = bugInputData.Title,
                Description = bugInputData.Description,
                Status = BugStatus.Open,
                AuthorId = user != null ? user.Id : null,
                DateCreated = DateTime.Now
            };

            //add to database
            this.Context.Bugs.Add(bug);
            this.Context.SaveChanges();

            if (currentUserId == null)
            {
                //it returns 201 Created
                return this.CreatedAtRoute(
                    "DefaultApi",
                    new { id = bug.Id },
                    new { bug.Id, Message = "Anonymous bug submitted." });
            }
            else
            {
                //it returns 201 Created
                return this.CreatedAtRoute(
                    "DefaultApi",
                    new { id = bug.Id },
                    new { bug.Id, Author = user.UserName, Message = "User bug submitted." });
            }
        }


        // PATCH: api/bugs/{id}
        [HttpPatch]
        [Route("api/bugs/{id}")]
        public IHttpActionResult EditBug(int id, EditBugBindingModel bugData)
        {
            if (bugData == null)
            {
                return BadRequest("Missing bug data to patch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bug = this.Context.Bugs.Find(id);
            if (bug == null)
            {
                return NotFound();
            }

            if (bugData.Title != null)
            {
                bug.Title = bugData.Title;
            }

            if (bugData.Description != null)
            {
                bug.Description = bugData.Description;
            }
            
            if (bugData.Status != null)
            {
                BugStatus newStatus;
                bool isSuccessfulParcing = Enum.TryParse(bugData.Status, out newStatus);
                if (!isSuccessfulParcing)
                {
                    return this.BadRequest("Invalid bug status");
                }
                bug.Status = newStatus;
            }

            this.Context.SaveChanges();

            return this.Ok(
                new
                {
                    //Message = "Bug #" + id + " patched."
                    Message = string.Format("Bug #{0} patched", bug.Id)
                }
            );
        }


        // DELETE: api/bugs/{id}
        [HttpDelete]
        [Route("api/bugs/{id}")]
        public IHttpActionResult DeleteBug(int id)
        {
            var bug = this.Context.Bugs.Find(id);
            if (bug == null)
            {
                return this.NotFound();
            }

            this.Context.Bugs.Delete(bug);
            this.Context.SaveChanges();

            return Ok(new
            {
                Message = "Bug #" + id + " deleted."
            });
        }


        // GET: api/bugs/filter
        [HttpGet]
        [Route("api/bugs/filter")]
        public IHttpActionResult GetBugsByFilter([FromUri]FilterBugsBindingModel filterData)
        {
            var bugs = this.Context.Bugs.All();

            if (filterData == null)
            {
                return this.BadRequest("You didint send any filter data");
            }

            if (filterData.Keyword != null)
            {
                bugs = bugs.Where(b => b.Title.Contains(filterData.Keyword));
            }

            //this will upgrade query with AND clause
            //if (filterData.Statuses != null)
            //{
            //    string[] statuses = filterData.Statuses.Split('|');
            //    for (int i = 0; i < statuses.Length; i++)
            //    {
            //        BugStatus parsedStatus;
            //        bool isSuccessfulParcing = Enum.TryParse(statuses[i], out parsedStatus);
            //        if (isSuccessfulParcing)
            //        {
            //            bugs = bugs.Where(b => b.Status == parsedStatus);
            //        }
            //    }
            //}

            //this will upgrade query with OR clause
            if (filterData.Statuses != null)
            {
                string[] statuses = filterData.Statuses.Split('|');
                var bugStatuses = new List<BugStatus>();
                for (int i = 0; i < statuses.Length; i++)
                {
                    BugStatus parsedStatus;
                    bool isSuccessfulParcing = Enum.TryParse(statuses[i], out parsedStatus);
                    if (isSuccessfulParcing)
                    {
                        bugStatuses.Add(parsedStatus);
                    }
                }
                bugs = bugs.Where(b => bugStatuses.Contains(b.Status));
            }

            if (filterData.Author != null)
            {
                bugs = bugs.Where(b => b.Author.UserName == filterData.Author);
            }

            //var outputBugs = bugs.Select(b => new BugViewModel()
            //{
            //    Id = b.Id,
            //    Title = b.Title,
            //    Status = b.Status.ToString(),
            //    Author = b.Author != null ? b.Author.UserName : null,
            //    DateCreated = b.DateCreated
            //});

            var outputBugs = bugs.Select(BugViewModel.Create());

            return this.Ok(outputBugs);
        }

    }
}
