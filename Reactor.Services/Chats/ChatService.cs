using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactor.Core.Domain.Chats;
using Reactor.Core.Domain.Messages;
using Reactor.Core.Extensions;
using Reactor.Core.Repository;

namespace Reactor.Services.Chats
{
    public class ChatService : IChatService
    {
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<Message> _messageRepository;

        public ChatService(IRepository<Message> messageRepository, IRepository<Chat> chatRepository)
        {
            _messageRepository = messageRepository;
            _chatRepository = chatRepository;
        }

        public async Task AddChatAsync(Chat chat)
        {
            await _chatRepository.AddAsync(chat);
        }

        public async Task<bool> DoesChatExistAsync(string senderId)
        {
            return await _chatRepository.Table.AnyAsync(c => c.ChatId == senderId);
        }

        public async Task AddMessageAsync(Message message)
        {
            await _messageRepository.AddAsync(message);
        }

        public async Task<(IEnumerable<Message>messages, bool loadMore)> GetChatMessagesAsync(string senderId,
            string recipientId,
            int pageIndex, int pageSize)
        {
            var query = _messageRepository.Table
                .Include(c => c.Chat)
                .Where(c =>
                    c.ChatId == senderId && c.RecipientId == recipientId ||
                    c.RecipientId == senderId && c.ChatId == recipientId)
                .OrderByDescending(c => c.CreatedOn)
                .AsQueryable();


            var loadMore = query.ShouldEnableLoadMore(pageIndex, pageSize);


            query = query.ApplyingPagination(pageIndex, pageSize);

            query = query.OrderBy(m => m.CreatedOn);


            return (await query.ToListAsync(), loadMore);
        }

        public async Task MarkAsReadAsync(string chatId)
        {
            var unReadMessages =
                await _messageRepository.Table.Where(m => m.ChatId == chatId && m.UnRead).ToListAsync();

            foreach (var message in unReadMessages)
            {
                message.MarkAsRead();
            }
            
        }

        public async Task<List<int>> GetUnReadMessageIdsAsync(string chatId)
        {
            return await _messageRepository.Table.Where(m => m.ChatId == chatId && m.UnRead).Select(m => m.Id)
                .ToListAsync();
        }
    }
}