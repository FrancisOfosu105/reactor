using System;
using Reactor.Core.Domain.Users;
using Reactor.Core.Repository;
using Reactor.Data.EfContext;

namespace Reactor.Data.Repository
{
    public class UserSettingRepository : Repository<UserSetting>, IUserSettingRepository
    {
        public UserSettingRepository(ReactorDbContext context) 
            : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
        }
    }
}