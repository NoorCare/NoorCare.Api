using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Linq;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class FacilityController : ApiController
    {
        Registration _registration = new Registration();
        IFacilityDetailRepository _facilityDetailRepo = RepositoryFactory.Create<IFacilityDetailRepository>(ContextTypes.EntityFramework);

        [Route("api/Facility/register/{UserId}/{Password}")]
        [HttpPost]
        [AllowAnonymous]
        public string Register(WebAPI.Entity.FacilityDetail obj,string UserId, string Password)
        {
            ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
            CountryCode countryCode = _countryCodeRepository.Find(x => x.Id == obj.CountryCode).FirstOrDefault();
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            ApplicationUser user = _registration.UserAcoount(obj, Convert.ToInt16(countryCode.CountryCodes));
            IdentityResult result = manager.Create(user, Password);
            IHttpActionResult errorResult = GetErrorResult(result);
            if (errorResult != null)
            {
                return "Registration has been Faild";
            }
            else
            {
                 obj.FacilityDetailId = user.Id;
                 _facilityDetailRepo.Insert(obj);

                _registration.sendRegistrationEmail(user);
            }

            return "Registration has been done, And Account activation link" +
                        "has been sent your eamil id: " +
                            obj.Email;
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
    }
}