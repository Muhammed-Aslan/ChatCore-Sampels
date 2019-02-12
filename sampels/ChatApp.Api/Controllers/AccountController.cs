using System.Threading.Tasks;
using ChatApp.Api.Models;
using ChatApp.Api.Models.Account;
using ChatCore.EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.EntityFrameworkCore;
namespace ChatApp.Api.Controllers
{
    [Authorize]
    [Route("[Controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ChatCoreContext chatContext;

        public AccountController( UserManager<User> userManager,ApplicationDbContext applicationDbContext ,ChatCoreContext chatContext)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            this.chatContext = chatContext;
        }
        
        // POST: /Account/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    return StatusCode(StatusCodes.Status409Conflict);
                }

                user = new User { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Ok(model);
                }
                AddErrors(result);
            }
            
            return BadRequest(ModelState);
        }
        
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody] string userId)
        {
            var x =await chatContext.AccountManager.DeleteUserAsync(chatContext.CurrentUser.AccountId);
            return Ok(x);
        }
        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion
    }
}
