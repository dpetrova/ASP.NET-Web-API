using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Videos.Rest.Controllers
{
    using Microsoft.AspNet.Identity;
    using Models.BindingModels;
    using Models.ViewModels;
    using Videos.Models;

    [Authorize]
    public class TagsController : BaseApiController
    {
        //POST /api/Tags
        [AllowAnonymous]
        [HttpPost]
        [Route("api/tags/")]
        public IHttpActionResult CreateNewTag(CreateTagBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            
            //check for duplicated tag names
            if (this.Context.Tags.Any(t => t.Name == model.Name))
            {
                return this.BadRequest("Such tag already exists");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            
            var newTag = new Tag
            {
                Name = model.Name,
                IsAdultContent = model.IsAdultContent,
                OwnerId = loggedUserId
            };

            this.Context.Tags.Add(newTag);
            this.Context.SaveChanges();

            var newTagFromDb = this.Context.Tags
                .Where(t => t.Id == newTag.Id)
                .Select(TagViewModel.Create)
                .FirstOrDefault();

            return this.CreatedAtRoute(
                "DefaultApi",
                new { controller = "tags", id = newTag.Id },
                newTagFromDb);
        }


        // PUT /api/Tags/{id}
        [HttpPut]
        [Route("api/Tags/{id}")]
        public IHttpActionResult EditTag(int id, CreateTagBindingModel model)
        {
            var tag = this.Context.Tags.Find(id);
            if (tag == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var loggedUserId = this.User.Identity.GetUserId();
            //if (loggedUserId != tag.OwnerId)
            //{
            //    return this.Unauthorized();
            //}

            tag.Name = model.Name;
            tag.IsAdultContent = model.IsAdultContent;
            this.Context.SaveChanges();

            var editedTagFromDb = this.Context.Tags
                .Where(t => t.Id == tag.Id)
                .Select(TagViewModel.Create)
                .FirstOrDefault();

            return this.Ok(editedTagFromDb);
        }


        // DELETE /api/Tags/{id}
        [HttpDelete]
        [Route("api/Tags/{id}")]
        public IHttpActionResult DeleteTag(int id)
        {
            var tag = this.Context.Tags.Find(id);
            if (tag == null)
            {
                return this.NotFound();
            }

            var loggedUserId = this.User.Identity.GetUserId();
            if (loggedUserId != tag.OwnerId)
            {
                return this.Unauthorized();
            }

            //if (this.Context.Videos.Any(v => v.Tags.Contains(tag)))
            //{
            //    return this.BadRequest("Cannot delete tag");
            //}

            //if (this.Context.Playlists.Any(p => p.Tags.Contains(tag)))
            //{
            //    return this.BadRequest("Cannot delete tag");
            //}

            this.Context.Tags.Remove(tag);
            this.Context.SaveChanges();

            return this.Ok(
                new
                {
                    Message = string.Format("Tag #{0} deleted", tag.Id)
                }
            );



        }
    }
}
