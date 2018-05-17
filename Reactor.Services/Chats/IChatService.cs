using System.Collections.Generic;
using System.Threading.Tasks;
using Reactor.Core.Domain.Chats;
using Reactor.Core.Domain.Messages;

namespace Reactor.Services.Chats
{
    public interface IChatService
    {
        Task AddChatAsync(Chat chat);
        
        Task<bool> DoesChatExistAsync(string senderId);
        
        Task AddMessageAsync(Message message);
        
        Task<(IEnumerable<Message> messages, bool loadMore)> GetChatMessagesAsync(string senderId, string recipientId,int pageIndex, int pageSize);

    }
}