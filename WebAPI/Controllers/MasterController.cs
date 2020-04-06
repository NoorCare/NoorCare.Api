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

namespace WebAPI.Controllers
{
    public class MasterController : ApiController
    {
        ITimeMasterRepository _timeMasterRepo = RepositoryFactory.Create<ITimeMasterRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        [Route("api/GetTimeMaster")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllTime()
        {
            var result = _timeMasterRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/facility")]
        [HttpGet]
        [AllowAnonymous]
        public List<Facility> GetFacility()
        {
            IFacilityRepository _facilityDetailRepo = RepositoryFactory.Create<IFacilityRepository>(ContextTypes.EntityFramework);
            return _facilityDetailRepo.GetAll().OrderBy(x => x.facility).ToList();
        }

        [Route("api/diseaseType")]
        [HttpGet]
        [AllowAnonymous]
        public List<Disease> GetDisease()
        {
            IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
            return _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
        }

        [Route("api/countryCode")]
        [HttpGet]
        [AllowAnonymous]
        public List<CountryCode> GetCountryCode()
        {
            ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
            return _countryCodeRepository.GetAll().ToList();
        }

        [Route("api/city/{countryId}")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblCity> GetCity(int countryId)
        {
            ICityRepository _cityRepository = RepositoryFactory.Create<ICityRepository>(ContextTypes.EntityFramework);
            return _cityRepository.Find(x => x.CountryId == countryId).OrderBy(x => x.City).ToList();
        }

        [Route("api/countries")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblCountry> GetCountries()
        {
            ICountryRepository _cityRepository = RepositoryFactory.Create<ICountryRepository>(ContextTypes.EntityFramework);
            var country = _cityRepository.GetAll().OrderBy(x => x.CountryName).ToList();
            Dictionary<string, string> countryies = new Dictionary<string, string>();
            return country;
        }

        [Route("api/state")]
        [HttpGet]
        [AllowAnonymous]
        public List<State> GetState()
        {
            IStateRepository _stateRepository = RepositoryFactory.Create<IStateRepository>(ContextTypes.EntityFramework);
            return _stateRepository.GetAll().OrderBy(x => x.state).ToList();
        }

        [Route("api/hospitalServices")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblHospitalServices> HospitalServices()
        {
            ITblHospitalServicesRepository _stateRepository = RepositoryFactory.Create<ITblHospitalServicesRepository>(ContextTypes.EntityFramework);
            return _stateRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
        }

        [Route("api/hospitalSpecialization")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblHospitalSpecialties> HospitalSpecialization()
        {
            ITblHospitalSpecialtiesRepository _stateRepository = RepositoryFactory.Create<ITblHospitalSpecialtiesRepository>(ContextTypes.EntityFramework);
            return _stateRepository.GetAll().OrderBy(x => x.HospitalSpecialties).ToList();
        }

        [Route("api/hospitalAmenities")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblHospitalAmenities> HospitalAmenities()
        {
            ITblHospitalAmenitiesRepository _stateRepository = RepositoryFactory.Create<ITblHospitalAmenitiesRepository>(ContextTypes.EntityFramework);
            return _stateRepository.GetAll().OrderBy(x => x.HospitalAmenities).ToList();
        }

        [Route("api/autocompletedata/{searchtype}")]
        [HttpGet]
        [AllowAnonymous]
        public List<string> AtocompleteData(string searchtype)
        {
            List<string> autodatalist = new List<string>();
            if (searchtype == "1")
            {
                var hospitals = from h in _hospitaldetailsRepo.GetAll()
                          select new { Name = h.HospitalName.ToString()+"( "+h.HospitalId+"-"+h.Address+")" };
                List<AutocompleteData> autocompleteData = new List<AutocompleteData>();
                foreach (var item in hospitals)
                {
                    autodatalist.Add(item.Name);
                }
                return autodatalist;
            }
            else
            {
                var doctors = from d in _doctorRepo.GetAll()
                              join h in _hospitaldetailsRepo.GetAll() on d.HospitalId equals h.HospitalId
                          select new { Name = d.FirstName + "( " + d.DoctorId + "-" + h.HospitalName + ")" };
                List<AutocompleteData> autocompleteData = new List<AutocompleteData>();
                foreach (var item in doctors)
                {
                    autodatalist.Add(item.Name);
                }
                return autodatalist;
            }
        }
    }
}

