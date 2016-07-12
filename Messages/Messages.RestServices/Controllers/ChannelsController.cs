using System;
using System.Collections.Generic;
//using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Messages.Data;
using Messages.Data.Models;

namespace Messages.RestServices.Controllers
{
    using Data.UnitOfWork;
    using Models;

    public class ChannelsController : ApiController
    {
        //private MessagesDbContext db = new MessagesDbContext();

        private IMessagesUnitOfWork data;

        public ChannelsController() : this (new MessagesUnitOfWork())
        {
        }

        public ChannelsController(IMessagesUnitOfWork data)
        {
            this.data = data;
        }


        // GET: api/Channels
        public IHttpActionResult GetChannels()
        {
            var channels = data.Channels.All()
               .OrderBy(c => c.Name)
               .Select(c => new ChannelViewModel()
               {
                   Id = c.Id,
                   Name = c.Name
               });
            return this.Ok(channels);
        }


        // GET: api/Channels/{id}
        [ResponseType(typeof(Channel))]
        public IHttpActionResult GetChannel(int id)
        {
            var channel = data.Channels.All()
                .Where(c => c.Id == id)
                .Select(c => new ChannelViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefault();

            if (channel == null)
            {
                return NotFound();
            }

            return Ok(channel);
        }

        //another way:
        // GET: api/Channels/{id}
        //[ResponseType(typeof(Channel))]
        //public IHttpActionResult GetChannel(int id)
        //{
        //    var channel = db.Channels.Find(id);
        //    if (channel == null)
        //    {
        //        return NotFound();
        //    }
        //    var channelToReturn = new ChannelViewModel()
        //    {
        //        Id = channel.Id,
        //        Name = channel.Name
        //    };

        //    return Ok(channelToReturn);
        //}

        

        // PUT: api/Channels/{id}
        [ResponseType(typeof(void))]
        public IHttpActionResult PutChannel(int id, ChannelBindingModel channelInputData)
        {
            //check for missing or invalid channel data (e.g. empty channel name)
            if (channelInputData == null)
            {
                return BadRequest("Invalid channel data.");
            }

            //makes validation by attributes
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var channel = data.Channels.Find(id);
            if (channel == null)
            {
                return this.NotFound();
            }

            //check for duplicated channel name
            var duplicatedChannel = data.Channels.All()
                .Any(c => c.Name == channelInputData.Name && c.Id != channel.Id);
            if (duplicatedChannel)
            {
                return this.Conflict();
            }

            channel.Name = channelInputData.Name;
            data.Channels.Update(channel);
            data.SaveChanges();

            return this.Ok(new {Message = "Channel #" + channel.Id + " edited successfully."});

        }

        // POST: api/Channels
        [ResponseType(typeof(Channel))]
        public IHttpActionResult PostChannel(ChannelBindingModel channelInputData)
        {
            //check for missing or invalid channel data (e.g. empty channel name)
            if (channelInputData == null)
            {
                return BadRequest();
            }

            //it makes validation by attributes
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check for duplicated channel name (when a channel with the specified name already exists in the DB)
            if (data.Channels.All().Any(c => c.Name == channelInputData.Name))
            {
                return this.Conflict();
            }

            var channel = new Channel()
            {
                Name = channelInputData.Name
            };

            data.Channels.Add(channel);
            data.SaveChanges();

            var outputChannel = new ChannelViewModel()
            {
                Id = channel.Id,
                Name = channel.Name
            };

            //it will return a header "Location" holding the URL of the created channel 
            //(e.g. Location: http://localhost:7777/api/channels/8
            //another way: return this.Created(...)
            return CreatedAtRoute("DefaultApi", new { id = channel.Id }, outputChannel);
        }


        // DELETE: api/Channels/{id}
        [ResponseType(typeof(Channel))]
        public IHttpActionResult DeleteChannel(int id)
        {
            var channel = data.Channels.Find(id);
            if (channel == null)
            {
                return NotFound();
            }

            //Returned when a non-empty channel failed to delete
            if (channel.ChannelMessages.Any())
            {
                return this.Content(HttpStatusCode.Conflict,
                    new { Message = "Cannot delete channel #" + id + " because it is not empty." });
            }

            data.Channels.Remove(channel);
            data.SaveChanges();

            return Ok(new { Message = "Channel #" + channel.Id + " deleted." });
        }
    }
}