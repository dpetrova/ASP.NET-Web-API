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
    public class VideosController : BaseApiController
    {
        // GET /api/videos?locationId={locationId}
        [AllowAnonymous]
        [HttpGet]
        [Route("api/videos/")]
        public IHttpActionResult GetVideosByLocationId([FromUri]int locationId)
        {
            var location = this.Context.Locations.Find(locationId);
            if (location == null)
            {
                return this.BadRequest("Invalid location id: " + locationId);
            }

            var videos = this.Context.Videos
                .Where(v => v.LocationId == locationId && v.Status == VideoStatus.Published)
                .OrderBy(v => v.Owner.UserName)
                .ThenBy(v => v.Title)
                .Select(VideoViewModel.Create)
                .ToList();

            return this.Ok(videos);
        }

        
        // POST /api/videos
        [HttpPost]
        [Route("api/videos/")]
        public IHttpActionResult CreateNewVideo(CreateVideoBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var location = this.Context.Locations.Find(model.LocationId);

            if (location == null)
            {
                return this.BadRequest("Invalid location id: " + model.LocationId);
            }

            //check for duplicated video names
            if (this.Context.Videos.Any(r => r.Title == model.Title))
            {
                return this.Conflict();
            }

            var loggedUserId = this.User.Identity.GetUserId();

            //if (loggedUserId == null)
            //{
            //    this.Unauthorized();
            //}

            var newVideo = new Video
            {
                Title = model.Title,
                LocationId = model.LocationId,
                Status = VideoStatus.Pending,
                OwnerId = loggedUserId
            };

            this.Context.Videos.Add(newVideo);
            this.Context.SaveChanges();

            var newVideoFromDb = this.Context.Videos
                .Where(v => v.Id == newVideo.Id)
                .Select(VideoViewModel.Create)
                .FirstOrDefault();

            return this.CreatedAtRoute(
                "DefaultApi",
                new { controller = "videos", id = newVideo.Id },
                newVideoFromDb);
        }


        //POST /api/videos/{id}/addTag
        [HttpPost]
        [Route("api/videos/{id}/addTag")]
        public IHttpActionResult AddTagToVideo(int id, AddTagBindingModel model)
        {
            var video = this.Context.Videos.Find(id);
            if (video == null)
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

            var tag = this.Context.Tags.Find(model.TagId);
            if (tag == null)
            {
                return this.BadRequest("There is not such tag");
            }

            video.Tags.Add(tag);
            this.Context.SaveChanges();

            return this.Ok(new
            {
                Message = string.Format("Tag #{0} was added", tag.Id)
            });
        }
    }
}
