using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using ChatApp.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Mvc.Internal;
using OpenIddict.Server;

namespace ChatApp.Api.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthorizationController(
            IOptions<IdentityOptions> identityOptions,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _identityOptions = identityOptions;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange([ModelBinder(typeof(OpenIddictMvcBinder))] OpenIdConnectRequest request)
        {
            if (request.IsPasswordGrantType())
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null)
                {
                    return BadRequest(new OpenIdConnectResponse
                    {
                        Error = OpenIdConnectConstants.Errors.InvalidGrant,
                        ErrorDescription = "Please check that your email and password is correct"
                    });
                }

                // Validate the username/password parameters and ensure the account is not locked out.
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

                // Ensure the user is not already locked out.
                if (result.IsLockedOut)
                {
                    return BadRequest(new OpenIdConnectResponse
                    {
                        Error = OpenIdConnectConstants.Errors.InvalidGrant,
                        ErrorDescription = "The specified user account has been suspended"
                    });
                }

                // Reject the token request if two-factor authentication has been enabled by the user.
                if (result.RequiresTwoFactor)
                {
                    return BadRequest(new OpenIdConnectResponse
                    {
                        Error = OpenIdConnectConstants.Errors.InvalidGrant,
                        ErrorDescription = "Invalid login procedure"
                    });
                }

                // Ensure the user is allowed to sign in.
                if (result.IsNotAllowed)
                {
                    return BadRequest(new OpenIdConnectResponse
                    {
                        Error = OpenIdConnectConstants.Errors.InvalidGrant,
                        ErrorDescription = "The specified user is not allowed to sign in"
                    });
                }

                if (!result.Succeeded)
                {
                    return BadRequest(new OpenIdConnectResponse
                    {
                        Error = OpenIdConnectConstants.Errors.InvalidGrant,
                        ErrorDescription = "Please check that your email and password is correct"
                    });
                }

                // Create a new authentication ticket.
                var ticket = await CreateTicketAsync(request, user);

                return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
            }
            else if (request.IsRefreshTokenGrantType())
            {
                // Retrieve the claims principal stored in the refresh token.
                var info = await HttpContext.AuthenticateAsync(OpenIddictServerDefaults.AuthenticationScheme);

                // Retrieve the user profile corresponding to the refresh token.
                // Note: if you want to automatically invalidate the refresh token
                // when the user password/roles change, use the following line instead:
                // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
                var user = await _userManager.GetUserAsync(info.Principal);
                if (user == null)
                {
                    return BadRequest(new OpenIdConnectResponse
                    {
                        Error = OpenIdConnectConstants.Errors.InvalidGrant,
                        ErrorDescription = "The refresh token is no longer valid"
                    });
                }

                // Ensure the user is still allowed to sign in.
                if (!await _signInManager.CanSignInAsync(user))
                {
                    return BadRequest(new OpenIdConnectResponse
                    {
                        Error = OpenIdConnectConstants.Errors.InvalidGrant,
                        ErrorDescription = "The user is no longer allowed to sign in"
                    });
                }

                // Create a new authentication ticket, but reuse the properties stored
                // in the refresh token, including the scopes originally granted.
                var ticket = await CreateTicketAsync(request, user);

                return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
            }

            return BadRequest(new OpenIdConnectResponse
            {
                Error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            });
        }

        private async Task<AuthenticationTicket> CreateTicketAsync(OpenIdConnectRequest request, User user)
        {
            // Create a new ClaimsPrincipal containing the claims that
            // will be used to create an id_token, a token or a code.
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            // Create a new authentication ticket holding the user identity.
            var ticket = new AuthenticationTicket(principal,
                new AuthenticationProperties(),
                OpenIddictServerDefaults.AuthenticationScheme);
            
            
            ticket.SetScopes(new[]
                      {
                          OpenIdConnectConstants.Scopes.OpenId,
                          OpenIdConnectConstants.Scopes.Profile,
                          OpenIdConnectConstants.Scopes.OfflineAccess
                      });


            var identity = principal.Identity as ClaimsIdentity;
            if (ticket.HasScope(OpenIdConnectConstants.Scopes.Profile) && request.IsPasswordGrantType())
            {
                if (!string.IsNullOrWhiteSpace(user.Email))
                    identity.AddClaim(OpenIdConnectConstants.Claims.Email, user.Email);
                if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                    identity.AddClaim(OpenIdConnectConstants.Claims.PhoneNumber, user.PhoneNumber);

            }

            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            foreach (var claim in ticket.Principal.Claims)
            {
                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                if (claim.Type == _identityOptions.Value.ClaimsIdentity.SecurityStampClaimType)
                {
                    continue;
                }

                var destinations = new List<string>
                          {
                              OpenIdConnectConstants.Destinations.IdentityToken
                          };

                // Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
                // The other claims will only be added to the access_token, which is encrypted when using the default format.
                if (claim.Type == OpenIdConnectConstants.Claims.Subject  ||
                    claim.Type == OpenIdConnectConstants.Claims.Name)
                {
                    destinations.Add(OpenIdConnectConstants.Destinations.AccessToken);
                }

                claim.SetDestinations(destinations);
            }

            return ticket;
        }
    }
}