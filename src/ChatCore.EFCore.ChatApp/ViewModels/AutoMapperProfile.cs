using AutoMapper;
using ChatApp.Abstractions;
using ChatCore.EFCore;
using ChatCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace ChatCore.ChatApp.ViewModels
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<TSource, TDestination>();
            CreateMap<User, UserViewModel>()
                    .ForMember(user => user.Contacts, x => x.Ignore())
                   // m => m.MapFrom(user => user.FriendsFrom.Select(f=>f.ToUser).Concat(user.FriendsTo.Select(f=>f.FromUser))));
                    .ReverseMap();
            //CreateMap<UserViewModel, User>();

            CreateMap<Message, MessageViewModel>().ReverseMap();

            CreateMap<Attachment, AttachmentViewModel>().ReverseMap();

            CreateMap<User, ContactViewModel>().ReverseMap();

            CreateMap<FriendRequest, FriendRequestViewModel>().ReverseMap();

            CreateMap<ChatViewModel, Chat>();
            CreateMap<Chat, ChatViewModel>()
                //.ForMember(cView => cView.UnReadMessagesCoun, m => m.MapFrom(c => ChatToViewUnReadCount(c)))
                .ForMember(cView => cView.Users, m => m.MapFrom(c => c.UsersChats.Select(uc => uc.User).ToArray()));
            

        }

        private int ChatToViewUnReadCount(Chat chat)
        {
            //var provider = Helper.GetServiceProvider();
            //var options = ((IOptions<ChatCoreEntityFrameworkCoreOptions>)provider.GetService(typeof(IOptions<ChatCoreEntityFrameworkCoreOptions>))).Value;
            //var identityProvider = (IIdentityProvider<string>)provider.GetService(typeof(IIdentityProvider<string>));
            //var Db = (DbContext)provider.GetService(options.DbContextType);

            //var userChat= Db.Set<UsersChats>().SingleOrDefault(uc => uc.ChatId == chat.Id && uc.User.AccountId == identityProvider.CurrentUserId );

            //return userChat?.UnReadMessagesCount ?? 0;
            return 0;
        }

    }
    ////Don't forget to add Helper class to services
    //public class Helper
    //{
    //    private static IServiceProvider serviceProvider;
    //    public Helper(IServiceProvider _serviceProvider)
    //    {
    //        serviceProvider= _serviceProvider;
    //    }
    //    public static object GeService(Type serviceType)
    //    {
    //        return  serviceProvider.GetService(serviceType);
    //    }
    //    public static IServiceProvider GetServiceProvider() => serviceProvider;
    //}
}
