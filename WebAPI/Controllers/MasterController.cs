using Newtonsoft.Json;
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
        IClientDetailRepository _clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);
        ILeaveMasterRepository _leaveMasterRepo = RepositoryFactory.Create<ILeaveMasterRepository>(ContextTypes.EntityFramework);

        [Route("api/GetTimeMaster")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllTime()
        {
            var result = _timeMasterRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/GetDayWiseTimeMaster")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetDayTimeMaster()
        {
            var result = _timeMasterRepo.GetAll().ToList();
            List<DayTimeMaster> timeMaster = new List<DayTimeMaster>();
             
            for (int i = 0; i < 7; i++)
            {
                string serialized = JsonConvert.SerializeObject(result);
                var copy = JsonConvert.DeserializeObject<List<TimeMaster>>(serialized);
                string[] array = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
                DayTimeMaster obj = new DayTimeMaster();
                obj.Day = array[i];
                obj.TimeMaster = copy;
                timeMaster.Add(obj);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, timeMaster);
        }


        [Route("api/facility")]
        [HttpGet]
        [AllowAnonymous]
        public List<Facility> GetFacility()
        {
            IFacilityRepository _facilityDetailRepo = RepositoryFactory.Create<IFacilityRepository>(ContextTypes.EntityFramework);
            return _facilityDetailRepo.GetAll().OrderBy(x => x.facility).ToList();
        }

        [Route("api/leaveMaster")]
        [HttpGet]
        [AllowAnonymous]
        public List<LeaveMaster> GetLeaveMaster()
        {
            return _leaveMasterRepo.GetAll().Where(a => a.IsDeleted == false).OrderBy(x => x.LeaveType).ToList();
        }

        [Route("api/diseaseType")]
        [HttpGet]
        [AllowAnonymous]
        public List<Disease> GetDisease()
        {
            IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
            var diseaseTypes = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            return diseaseTypes;
        }

        [Route("api/reporttype")]
        [HttpGet]
        [AllowAnonymous]
        public List<Report> GetReportType()
        {
            IReportRepository _reportDetailRepo = RepositoryFactory.Create<IReportRepository>(ContextTypes.EntityFramework);
            var reportTypes = _reportDetailRepo.GetAll().ToList();
            return reportTypes;
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
        public List<TblCity> GetCity(string countryId)
        {
            int _countryId = 0;
            if (countryId == "undefined" || countryId == "null" || string.IsNullOrWhiteSpace(countryId))
            {
                _countryId = 974;
            }
            else
            {
                _countryId = Convert.ToInt16(countryId);
            }

            ICityRepository _cityRepository = RepositoryFactory.Create<ICityRepository>(ContextTypes.EntityFramework);
            return _cityRepository.Find(x => x.CountryId == _countryId).OrderBy(x => x.City).ToList();
        }

        [Route("api/citybycountrycode/{countrycode}")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblCity> GetCityByCountryCode(int countrycode)
        {

            ICityRepository _cityRepository = RepositoryFactory.Create<ICityRepository>(ContextTypes.EntityFramework);
            var citylist = _cityRepository.Find(x => x.CountryId == countrycode).OrderBy(x => x.City).ToList();
            return citylist;
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

        [Route("api/insuranceMaster")]
        [HttpGet]
        [AllowAnonymous]
        public List<InsuranceMaster> GetInsurance()
        {
            IInsuranceMasterRepository _insuRepository = RepositoryFactory.Create<IInsuranceMasterRepository>(ContextTypes.EntityFramework);
            var insur = _insuRepository.GetAll().Where(x => x.InUsed == true).OrderBy(x => x.InsuranceCompanyName).ToList();
            return insur;
        }

        [Route("api/state")]
        [HttpGet]
        [AllowAnonymous]
        public List<State> GetState()
        {
            IStateRepository _stateRepository = RepositoryFactory.Create<IStateRepository>(ContextTypes.EntityFramework);
            return _stateRepository.GetAll().OrderBy(x => x.state).OrderBy(x => x.state).ToList();
        }

        [Route("api/hospitalServices")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblHospitalServices> HospitalServices()
        {
            ITblHospitalServicesRepository _stateRepository = RepositoryFactory.Create<ITblHospitalServicesRepository>(ContextTypes.EntityFramework);
            return _stateRepository.GetAll().OrderBy(x => x.HospitalServices).OrderBy(x=>x.HospitalServices).ToList();
        }

        [Route("api/hospitalSpecialization")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblHospitalSpecialties> HospitalSpecialization()
        {
            ITblHospitalSpecialtiesRepository _stateRepository = RepositoryFactory.Create<ITblHospitalSpecialtiesRepository>(ContextTypes.EntityFramework);
            return _stateRepository.GetAll().OrderBy(x => x.HospitalSpecialties).OrderBy(x => x.HospitalSpecialties).ToList();
        }

        [Route("api/hospitalAmenities")]
        [HttpGet]
        [AllowAnonymous]
        public List<TblHospitalAmenities> HospitalAmenities()
        {
            ITblHospitalAmenitiesRepository _stateRepository = RepositoryFactory.Create<ITblHospitalAmenitiesRepository>(ContextTypes.EntityFramework);
            var amenities = _stateRepository.GetAll().OrderBy(x => x.HospitalAmenities).OrderBy(x => x.HospitalAmenities).ToList();


            return amenities;
        }

        [Route("api/Specialization/{type}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetSpecialization(string type)
        {
            if (type == "3")
            {
                //doctor Search
                IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
                var diseaseTypes = from d in _diseaseDetailRepo.GetAll()
                                   select new { Id = d.Id, Name = d.DiseaseType};

                return Request.CreateResponse(HttpStatusCode.Accepted, diseaseTypes.OrderBy(x=>x.Name)); 
            }
            else
            {
                //Hospital Search 
                ITblHospitalSpecialtiesRepository _stateRepository = RepositoryFactory.Create<ITblHospitalSpecialtiesRepository>(ContextTypes.EntityFramework);
                var result= from h in  _stateRepository.GetAll()
                            select new { Id = h.Id, Name = h.HospitalSpecialties.ToString() };

                return Request.CreateResponse(HttpStatusCode.Accepted, result.OrderBy(x=>x.Name));
            }
        }

        [Route("api/autocomplete/{searchtype}/{autosearchtext}")]
        [HttpGet]
        [AllowAnonymous]
        public List<AutocompleteData> autocomplete(string searchtype, string autosearchtext)
        {
            //List<string> autodatalist = new List<string>();
            List<AutocompleteData> autocompleteData = new List<AutocompleteData>();
            if (searchtype == "Email")
            {
                var doctors = from d in _doctorRepo.GetAll()
                              join h in _hospitaldetailsRepo.GetAll() on d.HospitalId equals h.HospitalId
                              where (d.DoctorId.ToLower().Contains(autosearchtext.ToLower()) ||
                              d.FirstName.ToLower().Contains(autosearchtext.ToLower()) || d.LastName.ToLower().Contains(autosearchtext.ToLower()))
                              && d.EmailConfirmed == true && d.IsDeleted == false
                              select new { Id = d.DoctorId, Name = d.FirstName + " " + d.LastName + " " + d.DoctorId.Replace('-', ' ') };

                var patient = this._clientDetailRepo.GetAll().Where(x => (x.ClientId.ToLower().Contains(autosearchtext.ToLower()) ||
                              x.FirstName.ToLower().Contains(autosearchtext.ToLower()) || x.LastName.ToLower().Contains(autosearchtext.ToLower()))
                              && x.EmailConfirmed == true && x.IsActive == true).ToList();

                var hospitals = from h in _hospitaldetailsRepo.GetAll().Where(x => (x.HospitalId.ToLower().Contains(autosearchtext.ToLower()) || x.HospitalName.ToLower().Contains(autosearchtext.ToLower()))
                                && x.IsDeleted == false && x.IsDocumentApproved == 1 && x.EmailConfirmed == true && x.IsBlocked == false)
                                select new { Id = h.HospitalId, Name = h.HospitalName.ToString() + " " + h.HospitalId.Replace('-', ' ') };
                foreach (var item in doctors)
                {
                    var autocomp = new AutocompleteData();
                    autocomp.Id = item.Id;
                    autocomp.Name = item.Name;
                    autocompleteData.Add(autocomp);
                }
                foreach (var item in hospitals)
                {
                    var autocomp = new AutocompleteData();
                    autocomp.Id = item.Id;
                    autocomp.Name = item.Name;
                    autocompleteData.Add(autocomp);
                }
                foreach (var item in patient)
                {
                    var autocomp = new AutocompleteData();
                    autocomp.Id = item.ClientId;
                    autocomp.Name = item.FirstName + ' ' + item.LastName + " " + item.ClientId.Replace('-', ' ');
                    autocompleteData.Add(autocomp);
                }
                return autocompleteData;
            }
            else if (searchtype == "3" || searchtype == "0")
            {
                var doctors = from d in _doctorRepo.GetAll()
                              join h in _hospitaldetailsRepo.GetAll() on d.HospitalId equals h.HospitalId
                              where (d.FirstName.ToLower().Contains(autosearchtext.ToLower()) || d.LastName.ToLower().Contains(autosearchtext.ToLower()))
                              && d.EmailConfirmed == true && d.IsDeleted == false && h.IsBlocked == false
                              select new { Id = d.DoctorId, Name = d.FirstName + " " + d.LastName + " " + d.DoctorId.Replace('-', ' ') + " " + h.HospitalName };
                //List<AutocompleteData> autocompleteData = new List<AutocompleteData>();
                foreach (var item in doctors)
                {
                    var autocomp = new AutocompleteData();
                    autocomp.Id = item.Id;
                    autocomp.Name = item.Name;
                    autocompleteData.Add(autocomp);
                }
                return autocompleteData;


            }
            else
            {
                var hospitals = from h in _hospitaldetailsRepo.GetAll().Where(x => x.HospitalName.ToLower().Contains(autosearchtext.ToLower())
                                && x.IsDeleted == false && x.IsDocumentApproved == 1 && x.EmailConfirmed == true && x.IsBlocked == false)
                                select new { Id = h.HospitalId, Name = h.HospitalName.ToString() + " " + h.HospitalId.Replace('-', ' ') };
                //List<AutocompleteData> autocompleteData = new List<AutocompleteData>();
                foreach (var item in hospitals)
                {
                    var autocomp = new AutocompleteData();
                    autocomp.Id = item.Id;
                    autocomp.Name = item.Name;
                    autocompleteData.Add(autocomp);
                }
                return autocompleteData;
            }
        }

        [Route("api/autocompletedata/{searchtype}/{autosearchtext}")]
        [HttpGet]
        [AllowAnonymous]
        public List<string> AtocompleteData(string searchtype, string autosearchtext)
        {
            List<string> autodatalist = new List<string>();
            if (searchtype == "1")
            {
                var hospitals = from h in _hospitaldetailsRepo.GetAll().Where(x => x.HospitalName.ToLower().Contains(autosearchtext.ToLower())
                                && x.IsDeleted == false && x.IsDocumentApproved == 1 && x.EmailConfirmed == true && x.IsBlocked == false)
                                select new { Name = h.HospitalName.ToString() + " " + h.HospitalId.Replace('-', ' ') };
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
                              where (d.FirstName.ToLower().Contains(autosearchtext.ToLower()) || d.LastName.ToLower().Contains(autosearchtext.ToLower()))
                              && d.EmailConfirmed == true && d.IsDeleted == false
                              select new { Name = d.FirstName + " " + d.LastName + " " + d.DoctorId.Replace('-', ' ') + " " + h.HospitalName };
                List<AutocompleteData> autocompleteData = new List<AutocompleteData>();
                foreach (var item in doctors)
                {
                    autodatalist.Add(item.Name);
                }
                return autodatalist;
            }
        }

        [HttpGet]
        [Route("api/allhospital/logo")]
        [AllowAnonymous]
        public HttpResponseMessage GetAllHospitalLogoImage(string FacilityNoorCare)
        {
            try
            {
                var logoIMG = _hospitaldetailsRepo.GetAll().ToList();

                return Request.CreateResponse(HttpStatusCode.Accepted, logoIMG);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, "Error");

            }
        }

        [HttpGet]
        [Route("api/allspecialties")]
        [AllowAnonymous]
        public HttpResponseMessage GetAllSpecialties()
        {
            ISpecialtiesRepository _specialtiesRepository = RepositoryFactory.Create<ISpecialtiesRepository>(ContextTypes.EntityFramework);
            try
            {
                var specialties = _specialtiesRepository.GetAll().ToList();

                return Request.CreateResponse(HttpStatusCode.Accepted, specialties);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, "Error");

            }
        }

        [HttpGet]
        [Route("api/alleducation")]
        [AllowAnonymous]
        public HttpResponseMessage GetAllDoctorEducation()
        {
            IDoctorEducationRepository _doctorEducationRepository = RepositoryFactory.Create<IDoctorEducationRepository>(ContextTypes.EntityFramework);
            try
            {
                var doctorEducations = _doctorEducationRepository.GetAll().ToList();

                return Request.CreateResponse(HttpStatusCode.Accepted, doctorEducations);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, "Error");

            }
        }
    }
}