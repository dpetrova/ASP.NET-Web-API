using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Tests.Mocks
{
    using Data.Models;
    using Data.Repositories;
    using Data.UnitOfWork;
    using Microsoft.AspNet.Identity;

    public class MessagesDataMock : IMessagesUnitOfWork
    {
        private GenericRepositoryMock<User> usersMock = new GenericRepositoryMock<User>();
        private GenericRepositoryMock<Channel> channelsMock = new GenericRepositoryMock<Channel>();
        private GenericRepositoryMock<ChannelMessage> channelMessagesMock = new GenericRepositoryMock<ChannelMessage>();
        private GenericRepositoryMock<UserMessage> userMessagesMock = new GenericRepositoryMock<UserMessage>();
        private GenericUserStoreMock<User> userStoreMock = new GenericUserStoreMock<User>();

        public bool ChangesSaved { get; set; }

        public IRepository<User> Users
        {
            get { return this.usersMock; }
        }

        public IRepository<Channel> Channels
        {
            get { return this.channelsMock; }
        }

        public IRepository<ChannelMessage> ChannelMessages
        {
            get { return this.channelMessagesMock; }
        }

        public IRepository<UserMessage> UserMessages
        {
            get { return this.userMessagesMock; }
        }

        public IUserStore<User> UserStore
        {
            get { return this.userStoreMock; }
        }

        public void SaveChanges()
        {
            this.ChangesSaved = true;
        }
    }
}
