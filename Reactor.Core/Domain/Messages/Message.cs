using System;
using System.ComponentModel.DataAnnotations.Schema;
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

        public bool IsRead { get; set; }

        [NotMapped] public bool UnRead => IsRead != true;

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}