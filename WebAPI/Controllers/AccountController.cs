using AngularJSAuthentication.API.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class AccountController : ApiController
    {

        IClientDetailRepository _clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);
        IAccountModelRepository _accountRepository = 
            RepositoryFactory.Create<IAccountModelRepository>(ContextTypes.EntityFramework);
        EmailSender _emailSender = new EmailSender();
        string tokenCode = "";

        [Route("api/account/register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IdentityResult> Register(AccountModel model)
        {

            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            string clientId = model.Gender == 1 ? "NCM974-" + _emailSender.Get() : "NCF974-" + _emailSender.Get();
            var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email };
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Id = clientId;
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3
            };
            IdentityResult result = manager.Create(user, model.Password);
            
            ClientDetail _clientDetail = new ClientDetail
            {
                ClientId = clientId,
                Name = model.UserName,
                Gender = model.Gender,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Country = model.Country,
                MobileNo = model.PhoneNumber,
                EmailId = model.Email,
                Jobtype = model.jobType,
                CreatedDate = DateTime.Now
            };
            _clientDetailRepo.Insert(_clientDetail);
            IHttpActionResult errorResult = GetErrorResult(result);
            if (errorResult != null)
            {
                return null;
            }
            else
            {
                _emailSender.email_send(tokenCode, model.Email, user.FirstName+ " "+ user.LastName ,user.Id);
            }
            return result;
        }
    
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        [HttpGet]
        [Route("api/GetUserClaims")]
        public AccountModel GetUserClaims()
        {
            var identityClaims = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identityClaims.Claims;
            AccountModel model = new AccountModel()
            {
                UserName = identityClaims.FindFirst("Username").Value,
                Email = identityClaims.FindFirst("Email").Value,
                FirstName = identityClaims.FindFirst("FirstName").Value,
                LastName = identityClaims.FindFirst("LastName").Value,
                LoggedOn = identityClaims.FindFirst("LoggedOn").Value
            };
            return model;
        }

        [HttpPost]
        [Route("api/user/updateProfile")]
        public IHttpActionResult UpdateProfile(ClientDetail model)
        {
            ClientDetail _clientDetail = new ClientDetail
            {
                ClientId = model.ClientId,
                Name = model.Name,
                Gender = model.Gender,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Country = model.Country,
                MobileNo = model.MobileNo,
                EmailId = model.EmailId,
                Jobtype = model.Jobtype,
            };
            _clientDetailRepo.Update(_clientDetail);
            return Ok();
        }

        [HttpGet]
        [Route("api/user/profile/{ClientId}")]
        public IHttpActionResult getProfileData(string ClientId)
        {
            return Ok(_clientDetailRepo.Find(x =>x.ClientId == ClientId));
        }

        [HttpGet]
        [Route("api/{Email}")]
        public IHttpActionResult emailVerification(string Email)
        {
            return Ok(_accountRepository.Update(new AccountModel { Email = Email, EmailConfirmed = true }));
        }

        [HttpPost]
        [Route("api/user/changePassword")]
        public IHttpActionResult ChangePassword(ChangePassword model)
        {
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            IdentityResult result = manager.ChangePassword(model.UserName, model.OldPassword, model.NewPassword);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/userNameExist")]
        public bool GetUserNameEmailIdExit(AccountModel model)
        {
            return _clientDetailRepo.Find(p => p.EmailId == model.Email).Any();
        }
    }
}
