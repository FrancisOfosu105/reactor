using System;
using Reactor.Core.Domain.Chats;

namespace Reactor.Core.Domain.Messages
{
    public class Message : EntityBase
    {
        public string Content { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string RecipientId { get; set; }

        public Chat Chat { get; set; }

        public string ChatId { get; set; }
    }
}