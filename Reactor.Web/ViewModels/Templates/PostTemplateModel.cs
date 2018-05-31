using System.Collections.Generic;
using Reactor.Core.Domain.Posts;

namespace Reactor.Web.ViewModels.Templates
{
    public class PostTemplateModel
    {
        public IEnumerable<Post> Posts{ get; set; }

        public bool LoadMore { get; set; }

    }
}