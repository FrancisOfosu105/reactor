using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Reactor.Core;
using Reactor.Core.Domain.Chats;
using Reactor.Core.Domain.Messages;
using Reactor.Core.Domain.Users;
using Reactor.Services.Chats;
using Reactor.Services.Users;
using Reactor.Services.ViewRender;
using Reactor.Web.ViewModels.Chat;

namespace Reactor.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        #region fields

        private readonly IUserService _userService;
        private readonly IViewRenderService _renderService;
        private readonly ChatConnection _chatConnection;
        private readonly IChatService _chatService;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region ctor

        public ChatHub(
            IUserService userService,
            IViewRenderService renderService,
            ChatConnection chatConnection,
            IChatService chatService,
            IUnitOfWork unitOfWork
        )
        {
            _userService = userService;
            _renderService = renderService;
            _chatConnection = chatConnection;
            _chatService = chatService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region methods

        public async Task SendMessage(Message message)
        {
            var recipient = await _userService.GetUserByIdAsync(message.RecipientId);

            var sender = await _userService.GetUserByUserNameAsync(Context.User.Identity.Name);

            var senderDetail = GetUserDetail(sender.Id);

            if (senderDetail != null)
            {
                if (!await _chatService.DoesChatExistAsync(sender.Id))
                {
                    var chat = new Chat
                    {
                        ChatId = sender.Id
                    };

                    await _chatService.AddChatAsync(chat);
                }

                message.ChatId = sender.Id;

                await _chatService.AddMessageAsync(message);

                await _unitOfWork.CompleteAsync();

                var senderChatModel = PrepareChatModel(message, sender, MessagePosition.Right, "me");

                var recipientChatModel = PrepareChatModel(message, sender, MessagePosition.Left);

                var senderMessageTemplate = await PrepareMessageTemplate(senderChatModel);

                var recipientMessageTemplate = await PrepareMessageTemplate(recipientChatModel);

                //Send message to the recipient if and only if is online
                if (GetUserDetail(recipient.Id) != null)
                {
                    await Clients.User(recipient.Id)
                        .SendAsync("addChatMessage", recipientMessageTemplate, message.Id, sender.Id);
                }

                //Caller
                await Clients.User(sender.Id).SendAsync("addChatMessage", senderMessageTemplate, message.Id, null);
            }
        }


        public async Task<(string messages, bool loadMore)> GetChatHistory(string recipientId, int pageIndex,
            int pageSize)
        {
            var sender = await _userService.GetUserByUserNameAsync(Context.User.Identity.Name);

            var (messages, loadMore) =
                await _chatService.GetChatMessagesAsync(sender.Id, recipientId, pageIndex, pageSize);

            var recipient = await _userService.GetUserByIdAsync(recipientId);

            var messageTemplates = new List<string>();

            foreach (var message in messages)
            {
                if (message.ChatId == sender.Id)
                {
                    var senderChatModel = new ChatModel
                    {
                        CreatedOn = message.CreatedOn,
                        FullName = "me",
                        Message = message.Content,
                        Position = MessagePosition.Right,
                        ProfilePicture = sender.GetProfilePicture(),
                        IsRead = message.IsRead
                    };

                    messageTemplates.Add(await PrepareMessageTemplate(senderChatModel));
                }
                else
                {
                    var recipientChatModel = new ChatModel
                    {
                        CreatedOn = message.CreatedOn,
                        FullName = recipient.FullName,
                        Message = message.Content,
                        Position = MessagePosition.Left,
                        ProfilePicture = recipient.GetProfilePicture(),
                        IsRead = message.IsRead
                    };

                    messageTemplates.Add(await PrepareMessageTemplate(recipientChatModel));
                }
            }


            return (string.Join("", messageTemplates), loadMore);
        }

        public override async Task OnConnectedAsync()
        {
            var newconnectedUser = await _userService.GetUserByUserNameAsync(Context.User.Identity.Name);

            if (GetUserDetail(newconnectedUser.Id) == null)
            {
                _chatConnection.AddUserDetail(new UserDetail
                {
                    UserId = newconnectedUser.Id
                });
            }

            var connectedFriends = await GetConnectedFriendsDetailAsync(newconnectedUser.Id);

            foreach (var connectedFriend in connectedFriends)
            {
                await Clients.User(connectedFriend.UserId)
                    .SendAsync("onlineContact", newconnectedUser.Id);

                await Clients.User(newconnectedUser.Id)
                    .SendAsync("onlineContact", connectedFriend.UserId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var disconnectedUser = await _userService.GetUserByUserNameAsync(Context.User.Identity.Name);

            if (GetUserDetail(disconnectedUser.Id) != null)
                _chatConnection.RemoveUserDetail(GetUserDetail(disconnectedUser.Id));

            var connectedFriends = await GetConnectedFriendsDetailAsync(disconnectedUser.Id);

            foreach (var connectedFriend in connectedFriends)
            {
                await Clients.User(connectedFriend.UserId)
                    .SendAsync("offlineContact", disconnectedUser.Id);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task MarkMessagesAsRead(string recipientId, int? messageId)
        {
            
            var unReadMessageIds = await _chatService.GetUnReadMessageIdsAsync(recipientId);

            await _chatService.MarkAsReadAsync(recipientId);

            await _unitOfWork.CompleteAsync();

            if (messageId != null)
            {
                var sender = await _userService.GetUserByUserNameAsync(Context.User.Identity.Name);

                await Clients.User(recipientId).SendAsync("messageSeen", messageId);
                await Clients.User(sender.Id).SendAsync("messageSeen", messageId);
            }

            if (unReadMessageIds.Any())
            {
                foreach (var id in unReadMessageIds)
                    await Clients.User(recipientId).SendAsync("messageSeen", id);
            }
        }

        
        #endregion

        #region helpers

        private UserDetail GetUserDetail(string userId)
        {
            return _chatConnection.UserDetails.SingleOrDefault(u => u.UserId == userId);
        }

        private async Task<string> PrepareMessageTemplate(ChatModel model)
        {
            return await _renderService.RenderViewToStringAsync("Templates/_Chat", model);
        }

        private async Task<IEnumerable<UserDetail>> GetConnectedFriendsDetailAsync(string userId)
        {
            var user = await _userService.GetUserWithFriendsAsync(userId);

            var friends = user.ApprovedFriends();

            var friendIds = (from friend in friends
                select friend.RequestedBy.UserName == Context.User.Identity.Name
                    ? friend.RequestedTo.Id
                    : friend.RequestedBy.Id).ToList();

            var connectedFriends = _chatConnection.UserDetails.Where(u => friendIds.Any(id => id == u.UserId));

            return connectedFriends;
        }

        private static ChatModel PrepareChatModel(Message message, User sender, MessagePosition position,
            string senderName = null)
        {
            return new ChatModel
            {
                CreatedOn = message.CreatedOn,
                FullName = senderName ?? sender.FullName,
                Message = message.Content,
                Position = position,
                ProfilePicture = sender.GetProfilePicture(),
                MessageId = message.Id
            };
        }

        #endregion
    }
}