﻿using System;

namespace Reactor.Web.ViewModels.Chat
{
    public class ChatModel
    {
        public string Message { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public string ProfilePicture { get; set; }

        public string FullName { get; set; }

        public MessagePosition Position { get; set; }

        public bool IsRead { get; set; }
        
        public int  MessageId { get; set; }
    }

    public enum MessagePosition
    {
        Left, Right
    }
}