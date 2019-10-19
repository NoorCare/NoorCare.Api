using AngularJSAuthentication.API.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        [Route("api/Facility/register")]
        [HttpPost]
        [AllowAnonymous]

        public HttpResponseMessage Register(FacilityDetail obj)
        {
            ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
            CountryCode countryCode = _countryCodeRepository.Find(x => x.Id == obj.CountryCode).FirstOrDefault();
            if (countryCode != null)
            {
                EmailSender _emailSender = new EmailSender();
                var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var manager = new UserManager<ApplicationUser>(userStore);
                string password = _registration.RandomPassword(6);              

                ApplicationUser user = _registration.UserAcoount(obj, Convert.ToInt16(countryCode.CountryCodes));
                IdentityResult result = manager.Create(user, password);
                user.PasswordHash = password;
                _registration.sendRegistrationEmail(user);
                obj.FacilityDetailId = user.Id;

                _facilityDetailRepo.Insert(obj);

                return Request.CreateResponse(HttpStatusCode.Accepted, obj.FacilityDetailId);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, "Wrong country code");
            }
        }
    }
}