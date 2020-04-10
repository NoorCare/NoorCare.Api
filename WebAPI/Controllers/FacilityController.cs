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
        IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
        ITblHospitalServicesRepository _hospitalServicesRepository = RepositoryFactory.Create<ITblHospitalServicesRepository>(ContextTypes.EntityFramework);
        ITblHospitalAmenitiesRepository _hospitalAmenitieRepository = RepositoryFactory.Create<ITblHospitalAmenitiesRepository>(ContextTypes.EntityFramework);


        [Route("api/Facility/register")]
        [HttpPost]
        [AllowAnonymous]
        public string Register(AccountModel obj)
        {
            ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
            CountryCode countryCode = _countryCodeRepository.Find(x => x.Id == obj.CountryCode).FirstOrDefault();
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            ApplicationUser user = _registration.UserAcoount(obj, Convert.ToInt16(countryCode.CountryCodes));

            IdentityResult result = manager.Create(user, obj.Password);

            IHttpActionResult errorResult = GetErrorResult(result);
            if (errorResult != null)
            {
                return "Registration has been Faild";
            }
            else
            {
                _registration.AddFacilityDetail(user.Id, obj, _facilityDetailRepo);

                _registration.sendRegistrationEmail(user);

                _registration.sendRegistrationMessage(user);
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



        [Route("api/GetFacilityDetail/{FacilityDetailId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetFacilityDetail(string FacilityDetailId)
        {
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            var hospitalService = _hospitalServicesRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
            var hospitalAmenitie = _hospitalAmenitieRepository.GetAll().OrderBy(x => x.HospitalAmenities).ToList();
            List<FacilityDetail> facility = _facilityDetailRepo.Find(x => x.FacilityDetailId == FacilityDetailId);
            Facilities _facilties = new Facilities();

            List<Facilities> _faciltiess = new List<Facilities>();

            foreach (var f in facility ?? new List<FacilityDetail>())
            {
                _facilties = new Facilities
                {
                    Id = f.Id,
                    FacilityDetailId = f.FacilityDetailId,
                    FacilityId = f.FacilityId,
                    ProviderName = f.ProviderName,
                    FirstName = f.FirstName,
                    LastName = f.LastName,
                    CountryCode = f.CountryCode,
                    Email = f.Email,
                    EmailConfirmed = f.EmailConfirmed,
                    PhoneNumber = f.PhoneNumber,
                    jobType = f.jobType,
                    AboutUs = f.AboutUs,
                    PhotoPath = f.PhotoPath,
                    Website = f.Website,
                    EstablishYear = f.EstablishYear,
                    Address = f.Address,
                    Street = f.Street,
                    Country = f.Country,
                    City = f.City,
                    PostCode = f.PostCode,
                    Landmark = f.Landmark,
                    MapLocation = f.MapLocation,
                    Specialization = getSpecialization(f.Specialization, disease),
                    // Amenities = f.Amenities,
                    Amenities = getHospitalAmenities(f.Amenities, hospitalAmenitie),
                    // Services = f.Services,
                    Services = getHospitalService(f.Services, hospitalService),
                    Timing = f.Timing,
                    IsDeleted = f.IsDeleted,
                    CreatedBy = f.CreatedBy,
                    ModifiedBy = f.ModifiedBy,
                    DateEntered = f.DateEntered,
                    DateModified = f.DateModified,
                };

                _faciltiess.Add(_facilties);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, _faciltiess);
        }

        #region Utility

        private List<TblHospitalServices> getHospitalService(string serviceType, List<TblHospitalServices> hospitalService)
        {
            if (!string.IsNullOrEmpty(serviceType))
            {
                var serviceTypes = serviceType.Split(',');
                int[] myInts = Array.ConvertAll(serviceTypes, s => int.Parse(s));
                var hospitalServiceList = hospitalService.Where(x => myInts.Contains(x.Id)).ToList();
                return hospitalServiceList;
            }
            else
                return null;
        }

        private List<TblHospitalAmenities> getHospitalAmenities(string amenitieType, List<TblHospitalAmenities> hospitalAmenitie)
        {
            if (!string.IsNullOrEmpty(amenitieType))
            {
                var serviceTypes = amenitieType.Split(',');
                int[] myInts = Array.ConvertAll(serviceTypes, s => int.Parse(s));
                var hospitalAmenitieList = hospitalAmenitie.Where(x => myInts.Contains(x.Id)).ToList();

                return hospitalAmenitieList;
            }
            else
            {
                return null;
            }
        }

        private List<Disease> getSpecialization(string diesiesType, List<Disease> diseases)
        {
            if (!string.IsNullOrEmpty(diesiesType))
            {
                var diesiesTypes = diesiesType.Split(',');
                int[] myInts = Array.ConvertAll(diesiesTypes, s => int.Parse(s));
                var diseasesList = diseases.Where(x => myInts.Contains(x.Id)).ToList();
                return diseasesList;
            }
            else
            {
                return null;
            }
        }


        #endregion

    }
}