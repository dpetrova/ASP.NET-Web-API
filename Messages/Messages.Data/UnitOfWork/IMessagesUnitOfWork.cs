using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Data.UnitOfWork
{
    using Microsoft.AspNet.Identity;
    using Models;
    using Repositories;

    public interface IMessagesUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<Channel> Channels { get; }
        IRepository<ChannelMessage> ChannelMessages { get; }
        IRepository<UserMessage> UserMessages { get; }
        IUserStore<User> UserStore { get; }
        void SaveChanges();
    }
}
