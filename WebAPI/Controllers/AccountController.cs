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
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class AccountController : ApiController
    {

        IClientDetailRepository _clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);
        EmailSender _emailSender = new EmailSender();
        string tokenCode = "";

        [Route("api/account/register")]
        [HttpPost]
        [AllowAnonymous]
        public string Register(AccountModel model)
        {

            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            string clientId = model.Gender == 1 ? "NCM974-" + _emailSender.Get() : "NCF974-" + _emailSender.Get();
            var user = new ApplicationUser() {
                UserName = model.UserName, Email = model.Email
            };
            user.FirstName = model.FirstName;
            user.PhoneNumber = model.PhoneNumber;
            user.LastName = model.LastName;
            user.Id = clientId;

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
                MobileNo = Convert.ToInt32(model.PhoneNumber),
                EmailId = model.Email,
                Jobtype = model.jobType,
                CreatedDate = DateTime.Now
            };
            _clientDetailRepo.Insert(_clientDetail);
            IHttpActionResult errorResult = GetErrorResult(result);
            if (errorResult != null)
            {
                return "Registration has been Faild";
            }
            else
            {
                _emailSender.email_send(model.Email, user.FirstName+ " "+ user.LastName ,user.Id);
            }
            
            return "Registration has been done, And Account activation link" +
                        "has been sent your eamil id: " + 
                            model.Email;
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
        public ViewAccount GetUserClaims()
        {
            var identityClaims = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identityClaims.Claims;
            ViewAccount model = new ViewAccount()
            {
                UserName = identityClaims.FindFirst("Username").Value,
                Email = identityClaims.FindFirst("Email").Value,
                FirstName = identityClaims.FindFirst("FirstName").Value,
                LastName = identityClaims.FindFirst("LastName").Value,
                ClientId = identityClaims.FindFirst("UserId").Value,
                PhoneNo = identityClaims.FindFirst("PhoneNo").Value,
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
        [Route("api/user/emailverfication/{userName}")]
        [AllowAnonymous]
        public IHttpActionResult emailVerification(string userName)
        {
            ClientDetail clientDetail = _clientDetailRepo.Find(p => p.ClientId == userName).FirstOrDefault();
            clientDetail.EmailConfirmed = true;
            return Ok(_clientDetailRepo.Update(clientDetail));
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

        [HttpPost]
        [Route("api/user/forgetPassword")]
        [AllowAnonymous]
        public IHttpActionResult ForgetPassword(ForgetPassword model)
        {
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            ApplicationUser cUser = manager.FindByName(model.UserName);
            string hashedNewPassword = manager.PasswordHasher.HashPassword(model.NewPassword);
            userStore.SetPasswordHashAsync(cUser, hashedNewPassword);
            return Ok();
        }

        [HttpGet]
        [Route("api/userNameExist")]
        public bool GetUserNameEmailIdExit(AccountModel model)
        {
            return _clientDetailRepo.Find(p => p.EmailId == model.Email).Any();
        }
    }
}
