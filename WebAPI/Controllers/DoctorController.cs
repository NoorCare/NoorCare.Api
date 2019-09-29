using AngularJSAuthentication.API.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
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
    public class DoctorController : ApiController
    {
        Registration _registration = new Registration();
        IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _getDoctorList = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        IDoctorAvailableTimeRepository _doctorAvailabilityRepo = RepositoryFactory.Create<IDoctorAvailableTimeRepository>(ContextTypes.EntityFramework);
        IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
        ITblHospitalServicesRepository _hospitalServicesRepository = RepositoryFactory.Create<ITblHospitalServicesRepository>(ContextTypes.EntityFramework);
        ITblHospitalAmenitiesRepository _hospitalAmenitieRepository = RepositoryFactory.Create<ITblHospitalAmenitiesRepository>(ContextTypes.EntityFramework);
        IFeedbackRepository _feedbackRepo = RepositoryFactory.Create<IFeedbackRepository>(ContextTypes.EntityFramework);

        [Route("api/doctor/getall")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Doctor
        public HttpResponseMessage GetAll()
        {
            var result = _doctorRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/doctor/{country?}/{city?}/{diseaseType?}")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Doctor
        public HttpResponseMessage Getdoctor(string country = null, string city = null, string diseaseType = null)
        {
            IHospitalDetailsRepository _hospitalRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
            List<HospitalDetails> hospitalDetails = _hospitalRepo.Find(
                x => country != null ? x.Country == country : x.Country == x.Country &&
                city != null ? x.City == city : x.City == x.City);
            List<string> _hospitalid = new List<string>();
            foreach (var hospitalDetail in hospitalDetails ?? new List<HospitalDetails>())
            {
                _hospitalid.Add(hospitalDetail.HospitalId);
            }
            string _hospitalId = string.Join(",", _hospitalid);
            IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
            var result = _doctorRepo.Find(x =>
            x.HospitalId.Contains(_hospitalId) &&
            diseaseType != null ? x.Specialization.Contains(diseaseType) : x.Specialization == x.Specialization
            );


            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/doctorDetails/{doctorid}")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Doctor/5
        public HttpResponseMessage DoctorDetails(string doctorid)
        {
            var result = _doctorRepo.Find(x => x.DoctorId == doctorid).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/doctor/getdetail/{doctorid}")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Doctor/5
        public HttpResponseMessage GetDetail(string doctorid)
        {
            var result = _doctorRepo.Find(x => x.DoctorId == doctorid).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/doctor/register")]
        [HttpPost]
        [AllowAnonymous]
        // POST: api/Doctor
        public HttpResponseMessage Register(Doctor obj)
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
                obj.DoctorId = user.Id;
                IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
                var _doctorCreated = _doctorRepo.Insert(obj);

                return Request.CreateResponse(HttpStatusCode.Accepted, obj.DoctorId);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, "Wrong country code");
            }
        }

        [HttpPost]
        [Route("api/doctor/uploadprofilepic")]
        [AllowAnonymous]
        public IHttpActionResult UploadProfilePic()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;

            string doctorId = httpRequest.Form["DoctorId"];
            try
            {
                var postedFile = httpRequest.Files["Image"];
                if (postedFile != null)
                {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).
                        Take(10).ToArray()).
                        Replace(" ", "-");
                    imageName = doctorId + "." + ImageFormat.Jpeg;
                    var filePath = HttpContext.Current.Server.MapPath("~/ProfilePic/Doctor/" + imageName);
                    bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/ProfilePic/Doctor/" + imageName));
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
            return Ok(doctorId);
        }

        [HttpGet]
        [Route("api/Doctor/profile/{DoctorId}")]
        [AllowAnonymous]
        public IHttpActionResult getDoctorProfile(string DoctorId)
        {
            return Ok(_doctorRepo.Find(x => x.DoctorId == DoctorId));
        }


        [Route("api/doctor/update")]
        [HttpPut]
        [AllowAnonymous]
        // PUT: api/Doctor/5
        public HttpResponseMessage Update(Doctor obj)
        {
            obj.Id = _getDoctorList.Find(x => x.DoctorId == obj.DoctorId).FirstOrDefault().Id;
            var result = _doctorRepo.Update(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/doctor/delete/{doctorid}")]
        [HttpDelete]
        [AllowAnonymous]
        // DELETE: api/Doctor/5
        public HttpResponseMessage Delete(string doctorid)
        {
            int tbleId = _getDoctorList.Find(x => x.DoctorId == doctorid).FirstOrDefault().Id;
            var result = _doctorRepo.Delete(tbleId);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }
        
        //---------------- Doctor Availability
        [Route("api/doctor/doctorAvailablity")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage DoctorAvailablity(DoctorAvailableTime obj)
        {
            var _Created = _doctorAvailabilityRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, obj.Id);
        }

        [Route("api/doctor/getDoctorAvailablity/{doctorid}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getDoctorAvailablity(string doctorid)
        {
            var result = _doctorAvailabilityRepo.Find(x => x.DoctorId == doctorid).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/doctor/doctorDetails/{doctorid}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage doctorDetails(string doctorid)
        {
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            Doctor d = _doctorRepo.Find(x => x.DoctorId == doctorid).FirstOrDefault();
            var feedback = _feedbackRepo.Find(x => x.PageId == doctorid);
            var hospitalService = _hospitalServicesRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
            var hospitalAmenitie = _hospitalAmenitieRepository.GetAll().OrderBy(x => x.HospitalAmenities).ToList();
            HospitalDetails hospitals = _hospitaldetailsRepo.Find(x => x.HospitalId == d.HospitalId).FirstOrDefault();
            if (d != null)
            {
                Doctors _doctor = new Doctors
                {
                    DoctorId = d.DoctorId,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    AlternatePhoneNumber = d.AlternatePhoneNumber,
                    Gender = d.Gender,
                    Experience = d.Experience,
                    FeeMoney = d.FeeMoney,
                    Language = d.Language,
                    AgeGroupGender = d.AgeGroupGender,
                    Degree = d.Degree,
                    AboutUs = d.AboutUs,
                    HospitalName = hospitals.HospitalName,
                    aboutMe = d.AboutUs,
                    DoctorAvilability = _doctorAvailabilityRepo.Find(x => x.DoctorId == d.DoctorId),
                    Specialization = getSpecialization(d.Specialization, disease),
                    Amenities = getHospitalAmenities(hospitals.Amenities, hospitalAmenitie),
                    Services = getHospitalService(hospitals.Services, hospitalService),
                    Feedback = _feedbackRepo.Find(x => x.PageId == doctorid),
                    Likes = _feedbackRepo.Find(x => x.PageId == doctorid && x.ILike == true).Count(),
                    location = "",
                    ImgUrl = $"{constant.imgUrl}/Doctor/{d.DoctorId}.Jpeg",
                    website = hospitals.Website,
                    Address = hospitals.Address
                };
                return Request.CreateResponse(HttpStatusCode.Accepted, _doctor);

            }
            return Request.CreateResponse(HttpStatusCode.NotFound);

        }


        [Route("api/result/{type?}/{cityId?}/{countryId?}/{diseaseType?}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getDoctorDetail(string type = "0",string cityId = "0", string countryId = "0", string diseaseType = "0")
        {
            FilterDoctor _filterDoctor = new FilterDoctor();
            FilterHospital _filterHospital = new FilterHospital();
            result _result = new result();
            _result.Hospitals = getHospital(type, cityId, countryId, diseaseType, ref _filterDoctor, ref _filterHospital);
            _result.FilterDoctor = type == "0"? _filterDoctor : null;
            _result.FilterHospital = _filterHospital;
            return Request.CreateResponse(HttpStatusCode.Accepted, _result);
        }
        
        #region Uility
        private List<Disease> getSpecialization(string diesiesType, List<Disease> diseases)
        {
            var diesiesTypes = diesiesType.Split(',');
            int[] myInts = Array.ConvertAll(diesiesTypes, s => int.Parse(s));
            var diseasesList = diseases.Where(x => myInts.Contains(x.Id)).ToList();
            return diseasesList;
        }

        private List<TblHospitalServices> getHospitalService(string serviceType, List<TblHospitalServices> hospitalService)
        {
            var serviceTypes = serviceType.Split(',');
            int[] myInts = Array.ConvertAll(serviceTypes, s => int.Parse(s));
            var hospitalServiceList = hospitalService.Where(x => myInts.Contains(x.Id)).ToList();

            return hospitalServiceList;
        }

        private List<TblHospitalAmenities> getHospitalAmenities(string amenitieType, List<TblHospitalAmenities> hospitalAmenitie)
        {
            var serviceTypes = amenitieType.Split(',');
            int[] myInts = Array.ConvertAll(serviceTypes, s => int.Parse(s));
            var hospitalAmenitieList = hospitalAmenitie.Where(x => myInts.Contains(x.Id)).ToList();

            return hospitalAmenitieList;
        }
        private List<Doctors> getDoctors(string HospitalId, string diseaseType, ref FilterDoctor _filterDoctor)
        {
            var diesiesTypes = diseaseType.Split(',');

            int[] myInts = Array.ConvertAll(diesiesTypes, s => int.Parse(s));
            List<Disease> _disease = new List<Disease>();
            List<decimal> _priceses = new List<decimal>();
            Doctors _doctor = new Doctors();
            List<Doctors> _doctors = new List<Doctors>();
            List<Doctor> doctors = _doctorRepo.Find(x => x.HospitalId == HospitalId)
             .Where(x=> x.Specialization.Where(s=> diesiesTypes.Contains(x.Specialization)).ToList().Count() > 0).ToList();
            //.Where(x => x.Specialization.Where(c => myInts.Contains(c)).ToList().Count() > 0).ToList();
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            foreach (var d in doctors ?? new List<Doctor>())
            {
                var feedback = _feedbackRepo.Find(x => x.PageId == d.DoctorId);
                _doctor = new Doctors
                {
                    DoctorId = d.DoctorId,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    AlternatePhoneNumber = d.AlternatePhoneNumber,
                    Gender = d.Gender,
                    Experience = d.Experience,
                    FeeMoney = d.FeeMoney,
                    Language = d.Language,
                    AgeGroupGender = d.AgeGroupGender,
                    Degree = d.Degree,
                    SpecializationIds = Array.ConvertAll(d.Specialization.Split(','), s => int.Parse(s)),//d.Specialization,
                    Specialization = getSpecialization(d.Specialization, disease),
                    AboutUs = d.AboutUs,
                    Likes = feedback.Where(x => x.ILike == true).Count(),
                    Feedbacks = feedback.Count(),
                    BookingUrl = $"booking/{d.DoctorId}",
                    ProfileDetailUrl = $"doctorDetails/{d.DoctorId}",
                    ImgUrl = $"{constant.imgUrl}/Doctor/{d.DoctorId}.Jpeg"
                };

                // Add Filter Value
                _priceses.Add(d.FeeMoney);
                _disease.AddRange(_doctor.Specialization);
                _doctors.Add(_doctor);
            }
            _filterDoctor.Price = _priceses;
            _filterDoctor.Specialization = _disease.Select(x => new FilterData { Id = x.Id, Name = x.DiseaseType }).Distinct().ToList();
            return _doctors;
        }
        private List<Hospital> getHospital(string type, string cityId, string countryId, string diseaseType, ref FilterDoctor _filterDoctor, ref FilterHospital _filterHospital)
        {
            var hospitalService = _hospitalServicesRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
            var hospitalAmenitie = _hospitalAmenitieRepository.GetAll().OrderBy(x => x.HospitalAmenities).ToList();
            Hospital _hospital = new Hospital();
            List<Hospital> _hospitals = new List<Hospital>();
            List<HospitalDetails> hospitals = _hospitaldetailsRepo.Find(x => (cityId != "0" && x.City == cityId) &&
             (countryId != "0" && x.Country == countryId));
            List<TblHospitalServices> _hospitalServices = new List<TblHospitalServices>();
            List<TblHospitalAmenities> _hospitalAmenities = new List<TblHospitalAmenities>();

            foreach (var h in hospitals ?? new List<HospitalDetails>())
            {
               var feedback = _feedbackRepo.Find(x => x.PageId == h.HospitalId);
                _hospital = new Hospital
                {
                    HospitalId = h.HospitalId,
                    HospitalName = h.HospitalName,
                    Mobile = h.Mobile,
                    AlternateNumber = h.AlternateNumber,
                    Website = h.Website,
                    EstablishYear = h.EstablishYear,
                    NumberofBed = h.NumberofBed,
                    NumberofAmbulance = h.NumberofAmbulance,
                    PaymentType = h.PaymentType,
                    Emergency = h.Emergency,
                    FacilityId = h.FacilityId,
                    Address = h.Address,
                    Street = h.Street,
                    Country = h.Country,
                    City = h.City,
                    PostCode = h.PostCode,
                    Landmark = h.Landmark,
                    InsuranceCompanies = h.InsuranceCompanies,
                    AmenitiesIds = Array.ConvertAll(h.Amenities.Split(','), s => int.Parse(s)),
                    Amenities = getHospitalAmenities(h.Amenities, hospitalAmenitie),
                    ServicesIds = Array.ConvertAll(h.Services.Split(','), s => int.Parse(s)),
                    Services = getHospitalService(h.Services, hospitalService),
                    Doctors = type == "0" ? getDoctors(h.HospitalId, diseaseType, ref _filterDoctor) : null,
                    Likes = feedback.Where(x => x.ILike == true).Count(),
                    Feedbacks = feedback.Count(),
                    BookingUrl = $"booking/{h.HospitalId}",
                    ProfileDetailUrl = $"hospitalDetails/{h.HospitalId}",
                    ImgUrl = $"{constant.imgUrl}/Hospital/{h.HospitalId}.Jpeg"
                };
                _hospitalServices.AddRange(_hospital.Services);
                _hospitalAmenities.AddRange(_hospital.Amenities);
                _hospitals.Add(_hospital);
            }
            var Services = _hospitalServices.Select(x => new FilterData { Id = x.Id, Name = x.HospitalServices }).Distinct().ToList();
            _filterDoctor.Services = Services;
            _filterHospital.Services = Services;
            _filterHospital.Amenities = _hospitalAmenities.Select(x => new FilterData { Id = x.Id, Name = x.HospitalAmenities }).Distinct().ToList();
           // _filterHospital.Specialization = _filterDoctor.Specialization;
            return _hospitals;
        }
        #endregion
    }
}

