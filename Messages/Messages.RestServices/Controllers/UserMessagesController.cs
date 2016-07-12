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

    public class UserMessagesController : ApiController
    {
        //private MessagesDbContext db = new MessagesDbContext();

        private IMessagesUnitOfWork data;

        public UserMessagesController() : this (new MessagesUnitOfWork())
        {
        }

        public UserMessagesController(IMessagesUnitOfWork data)
        {
            this.data = data;
        }


        // GET: api/user/personal-messages
        [Route("api/user/personal-messages")]
        [Authorize]
        public IHttpActionResult GetPersonalMessages()
        {
            var currentUserId = User.Identity.GetUserId();
            //if (currentUserId == null)
            //{
            //    return this.Unauthorized();
            //}

            var userMessages = data.UserMessages.All()
                .Where(m => m.RecipientId == currentUserId)
                .OrderByDescending(m => m.DateSent)
                .ThenByDescending(m => m.Id)
                .Select(m => new MessageViewModel()
                {
                    Id = m.Id,
                    Text = m.Text,
                    DateSent = m.DateSent,
                    Sender = m.Sender == null ? null: m.Sender.UserName
                });

            return this.Ok(userMessages);
        }


        // POST: api/user/personal-messages
        [HttpPost]
        [Route("api/user/personal-messages")]
        public IHttpActionResult SendPersonalMessage(UserMessageBindingModel messageData)
        {
            if (messageData == null)
            {
                return BadRequest("Missing message data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipientUser = this.data.Users.All()
                .FirstOrDefault(u => u.UserName == messageData.Recipient);
            if (recipientUser == null)
            {
                return BadRequest("Recipient user " + messageData.Recipient + " does not exists.");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = this.data.Users.Find(currentUserId);

            var message = new UserMessage()
            {
                Text = messageData.Text,
                DateSent = DateTime.Now,
                Sender = currentUser,
                Recipient = recipientUser
            };
            data.UserMessages.Add(message);
            data.SaveChanges();

            if (message.Sender == null)
            {
                return this.Ok(
                    new
                    {
                        message.Id,
                        Message = "Anonymous message sent successfully to user " + recipientUser.UserName + "."
                    }
                );
            }

            return this.Ok(
                new
                {
                    message.Id,
                    Sender = message.Sender.UserName,
                    Message = "Message sent successfully to user " + recipientUser.UserName + "."
                }
            );
        }
    }
}
