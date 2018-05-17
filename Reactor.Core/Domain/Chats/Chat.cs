using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Reactor.Core.Domain.Messages;

namespace Reactor.Core.Domain.Chats
{
    public class Chat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ChatId { get; set; }

        public ICollection<Message> Messages { get; set; }

        public Chat()
        {
            Messages = new List<Message>();
        }
    }
}