using AutoMapper;
using ChatApp.Api.Models;
using ChatCore.EFCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ChatApp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            //SeedData(host);

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>();

        public static void SeedData(IWebHost host)
        {
            var context = host.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var chatContext = host.Services.CreateScope().ServiceProvider.GetRequiredService<ChatCoreContext>();
            var userManager = host.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<User>>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            if (!context.Users.Any())
            {
                User user = new User()
                {
                    /*
                        confirmPassword: "password"
                        email: "aslan@email.com"
                        password: "password"
                        rememberMe: false
                        userName: "aslan"
                    */
                    Email = "aslan@email.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "Aslan",

                };
                User user1 = new User()
                {
                    Email = "aslan1@email.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "Aslan1",

                };
                 userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
                 userManager.CreateAsync(user1, "password").GetAwaiter().GetResult();
                 context.SaveChanges();
                var _user = new ChatCore.Models.User()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = user.Email,
                    AccountId = user.Id,
                    UserName = user.UserName
                };
                var _user1 = new ChatCore.Models.User()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = user1.Email,
                    AccountId = user1.Id,
                    UserName = user1.UserName
                };
                chatContext.AccountManager.CreateUserAsync(_user,user.Id).GetAwaiter().GetResult();
                chatContext.AccountManager.CreateUserAsync(_user1, user1.Id).GetAwaiter().GetResult();
            }
        }

    }


}
