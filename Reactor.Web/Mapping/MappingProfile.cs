using AutoMapper;
using Reactor.Core.Domain.Users;
using Reactor.Web.ViewModels.SettingsViewModel;

namespace Reactor.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, BasicViewModel>()
                .ForMember(u => u.ProfileCoverPicture, opts => opts.Ignore())
                .ForMember(u => u.ProfilePicture, opts => opts.Ignore());

            CreateMap<UserSetting, NotificationViewModel>();

            CreateMap<BasicViewModel, User>()
                .ForMember(u => u.ProfilePictureUrl, opts => opts.Ignore())
                .ForMember(u => u.ProfileCoverPictureUrl, opts => opts.Ignore());
                

            CreateMap<NotificationViewModel, UserSetting>();
        }
    }
}