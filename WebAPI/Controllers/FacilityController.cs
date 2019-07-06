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
            return _facilityDetailRepo.GetAll().ToList();
        }

        [Route("api/diseaseType")]
        [HttpGet]
        [AllowAnonymous]
        public List<Disease> GetDisease()
        {
            IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
            return _diseaseDetailRepo.GetAll().ToList();
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
            return _cityRepository.Find(x=>x.CountryId == countryId).ToList();
        }

        [Route("api/countries")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblCountry> GetCountries()
        {
            ICountryRepository _cityRepository = RepositoryFactory.Create<ICountryRepository>(ContextTypes.EntityFramework);
            return _cityRepository.GetAll().ToList();
        }

        [Route("api/state")]
        [HttpGet]
        [AllowAnonymous]
        public List<State> GetState()
        {
            IStateRepository _stateRepository = RepositoryFactory.Create<IStateRepository>(ContextTypes.EntityFramework);
            return _stateRepository.GetAll().ToList();
        }

    }
}
