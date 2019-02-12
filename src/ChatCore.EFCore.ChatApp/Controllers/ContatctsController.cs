using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ChatCore.ChatApp.ViewModels;
using System.Linq;

namespace ChatCore.ChatApp.Controllers
{
    [Authorize]
    [CustomRoute("[controller]")]
    [ApiController]
    public class ContactsController : CustomControllerBase
    {

        // GET: ChatApp/Contacts
        [HttpGet]
        public async Task<IEnumerable<ContactViewModel>> GetAllContacts()
        {
            return Mapper.Map<IEnumerable<ContactViewModel>>(await ChatContext.ContactManager.GetAllContactsAsync());
        }

        // GET: ChatApp/Contacts
        [HttpGet("{contactId}")]
        public async Task<ContactViewModel> GetContact([FromRoute]string contactId)
        {
            return Mapper.Map<ContactViewModel>((await ChatContext.ContactManager.GetAllContactsAsync()).FirstOrDefault(u=>u.Id==contactId));
        }

        [HttpDelete]
        [Route("Remove/{contactId}")]
        public async Task<IActionResult> Remove([FromRoute]string contactId)
        {
            var result = await ChatContext.ContactManager.RemoveContactByIdAsync(contactId);
            if (result)
                return Ok();
            return BadRequest();
        }

    }
}