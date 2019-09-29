using NoorCare.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class FacilityController : ApiController
    {
        [Route("api/facility")]
        [HttpGet]
        [AllowAnonymous]
        public List<Facility> GetFacility()
        {
            IFacilityRepository _facilityDetailRepo = RepositoryFactory.Create<IFacilityRepository>(ContextTypes.EntityFramework);
            return _facilityDetailRepo.GetAll().OrderBy(x=>x.facility).ToList();
        }

        [Route("api/diseaseType")]
        [HttpGet]
        [AllowAnonymous]
        public List<Disease> GetDisease()
        {
            IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
            return _diseaseDetailRepo.GetAll().OrderBy(x=>x.DiseaseType).ToList();
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
            return _cityRepository.Find(x=>x.CountryId == countryId).OrderBy(x => x.City).ToList();
        }

        [Route("api/countries")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblCountry> GetCountries()
        {
            ICountryRepository _cityRepository = RepositoryFactory.Create<ICountryRepository>(ContextTypes.EntityFramework);
            return _cityRepository.GetAll().OrderBy(x => x.CountryName).ToList();
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

    }
}
