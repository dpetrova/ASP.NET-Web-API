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
    public class PlaylistsController : BaseApiController
    {
        //POST /api/Playlists
        [HttpPost]
        [Route("api/Playlists")]
        public IHttpActionResult CreateNewPlaylist(CreatePlaylistBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Data cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var loggedUserId = this.User.Identity.GetUserId();

            var newPlaylist = new Playlist
            {
                Name = model.Name,
                OwnerId = loggedUserId
            };

            this.Context.Playlists.Add(newPlaylist);
            this.Context.SaveChanges();

            var newPlaylistFromDb = this.Context.Playlists
                .Where(p => p.Id == newPlaylist.Id)
                .Select(p => new PlaylistDetailsViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Author = p.Owner.UserName,
                    Tags = p.Tags.Select(t => t.Name).ToList(),
                    Videos = p.Videos.Select(v => v.Title).ToList()
                })
                .FirstOrDefault();

            return this.CreatedAtRoute(
                "DefaultApi",
                new { controller = "playlists", id = newPlaylist.Id },
                newPlaylistFromDb);
        }


        //POST /api/playlists/{id}/addVideo
        [HttpPost]
        [Route("api/playlists/{id}/addVideo")]
        public IHttpActionResult AddVideoToPlaylist(int id, AddVideoBindingModel model)
        {
            var playlist = this.Context.Playlists.Find(id);
            if (playlist == null)
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

            var video = this.Context.Videos.Find(model.VideoId);
            if (video == null)
            {
                return this.BadRequest("There is not such video");
            }

            playlist.Videos.Add(video);
            this.Context.SaveChanges();

            return this.Ok(new
            {
                Message = string.Format("Video {0} was added", video.Title)
            });
        }


        //POST /api/playlists/{id}/addTag
        [HttpPost]
        [Route("api/playlists/{id}/addTag")]
        public IHttpActionResult AddTagToPlaylist(int id, AddTagBindingModel model)
        {
            var playlist = this.Context.Playlists.Find(id);
            if (playlist == null)
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

            if (playlist.Tags.Contains(tag))
            {
                return this.BadRequest("There is already such tag");
            }

            playlist.Tags.Add(tag);
            this.Context.SaveChanges();

            return this.Ok(new
            {
                Message = string.Format("Tag #{0} was added", tag.Id)
            });
        }


        //GET /api/Playlists?startPage={start-page}&limit={page-size}& locationId={locationId}&adultContentAllowed={adultContentAllowed}
        [HttpGet]
        [Route("api/playlists/")]
        public IHttpActionResult GetPlaylistsByLocationAndAdultContent([FromUri]GetPlaylistsBindingModel model)
        {
            var playlistsAsQueryable = this.Context.Playlists.AsQueryable();

            if (model == null)
            {
                return this.BadRequest("You didn't send any filter data");
            }

            var loggedUserId = this.User.Identity.GetUserId();

            var ownPlaylists = this.Context.Playlists
                .Where(p => p.OwnerId == loggedUserId);

            if (model.LocationId.HasValue)
            {
                ownPlaylists = ownPlaylists
                    .Where(p => p.Location.Id == model.LocationId);
            }

            if (model.IsAdultContent != null || model.IsAdultContent == true)
            {
                var pagedPlaylists = ownPlaylists
                .OrderBy(p => p.Name)
                .Skip(model.StartPage * model.Limit)
                .Take(model.Limit)
                .Select(p => new PlaylistDetailsViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Author = p.Owner.UserName,
                    Tags = p.Tags.Where(t => t.IsAdultContent == false).Select(t => t.Name).ToList(),
                    Videos = p.Videos.Select(v => v.Title).ToList()
                })
                .ToList();

                return this.Ok(pagedPlaylists);
            }

            var pagedLists = ownPlaylists
            .OrderBy(p => p.Name)
            .Skip(model.StartPage * model.Limit)
            .Take(model.Limit)
            .Select(p => new PlaylistDetailsViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Author = p.Owner.UserName,
                Tags = p.Tags.Select(t => t.Name).ToList(),
                Videos = p.Videos.Select(v => v.Title).ToList()
            })
            .ToList();

            return this.Ok(pagedLists);

        }

    }
}
