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
using System.Security.Cryptography;
using System.Text;
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
        ISecretaryRepository _secretaryRepository = RepositoryFactory.Create<ISecretaryRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _doctorRepository = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IFacilityDetailRepository _facilityDetailRepo = RepositoryFactory.Create<IFacilityDetailRepository>(ContextTypes.EntityFramework);

        EmailSender _emailSender = new EmailSender();
        Registration _registration = new Registration();
        //string tokenCode = "";

        [Route("api/account/register")]
        [HttpPost]
        [AllowAnonymous]
        public string Register(AccountModel model)
        {
            ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
            CountryCode countryCode = _countryCodeRepository.Find(x=>x.Id == model.CountryCode).FirstOrDefault();
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            ApplicationUser user = _registration.UserAccount(model, Convert.ToInt16(model.CountryCode));
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
                    _registration.AddClientDetail(user.Id, model, _clientDetailRepo);
                }
                else
                {
                    _registration.AddHospitalDetail(user.Id, model, _hospitalDetailsRepository);
                }
                try
                {
                    _registration.sendRegistrationEmail(user);
                    _registration.sendRegistrationMessage(user);
                }
                catch (Exception ex)
                {
                    return "Registration has been done & getting error in sending email & message" +
                                            ex.Message;
                }
                
            }
            
            return "Registration has been done, And Account activation link" +
                        "has been sent your eamil id: " + 
                            model.Email;
        }
    
        

        [HttpGet]
        [Route("api/GetUserClaims")]
        public ViewAccount GetUserClaims()
        {
            var identityClaims = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identityClaims.Claims;
            var id = claims.FirstOrDefault().Value;
            string firstname = "";
            string lastname = "";
            string phoneno = "";
            string profilepic = "";
            string CountryCode = "";
            var usertype = id.Split('-')[0];
            
            string fileName = id + ".Jpeg";
            try
            {
                if (usertype == "NCM" || usertype == "NCF")
                {
                    var patient = this._clientDetailRepo.GetAll().Where(x => x.ClientId == id).FirstOrDefault();
                    if (patient != null)
                    {
                        string[] fileEntries = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/ProfilePic"));
                        firstname = patient.FirstName;
                        lastname = patient.LastName;
                        phoneno = Convert.ToString(patient.MobileNo);
                        CountryCode = Convert.ToString(patient.CountryCode);
                        foreach (var item in fileEntries)
                        {
                            if (fileName == Path.GetFileName(item))
                            {
                                profilepic = $"{constant.imgUrl}/ProfilePic/{fileName}";
                            }
                        }

                    }
                }
                else if (usertype == "NCD")
                {
                    var doctor = this._doctorRepository.GetAll().Where(x => x.DoctorId == id).FirstOrDefault();
                    if (doctor != null)
                    {
                        firstname = doctor.FirstName;
                        lastname = doctor.LastName;
                        phoneno = doctor.PhoneNumber;
                        CountryCode = Convert.ToString(doctor.CountryCode);
                        string[] fileEntries = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/ProfilePic/Doctor"));
                        foreach (var item in fileEntries)
                        {
                            if (fileName == Path.GetFileName(item))
                            {
                                profilepic = $"{constant.imgUrl}/ProfilePic/Doctor/{fileName}";
                            }
                        }
                        //profilepic = $"{constant.baseUrl}/ProfilePic/{doctor.PhotoPath}";

                    }
                }
                else if (usertype == "NCS")
                {
                    var secretary = this._secretaryRepository.GetAll().Where(x => x.SecretaryId == id).FirstOrDefault();
                    if (secretary != null)
                    {
                        firstname = secretary.FirstName;
                        lastname = secretary.LastName;
                        phoneno = secretary.PhoneNumber;
                        CountryCode = Convert.ToString(secretary.CountryCode);
                        string[] fileEntries = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/ProfilePic/Secretary"));
                        foreach (var item in fileEntries)
                        {
                            if (fileName == Path.GetFileName(item))
                            {
                                profilepic = $"{constant.imgUrl}/ProfilePic/Secretary/{fileName}";
                            }
                        }
                    }
                }
                else
                {
                    var hospital = this._hospitalDetailsRepository.GetAll().Where(x => x.HospitalId == id).FirstOrDefault();
                    if (hospital != null)
                    {
                        firstname = hospital.HospitalName;
                        phoneno = hospital.Mobile.ToString();
                        CountryCode = Convert.ToString(hospital.Country);
                        string[] fileEntries = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/ProfilePic/Hospital"));
                        foreach (var item in fileEntries)
                        {
                            if (fileName == Path.GetFileName(item))
                            {
                                profilepic = $"{constant.imgUrl}/ProfilePic/Hospital/{fileName}";
                            }
                        }
                        //profilepic = $"{constant.baseUrl}/{hospital.ProfilePath}";
                    }
                }
            }catch(Exception ex) { }
            ViewAccount model = new ViewAccount()
            {
                UserName = identityClaims.FindFirst("Username").Value,
                Email = identityClaims.FindFirst("Email").Value,
                FirstName = firstname, //identityClaims.FindFirst("FirstName").Value,
                LastName = lastname, //identityClaims.FindFirst("LastName").Value,
                ClientId = identityClaims.FindFirst("UserId").Value,
                PhoneNo = phoneno.ToString(), //identityClaims.FindFirst("PhoneNo").Value,
                JobType = identityClaims.FindFirst("JobType").Value,
                JobTypePermission = identityClaims.FindFirst("JobTypePermission").Value,
                ProfilePic = profilepic,
                CountryCode=CountryCode
            };
            return model;
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
            clientDetail.PinCode = httpRequest.Form["PinCode"] == null ? clientDetail.PinCode : Convert.ToInt32(httpRequest.Form["PinCode"]);
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
            var clientType = ClientId.Split('-')[0];
            
            if (clientType== "NCH")
            {
                var user = _hospitalDetailsRepository.Find(x => x.HospitalId == ClientId);
                return Ok(user);
            }
            else if (clientType == "NCD")
            {
                var user = _doctorRepository.Find(x => x.DoctorId == ClientId);
                return Ok(user);
            }
            else if (clientType == "NCS")
            {
                var user = _secretaryRepository.Find(x => x.SecretaryId == ClientId);
                return Ok(user);
            }
            else 
            {
                var user = _clientDetailRepo.Find(x => x.ClientId == ClientId);
                return Ok(user);
            }

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
                HospitalDetails hospitalDetails = _hospitalDetailsRepository.Find(x => x.HospitalId.Trim() == userName.Trim()).FirstOrDefault();
                if (hospitalDetails != null)
                {
                    hospitalDetails.EmailConfirmed = true;
                    return Ok(_hospitalDetailsRepository.Update(hospitalDetails));
                }
            }
            else if (userName.Contains("NCS"))
            {
                Secretary secretaryDetail = _secretaryRepository.Find(p => p.SecretaryId == userName).FirstOrDefault();
                if (secretaryDetail != null)
                {
                    secretaryDetail.EmailConfirmed = true;
                    return Ok(_secretaryRepository.Update(secretaryDetail));
                }
            }
            else if (userName.Contains("NCD"))
            {
                Doctor doctorDetail = _doctorRepository.Find(p => p.DoctorId == userName).FirstOrDefault();
                if (doctorDetail != null)
                {
                    doctorDetail.EmailConfirmed = true;
                    return Ok(_doctorRepository.Update(doctorDetail));
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
            string password = _registration.RandomPassword(6);
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            ApplicationUser cUser = manager.FindByName(model.UserName);
            string hashedNewPassword = manager.PasswordHasher.HashPassword(password);
            if (cUser!=null)
            {
                cUser.PasswordHash = hashedNewPassword;
                IdentityResult result = manager.Update(cUser);
                _registration.sendForgotPassword(cUser, password);
                return Ok("success");
            }
            else
            {
                return Ok("fail");
            }
           
           
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
