using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Messages.RestServices.Controllers
{
    using System.Web.Http.Description;
    using Data;
    using Data.Models;
    using Data.UnitOfWork;
    using Microsoft.AspNet.Identity;
    using Models;

    public class ChannelMessagesController : ApiController
    {
        //private MessagesDbContext db = new MessagesDbContext();

        private IMessagesUnitOfWork data;

        public ChannelMessagesController() : this (new MessagesUnitOfWork())
        {
        }

        public ChannelMessagesController(IMessagesUnitOfWork data)
        {
            this.data = data;
        }

        // GET: api/channel-messages/{channelName}
        //[Route("api/channel-messages/{channelName}")]
        //[ResponseType(typeof(ChannelMessage))]
        //public IHttpActionResult GetChannelMessages(string channelName)
        //{
        //    var channel = db.Channels
        //        .FirstOrDefault(c => c.Name == channelName);

        //    if (channel == null)
        //    {
        //        return NotFound();
        //    }

        //    var channelMessages = channel.ChannelMessages
        //        .OrderByDescending(m => m.DateSent)
        //        .ThenByDescending(m => m.Id)
        //        .Select(m => new MessageViewModel()
        //        {
        //            Id = m.Id,
        //            Text = m.Text,
        //            DateSent = m.DateSent,
        //            Sender = (m.Sender != null) ? m.Sender.UserName : null
        //        });

        //    return Ok(channelMessages);
        //}


        // GET: api/channel-messages/{channel}?limit={limit}
        [Route("api/channel-messages/{channelName}")]
        [ResponseType(typeof(ChannelMessage))]
        public IHttpActionResult GetChannelMessages(string channelName, [FromUri]string limit = null) //limit is optional parameter
        {
            var channel = data.Channels.All()
                .FirstOrDefault(c => c.Name == channelName);

            if (channel == null)
            {
                return NotFound();
            }

            var channelMessages = channel.ChannelMessages
                .OrderByDescending(m => m.DateSent)
                .ThenByDescending(m => m.Id)
                .Select(m => new MessageViewModel()
                {
                    Id = m.Id,
                    Text = m.Text,
                    DateSent = m.DateSent,
                    Sender = (m.Sender != null) ? m.Sender.UserName : null
                });

            if (limit != null)
            {
                int maxCount = 0;
                int.TryParse(limit, out maxCount);
                if (maxCount < 1 || maxCount > 1000)
                {
                    return this.BadRequest("Limit should be integer in range [1..1000]");
                }
                channelMessages = channelMessages.Take(maxCount);
            }

            return Ok(channelMessages);
        }


        // POST: api/channel-messages/{channel}
        [Route("api/channel-messages/{channelName}")]
        [ResponseType(typeof(ChannelMessage))]
        public IHttpActionResult PostChannelMessages(string channelName, MessageBindingModel messageInputData)
        {
            if (messageInputData == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var channel = data.Channels.All()
                .FirstOrDefault(c => c.Name == channelName);

            if (channel == null)
            {
                return NotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.data.Users.Find(currentUserId);
            
            var message = new ChannelMessage()
            {
                Text = messageInputData.Text,
                Channel = channel,
                DateSent = DateTime.Now,
                Sender = currentUser
            };
           
            data.ChannelMessages.Add(message);
            data.SaveChanges();

            if (message.Sender == null)
            {
                return this.Ok(
                    new
                    {
                        message.Id,
                        Message = "Anonymous message sent successfully to channel " + channelName + "."
                    }
                );
            }
            return this.Ok(
                new
                {
                    message.Id,
                    Sender = message.Sender.UserName,
                    Message = "Message sent successfully to channel " + channelName + "."
                }
            );
        }
    }
}
