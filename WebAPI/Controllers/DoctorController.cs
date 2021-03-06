﻿using AngularJSAuthentication.API.Services;
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

        ICountryRepository _countryRepo = RepositoryFactory.Create<ICountryRepository>(ContextTypes.EntityFramework);
        ICityRepository _cityRepo = RepositoryFactory.Create<ICityRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _getDoctorList = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        IDoctorAvailableTimeRepository _doctorAvailabilityRepo = RepositoryFactory.Create<IDoctorAvailableTimeRepository>(ContextTypes.EntityFramework);
        IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
        ITblHospitalServicesRepository _hospitalServicesRepository = RepositoryFactory.Create<ITblHospitalServicesRepository>(ContextTypes.EntityFramework);
        ITblHospitalAmenitiesRepository _hospitalAmenitieRepository = RepositoryFactory.Create<ITblHospitalAmenitiesRepository>(ContextTypes.EntityFramework);
        IFeedbackRepository _feedbackRepo = RepositoryFactory.Create<IFeedbackRepository>(ContextTypes.EntityFramework);
        IAppointmentRepository _appointmentRepo = RepositoryFactory.Create<IAppointmentRepository>(ContextTypes.EntityFramework);


        [Route("api/doctor/IsValidNoorCare/{doctorId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage IsValidNoorCare(string doctorId)
        {
            var result = _doctorRepo.Find(x => x.DoctorId == doctorId);
            if (result.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, true);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, false);
            }
        }

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

        [Route("api/doctorbypatient/{ClientID}")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Doctor
        public HttpResponseMessage doctorbypatient(string ClientID)
        {
            var result = from a in _appointmentRepo.GetAll()
                         join
            d in _getDoctorList.GetAll() on a.DoctorId equals d.DoctorId
                         where a.ClientId == ClientID && a.Status.ToLower() == "booked"
                         select new
                         {
                             Name = d.FirstName + " " + d.FirstName + "_" + d.Degree,
                             Value = d.DoctorId
                         };
            return Request.CreateResponse(HttpStatusCode.Accepted, result.Distinct());
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
        public HttpResponseMessage Register([FromBody]Doctor obj)
        {
            //ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
            //CountryCode countryCode = _countryCodeRepository.Find(x => x.Id == obj.CountryCode).FirstOrDefault();
            //if (countryCode != null)
            //{

            EmailSender _emailSender = new EmailSender();
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            string password = _registration.RandomPassword(6);
            //user registration
            ApplicationUser user = _registration.UserAccount(obj, Convert.ToInt32(obj.CountryCode));
            IdentityResult result = manager.Create(user, password);
            user.PasswordHash = password;

            obj.DoctorId = user.Id;
            obj.EmailConfirmed = true;
            //doctor registration
            IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
            var _doctorCreated = _doctorRepo.Insert(obj);

            //send Email
            try
            {
                _registration.sendRegistrationEmail(user);
                _registration.sendRegistrationMessage(user);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "email");
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, obj.DoctorId);
            //}
            //else
            //{
            //    return Request.CreateResponse(HttpStatusCode.Accepted, "Wrong country code");
            //}
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
        public HttpResponseMessage DoctorAvailablity(DoctorAvailableTime[] obj)
        {
            int id = 0;
            if (obj != null)
            {
                foreach (DoctorAvailableTime element in obj)
                {
                    id = _doctorAvailabilityRepo.Insert(element);

                }
            }
            //var _Created = _doctorAvailabilityRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, id);
        }

        [Route("api/doctor/getDoctorAvailablity/{doctorid}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getDoctorAvailablity(string doctorid)
        {
            var result = _doctorAvailabilityRepo.Find(x => x.DoctorId == doctorid);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/doctor/doctorDetails/{doctorid}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage doctorDetails(string doctorid)
        {
            List<Feedback> feedbacks = new List<Feedback>();
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
                    HospitalId = hospitals.HospitalId,
                    HospitalEmail = hospitals.Email,
                    HospitalAddress = hospitals.Address,
                    HospitalPicUrl = $"{constant.imgUrl}/" + hospitals.ProfilePath,
                    aboutMe = d.AboutUs,
                    DoctorAvilability = _doctorAvailabilityRepo.Find(x => x.DoctorId == d.DoctorId),
                    Specialization = getSpecialization(d.Specialization, disease),
                    Amenities = getHospitalAmenities(hospitals.Amenities, hospitalAmenitie),
                    Services = getHospitalService(hospitals.Services, hospitalService),
                    Feedback = _feedbackRepo.Find(x => x.PageId == doctorid),
                    Likes = _feedbackRepo.Find(x => x.PageId == doctorid && x.ILike == true).Count(),
                    location = "",
                    ImgUrl = $"{constant.imgUrl}/ProfilePic/Doctor/{d.DoctorId}.Jpeg",
                    website = hospitals.Website,
                    Address = hospitals.Address
                };
                return Request.CreateResponse(HttpStatusCode.Accepted, _doctor);

            }
            return Request.CreateResponse(HttpStatusCode.NotFound);

        }


        [Route("api/searchdoctor/{searchType?}/{searchName?}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getSearchDoctorDetail(string searchType, string searchName)
        {
            string type = "0";
            FilterDoctor _filterDoctor = new FilterDoctor();
            FilterHospital _filterHospital = new FilterHospital();
            result _result = new result();
            _result.Hospitals = getHospital("0", "0", "0", "0", "0", searchType, searchName, ref _filterDoctor, ref _filterHospital);
            _result.FilterDoctor = type == "0" ? _filterDoctor : null;
            _result.FilterHospital = _filterHospital;
            return Request.CreateResponse(HttpStatusCode.Accepted, _result);
        }

        [Route("api/result/{type?}/{cityId?}/{countryId?}/{diseaseType?}/{hospitalType?}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getDoctorDetail(string type = "0", string cityId = "0", string countryId = "0", string diseaseType = "0", string hospitalType = "0")
        {
            FilterDoctor _filterDoctor = new FilterDoctor();
            FilterHospital _filterHospital = new FilterHospital();
            result _result = new result();
            _result.Hospitals = getHospital(type, cityId, countryId, diseaseType, hospitalType, null, null, ref _filterDoctor, ref _filterHospital);
            _result.FilterDoctor = type == "0" ? _filterDoctor : null;
            _result.FilterHospital = _filterHospital;
            return Request.CreateResponse(HttpStatusCode.Accepted, _result);
        }
        private List<Hospital> getHospital(string type, string cityId, string countryId, string diseaseType, string hospitalType, string searchType, string searchName, ref FilterDoctor _filterDoctor, ref FilterHospital _filterHospital)
        {
            int[] a = new int[0];
            var hospitalService = _hospitalServicesRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
            var hospitalAmenitie = _hospitalAmenitieRepository.GetAll().OrderBy(x => x.HospitalAmenities).ToList();
            Hospital _hospital = new Hospital();
            List<Hospital> _hospitals = new List<Hospital>();
            List<HospitalDetails> hospitals = new List<HospitalDetails>();
            string searchtext = "null";
            if (string.IsNullOrEmpty(type))
                type = "0";

            int facilityid = Convert.ToInt32(type);
            if (searchName != null)
            {
                searchtext = searchName.Split('(')[0];
            }

            if (searchType == "1")
            {
                hospitals = _hospitaldetailsRepo.Find(x => x.HospitalName.Contains(searchtext) && x.EmailConfirmed == true);
            }
            else if (searchType == "2")
            {
                var docId = searchName.Split('(')[1].Split(')')[0];
                searchtext = docId;
                var doctor = _doctorRepo.GetAll().Where(x => x.DoctorId == docId).FirstOrDefault();
                if (doctor != null)
                {
                    hospitals = _hospitaldetailsRepo.Find(x => x.HospitalId == doctor.HospitalId && x.EmailConfirmed == true).ToList();
                };
            }
            else
            {
                if (cityId != "null")
                {

                    if (type == "2")
                    {
                        hospitals = _hospitaldetailsRepo.Find(x => (cityId != "0" && x.City == cityId) &&
                 (countryId != "0" && x.Country == countryId) && x.Type == hospitalType && x.FacilityId == 2);
                    }
                    else if (type == "0")
                    {
                        hospitals = _hospitaldetailsRepo.Find(x => (cityId != "0" && x.City == cityId) &&
                    (countryId != "0" && x.Country == countryId  && x.IsDocumentApproved == 1));

                    }
                    else
                    {
                        hospitals = _hospitaldetailsRepo.Find(x => (cityId != "0" && x.City == cityId) &&
                 (countryId != "0" && x.Country == countryId && x.FacilityId == facilityid && x.IsDocumentApproved == 1));
                    }

                }
                else
                {
                    if (type == "2")
                    {
                        hospitals = _hospitaldetailsRepo.Find(x => countryId != "0" && x.Country == countryId && x.Type == hospitalType && x.FacilityId == facilityid && x.IsDocumentApproved == 1);
                    }
                    else if (type == "0")
                    {
                        hospitals = _hospitaldetailsRepo.Find(x => countryId != "0" && x.Country == countryId && x.IsDocumentApproved == 1);
                    }
                    else
                    {
                        hospitals = _hospitaldetailsRepo.Find(x => countryId != "0" && x.Country == countryId && x.Type == hospitalType && x.FacilityId == facilityid && x.IsDocumentApproved == 1);
                    }

                    //hospitals = _hospitaldetailsRepo.Find(x => countryId != "0" && x.Country == countryId && x.Type == hospitalType && x.IsDocumentApproved == 1);
                }
            }
            List<TblHospitalServices> _hospitalServices = new List<TblHospitalServices>();
            List<TblHospitalAmenities> _hospitalAmenities = new List<TblHospitalAmenities>();
            if (searchType != "2")
            {
                foreach (var h in hospitals ?? new List<HospitalDetails>())
                {
                    var feedback = _feedbackRepo.Find(x => x.PageId == h.HospitalId);

                    _hospital = new Hospital();

                    _hospital.HospitalId = h.HospitalId;
                    _hospital.HospitalName = h.HospitalName;
                    _hospital.Mobile = h.Mobile;
                    _hospital.AlternateNumber = h.AlternateNumber;
                    _hospital.Website = h.Website;
                    _hospital.EstablishYear = h.EstablishYear;
                    _hospital.NumberofBed = h.NumberofBed;
                    _hospital.NumberofAmbulance = h.NumberofAmbulance;
                    _hospital.PaymentType = h.PaymentType;
                    _hospital.Emergency = h.Emergency;
                    _hospital.FacilityId = h.FacilityId;
                    _hospital.JobType = h.jobType;
                    _hospital.Address = h.Address;
                    _hospital.Street = h.Street;
                    _hospital.Country = GetCountryName(Convert.ToInt16(h.Country));
                    _hospital.City = GetCityName(Convert.ToInt16(h.City));
                    _hospital.PostCode = h.PostCode;
                    _hospital.Landmark = h.Landmark;
                    _hospital.InsuranceCompanies = h.InsuranceCompanies ?? "";
                    _hospital.AmenitiesIds = h.Amenities == null ? a : Array.ConvertAll(h.Amenities.Split(','), s => int.Parse(s));
                    _hospital.Amenities = getHospitalAmenities(h.Amenities, hospitalAmenitie);
                    _hospital.ServicesIds = h.Services == null ? a : Array.ConvertAll(h.Services.Split(','), s => int.Parse(s));
                    _hospital.Services = getHospitalService(h.Services, hospitalService);
                    _hospital.Doctors = type == "0" ? getDoctors(h.HospitalId, diseaseType, searchtext, ref _filterDoctor) : null;
                    _hospital.Likes = feedback.Where(x => x.ILike == true).Count();
                    _hospital.Feedbacks = feedback.Count();
                    _hospital.BookingUrl = $"booking/{h.HospitalId}";
                    _hospital.ProfileDetailUrl = $"hospitalDetails/{h.HospitalId}";
                    _hospital.ImgUrl = h.ProfilePath == null ? $"{constant.imgUrl}/ProfilePic/Hospital/{h.HospitalId}.Jpeg" : $"{constant.imgUrl}/{h.ProfilePath}";
                    _hospitalServices.AddRange(_hospital.Services);
                    _hospitalAmenities.AddRange(_hospital.Amenities);
                    _hospitals.Add(_hospital);
                }
            }
            else
            {
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
                        JobType = h.jobType,
                        Address = h.Address,
                        Street = h.Street,
                        Country = GetCountryName(Convert.ToInt16(h.Country)),
                        City = GetCityName(Convert.ToInt16(h.City)),
                        PostCode = h.PostCode,
                        Landmark = h.Landmark,
                        InsuranceCompanies = h.InsuranceCompanies ?? "",
                        AmenitiesIds = h.Amenities == null ? a : Array.ConvertAll(h.Amenities.Split(','), s => int.Parse(s)),
                        Amenities = getHospitalAmenities(h.Amenities, hospitalAmenitie),
                        ServicesIds = h.Services == null ? a : Array.ConvertAll(h.Services.Split(','), s => int.Parse(s)),
                        Services = getHospitalService(h.Services, hospitalService),
                        Doctors = getDoctors(h.HospitalId, diseaseType, searchtext, ref _filterDoctor),
                        Likes = feedback.Where(x => x.ILike == true).Count(),
                        Feedbacks = feedback.Count(),
                        BookingUrl = $"booking/{h.HospitalId}",
                        ProfileDetailUrl = $"hospitalDetails/{h.HospitalId}",
                        ImgUrl = $"{constant.imgUrl}/ProfilePic/Hospital/{h.HospitalId}.Jpeg"
                    };
                    _hospitalServices.AddRange(_hospital.Services);
                    _hospitalAmenities.AddRange(_hospital.Amenities);
                    _hospitals.Add(_hospital);
                }
            }
            var Services = _hospitalServices.Select(x => new FilterData { Id = x.Id, Name = x.HospitalServices }).Distinct().ToList();
            _filterDoctor.Services = Services;
            _filterHospital.Services = Services;
            _filterHospital.Amenities = _hospitalAmenities.Select(x => new FilterData { Id = x.Id, Name = x.HospitalAmenities }).Distinct().ToList();
            // _filterHospital.Specialization = _filterDoctor.Specialization;
            bool isDoctors = false;
            if (type == "0" && searchName == null)
            {
                foreach (var item in _hospitals)
                {
                    if (item.Doctors.Count > 0)
                    {
                        isDoctors = true;
                    }
                }
                if (isDoctors == false)
                {
                    _hospitals = new List<Hospital>();
                }
            }
            return _hospitals;
        }
        #region Uility
        private List<Disease> getSpecialization(string diesiesType, List<Disease> diseases)
        {
            if (diesiesType == null || diesiesType == "")
            {
                return new List<Disease>();
            }
            var diesiesTypes = diesiesType.Split(',');
            int[] myInts = Array.ConvertAll(diesiesTypes, s => int.Parse(s));
            var diseasesList = diseases.Where(x => myInts.Contains(x.Id)).ToList();
            return diseasesList;
        }

        private List<TblHospitalServices> getHospitalService(string serviceType, List<TblHospitalServices> hospitalService)
        {
            if (serviceType == null)
            {
                return new List<TblHospitalServices>();
            }
            var serviceTypes = serviceType.Split(',');
            int[] myInts = Array.ConvertAll(serviceTypes, s => int.Parse(s));
            var hospitalServiceList = hospitalService.Where(x => myInts.Contains(x.Id)).ToList();

            return hospitalServiceList;
        }

        private List<TblHospitalAmenities> getHospitalAmenities(string amenitieType, List<TblHospitalAmenities> hospitalAmenitie)
        {
            if (amenitieType == null)
            {
                return new List<TblHospitalAmenities>();
            }
            var serviceTypes = amenitieType.Split(',');
            int[] myInts = Array.ConvertAll(serviceTypes, s => int.Parse(s));
            var hospitalAmenitieList = hospitalAmenitie.Where(x => myInts.Contains(x.Id)).ToList();

            return hospitalAmenitieList;
        }
        private List<Doctors> getDoctors(string HospitalId, string diseaseType, string searchtext, ref FilterDoctor _filterDoctor)
        {
            if (searchtext == "null")
            {
                if (diseaseType == null || diseaseType == "" || diseaseType == "null")
                {
                    return new List<Doctors>();
                }
            }
            var diesiesTypes = diseaseType.Split(',');
            //int[] myInts = Array.ConvertAll(diesiesTypes, s => int.Parse(s));
            List<Disease> _disease = new List<Disease>();
            List<decimal> _priceses = new List<decimal>();
            Doctors _doctor = new Doctors();
            List<Doctors> _doctors = new List<Doctors>();
            List<Doctor> doctors = new List<Doctor>();
            try
            {


                if (searchtext != "" && searchtext != "null")
                {

                    doctors = _doctorRepo.Find(x => x.HospitalId == HospitalId && x.DoctorId == searchtext);

                }
                else
                {
                    var doctorsList = _doctorRepo.Find(x => x.HospitalId == HospitalId);
                    doctors = doctorsList.Where(x => x.Specialization.Contains(diesiesTypes[0])).ToList<Doctor>();
                }
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
                        ImgUrl = d.PhotoPath == null ? $"{constant.imgUrl}/Doctor/{d.DoctorId}.Jpeg" : $"{constant.imgUrl}/{d.PhotoPath}"
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
            catch (Exception)
            {

                return _doctors;
            }
        }
        private string GetCountryName(Int32 id)
        {
            string countryName = "N/A";
            var country = _countryRepo.Find(x => x.Id == id).FirstOrDefault();
            if (country != null)
            {
                countryName = country.CountryName;
            }
            return countryName;
        }
        private string GetCityName(int id)
        {
            string cityName = "N/A";
            var city = _cityRepo.Find(x => x.Id == id).FirstOrDefault();
            if (city != null)
            {
                cityName = city.City;
            }
            return cityName;
        }
        #endregion
        [Route("api/doctorbyhospital/{HospitalID}")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Doctor
        public HttpResponseMessage doctorbyhospital(string HospitalID)
        {
            var doctors = _getDoctorList.GetAll().Where(x => x.HospitalId == HospitalID && x.IsDeleted == false).ToList();
            var docList = from d in doctors
                          select new
                          {
                              Name = d.FirstName + " " + d.LastName + "("+ d.DoctorId + ") " + d.Degree,
                              DoctorId = d.DoctorId
                          };
            return Request.CreateResponse(HttpStatusCode.Accepted, docList.ToList().Distinct());
        }

       
    }
}