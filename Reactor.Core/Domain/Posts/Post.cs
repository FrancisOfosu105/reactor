using System;
using System.Collections.Generic;
using Reactor.Core.Domain.Photos;
using Reactor.Core.Domain.Users;

namespace Reactor.Core.Domain.Posts
{
    public class Post : EntityBase
    {
        public string Content { get; set; }

        public User CreatedBy { get; set; }        

        public string CreatedById { get; set; }    

        public DateTime CreatedOn { get; set; }
        
        public ICollection<Photo> Photos { get; set; }

        public Post()
        {
            Photos = new List<Photo>();
        }
    }
}