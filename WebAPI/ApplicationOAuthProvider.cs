using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        IClientDetailRepository _clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            var user = await manager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("Please check username and password");
            }
            else
            {
                var clientDetailRepo = _clientDetailRepo.Find(x => x.ClientId == user.Id).FirstOrDefault();
                if (!clientDetailRepo.EmailConfirmed)
                {
                    context.SetError("Please verify your email address");
                }
                else
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("UserId", user.Id));
                    identity.AddClaim(new Claim("Username", user.UserName));
                    identity.AddClaim(new Claim("Email", user.Email));
                    identity.AddClaim(new Claim("FirstName", user.FirstName));
                    identity.AddClaim(new Claim("LastName", user.LastName));
                    identity.AddClaim(new Claim("LoggedOn", DateTime.Now.ToString()));
                    identity.AddClaim(new Claim("PhoneNo", user.PhoneNumber == null? " " : user.PhoneNumber));
                    context.Validated(identity);
                }
            }
        }
    }
}