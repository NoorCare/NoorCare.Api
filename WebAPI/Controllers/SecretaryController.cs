using AngularJSAuthentication.API.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class SecretaryController : ApiController
    {
        ISecretaryRepository _secretaryRepo = RepositoryFactory.Create<ISecretaryRepository>(ContextTypes.EntityFramework);
        ISecretaryRepository _getSecretaryList = RepositoryFactory.Create<ISecretaryRepository>(ContextTypes.EntityFramework);
        Registration _registration = new Registration();

        [Route("api/secretary/getall")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAll()
        {
            var result = _secretaryRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/secretary/getdetail/{secretaryId}")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Secretary/5
        public HttpResponseMessage GetDetail(string secretaryId)
        {
            var result = _secretaryRepo.Find(x => x.SecretaryId == secretaryId).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/secretary/register")]
        [HttpPost]
        [AllowAnonymous]
        // POST: api/Secretary
        public HttpResponseMessage Register(Secretary obj)
        {
            ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
            CountryCode countryCode = _countryCodeRepository.Find(x => x.Id == obj.CountryCode).FirstOrDefault();
            int country_Code = 0;
            if (countryCode!=null)
            {
                country_Code = Convert.ToInt16(countryCode.CountryCodes);
            }
            EmailSender _emailSender = new EmailSender();
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            string password = _registration.RandomPassword(6);
            ApplicationUser user = _registration.UserAcoount(obj, Convert.ToInt16(country_Code));
            IdentityResult result = manager.Create(user, password);
            user.PasswordHash = password;
            _registration.sendRegistrationEmail(user);
            _registration.sendRegistrationMessage(user);
            obj.SecretaryId = user.Id;
            var _sectiryCreated = _secretaryRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, obj.SecretaryId);
        }

        [HttpPost]
        [Route("api/secretary/uploadprofilepic")]
        [AllowAnonymous]
        public IHttpActionResult UploadProfilePic()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;

            string secretaryId = httpRequest.Form["SecretaryId"];
            try
            {
                var postedFile = httpRequest.Files["Image"];
                if (postedFile != null)
                {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).
                        Take(10).ToArray()).
                        Replace(" ", "-");
                    imageName = secretaryId + "." + ImageFormat.Jpeg;
                    var filePath = HttpContext.Current.Server.MapPath("~/ProfilePic/Secretary/" + imageName);
                    bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/ProfilePic/Secretary/" + imageName));
                    if (exists)
                    {
                        File.Delete(filePath);
                    }
                    postedFile.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok(secretaryId);
        }

        [HttpGet]
        [Route("api/secretary/profile/{secretaryId}")]
        [AllowAnonymous]
        public IHttpActionResult getSecretaryProfile(string secretaryId)
        {
            return Ok(_secretaryRepo.Find(x => x.SecretaryId == secretaryId));
        }
        
        [Route("api/secretary/update")]
        [HttpPut]
        [AllowAnonymous]
        // PUT: api/Secretary/5
        public HttpResponseMessage Update(Secretary obj)
        {
            var secretary = _getSecretaryList.Find(s => s.SecretaryId == obj.SecretaryId).FirstOrDefault();
            if (secretary!=null)
            {
                obj.Id = secretary.Id;
            }
            var result = _secretaryRepo.Update(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/secretary/delete/{secretaryId}")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage Delete(string secretaryId)
        {
            var secretary = _getSecretaryList.Find(s => s.SecretaryId == obj.SecretaryId).FirstOrDefault();
            int tbleId = 0;
            if (secretary != null)
            {
                tbleId = secretary.Id;
            }
            var result = _secretaryRepo.Delete(tbleId);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }
    }
}
