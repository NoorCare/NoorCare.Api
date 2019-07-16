using AngularJSAuthentication.API.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class AccountController : ApiController
    {

        IClientDetailRepository _clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitalDetailsRepository = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        EmailSender _emailSender = new EmailSender();
        Registration _registration = new Registration();
        string tokenCode = "";

        [Route("api/account/register")]
        [HttpPost]
        [AllowAnonymous]
        public string Register(AccountModel model)
        {
            ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
            CountryCode countryCode = _countryCodeRepository.Find(x=>x.Id == model.CountryCode).FirstOrDefault();

            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            
            string clientId = _registration.creatIdPrix(model) + countryCode.CountryCodes + "-" + _emailSender.Get();
            var user = new ApplicationUser() {
                UserName = model.UserName, Email = model.Email
            };
            user.FirstName = model.FirstName;
            user.PhoneNumber = model.PhoneNumber;
            user.LastName = model.LastName;
            user.Id = clientId;
            IdentityResult result = manager.Create(user, model.Password);
            IHttpActionResult errorResult = GetErrorResult(result);
            if (errorResult != null)
            {
                return "Registration has been Faild";
            }
            else
            {
                if (model.jobType == 1)
                {
                    _registration.AddClientDetail(clientId, model, _clientDetailRepo);
                }
                else if (model.jobType == 2)
                {
                    _registration.AddHospitalDetail(clientId, model, _hospitalDetailsRepository);
                }
                try
                {
                    _emailSender.email_send(model.Email, user.FirstName+ " "+ user.LastName ,user.Id);
                }
                catch (Exception)
                {
                                      
                }
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
                JobType = identityClaims.FindFirst("JobType").Value,
            };
            return model;
        }

        private void createDocPath(string clientId, int desiesId)
        {
            string subPath = $"ProfilePic/{clientId}";
            bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
            if (!exists)
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
        }

        [HttpPost]
        [Route("api/user/updateProfile")]
        [AllowAnonymous]
        public IHttpActionResult UpdateProfile()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;

            string clientId = httpRequest.Form["ClientId"];
            try
            {
                var postedFile = httpRequest.Files["Image"];
                if (postedFile != null)
                {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).
                        Take(10).ToArray()).
                        Replace(" ", "-");
                    imageName = clientId + "." + ImageFormat.Jpeg;
                    var filePath = HttpContext.Current.Server.MapPath("~/ProfilePic/" + imageName);
                    bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/ProfilePic/" + imageName));
                    if (exists)
                    {
                        File.Delete(filePath);
                    }
                    postedFile.SaveAs(filePath);
                }
            }
            catch (Exception)
            {
            }
            
            ClientDetail clientDetail = _clientDetailRepo.Find(p => p.ClientId == clientId).FirstOrDefault();
            clientDetail.FirstName = httpRequest.Form["FirstName"] == null ? clientDetail.FirstName: httpRequest.Form["FirstName"];
            clientDetail.LastName = httpRequest.Form["LastName"] == null ? clientDetail.LastName : httpRequest.Form["LastName"];
            clientDetail.PinCode = httpRequest.Form["PinCode"] == null ? clientDetail.PinCode : Convert.ToInt16(httpRequest.Form["PinCode"]);
            clientDetail.Gender = httpRequest.Form["Gender"] == null ? clientDetail.Gender : Convert.ToInt16(httpRequest.Form["Gender"]);
            clientDetail.Address = httpRequest.Form["Address"] == null ? clientDetail.Address : httpRequest.Form["Address"];
            clientDetail.City = httpRequest.Form["City"] == null ? clientDetail.City : httpRequest.Form["City"];
            clientDetail.State = httpRequest.Form["State"] == null ? clientDetail.State : httpRequest.Form["State"];
            clientDetail.Country = httpRequest.Form["Country"] == null ? clientDetail.Country : httpRequest.Form["Country"];
            clientDetail.MobileNo = httpRequest.Form["MobileNo"] == null ? clientDetail.MobileNo : Convert.ToInt32(httpRequest.Form["MobileNo"]);
            clientDetail.EmailId = httpRequest.Form["EmailId"] == null ? clientDetail.EmailId : httpRequest.Form["EmailId"];
            clientDetail.MaritalStatus = httpRequest.Form["MaritalStatus"]== null ? clientDetail.MaritalStatus : Convert.ToInt16(httpRequest.Form["MaritalStatus"]);
            clientDetail.DOB = httpRequest.Form["DOB"]== null ? clientDetail.DOB :httpRequest.Form["DOB"];
            return Ok(_clientDetailRepo.Update(clientDetail));
        }

        [HttpGet]
        [Route("api/user/profile/{ClientId}")]
        public IHttpActionResult getProfileData(string ClientId)
        {
            return Ok(_clientDetailRepo.Find(x =>x.ClientId == ClientId));
        }

        [HttpGet]
        [Route("api/hospital/profile/{ClientId}")]
        public IHttpActionResult gethospitalData(string ClientId)
        {
            return Ok(_hospitalDetailsRepository.Find(x => x.HospitalId == ClientId));
        }

        [HttpGet]
        [Route("api/user/emailverfication/{userName}")]
        [AllowAnonymous]
        public IHttpActionResult emailVerification(string userName)
        {
            if (userName.Contains("NCH"))
            {
                HospitalDetail hospitalDetails = _hospitalDetailsRepository.Find(x => x.HospitalId.Trim() == userName.Trim()).FirstOrDefault();
                if (hospitalDetails != null)
                {
                    hospitalDetails.EmailConfirmed = true;
                    return Ok(_hospitalDetailsRepository.Update(hospitalDetails));
                }
            }
            else
            {
                ClientDetail clientDetail = _clientDetailRepo.Find(p => p.ClientId == userName).FirstOrDefault();
                if (clientDetail != null)
                {
                    clientDetail.EmailConfirmed = true;
                    return Ok(_clientDetailRepo.Update(clientDetail));
                }
            }
            
            return Ok();
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

        [HttpPost]
        [Route("api/userNameExist")]
        [AllowAnonymous]
        public IHttpActionResult GetUserNameEmailIdExit(AccountModel model)
        {
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);

            return Ok(manager.FindByName(model.UserName));
        }

        [HttpPost]
        [Route("api/userEmailExists")]
        [AllowAnonymous]
        public IHttpActionResult GetUserEmailId(AccountModel model)
        {
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);

            return Ok(manager.FindByEmail(model.Email));
        }
    }
}
