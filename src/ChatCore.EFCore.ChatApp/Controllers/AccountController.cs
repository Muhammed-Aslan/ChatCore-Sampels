using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using ChatCore.ChatApp.ViewModels;
using ChatCore.Models;
using ChatApp.Abstractions;
using System.Collections.Generic;
using ChatCore.EFCore.ChatApp.Services;

namespace ChatCore.ChatApp.Controllers
{
    [Authorize]
    [CustomRoute("[controller]")]
    public class AccountController : CustomControllerBase
    {
        private readonly IIdentityProvider<string> _IdentityProvider;
        public AccountController(IIdentityProvider<string> IdentityProvider)
        {
            _IdentityProvider = IdentityProvider;
        }
        //ChatApp/Account/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserViewModel userView)
        {
            if(ChatContext.CurrentUser == null)// in future check if the user is admin or not has chat account
            {
                if (ModelState.IsValid)
                {
                    var newUser = Mapper.Map<User>(userView);
                    newUser.AccountId = _IdentityProvider.CurrentUserId;
                    var user = await ChatContext.AccountManager.CreateUserAsync(newUser, newUser.AccountId);
                    return Ok(Mapper.Map<UserViewModel>(user));
                }
                return BadRequest(ModelState);
            }
            return BadRequest(new { error = "Invaild registrations" });
        }

        [HttpGet("me")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<UserViewModel> GetCurrentUser()
        {
            var x = Mapper.Map<UserViewModel>(await ChatContext.AccountManager.GetUserByAccountIdAsync(_IdentityProvider.CurrentUserId));
            return x;
        }

        [HttpPost("me")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<UserViewModel> UpdateCurrentUser([FromBody]UserViewModel user)
        {
            var u = Mapper.Map<User>(user);
            return Mapper.Map<UserViewModel>(await ChatContext.AccountManager.UpdateUserAsync(u));
        }

        [HttpGet("userName/{userName}")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<UserViewModel> GetUserByUserName([FromRoute]string userName)
        {
            return Mapper.Map<UserViewModel>(await ChatContext.AccountManager.GetUserByUserNameAsync(userName));
        }

        [HttpGet("userId/{id}")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<UserViewModel> GetUserByUserId([FromRoute]string id)
        {
            return Mapper.Map<UserViewModel>(await ChatContext.AccountManager.GetUserByIdAsync(id));
        }

        [HttpGet]
        [Route("{page}/{pageSize}/{userName}")]
        public async Task<IEnumerable<ContactViewModel>> GetUsers([FromRoute]string userName,int page = 1, int pageSize = 5)
        {
            var users = await ChatContext.AccountManager.GetMatchUsersAsync(page, pageSize, userName);
            return Mapper.Map<IEnumerable<ContactViewModel>>(users);
        }

    }
}