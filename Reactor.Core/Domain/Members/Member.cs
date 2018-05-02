using Microsoft.AspNetCore.Identity;

namespace Reactor.Core.Domain.Members
{
    public class Member : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
      
    }
}