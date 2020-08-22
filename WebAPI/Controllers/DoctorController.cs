using AngularJSAuthentication.API.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
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
using WebGrease.Css.Extensions;

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
        ILikeVisitorRepository _likeVisitorRepo = RepositoryFactory.Create<ILikeVisitorRepository>(ContextTypes.EntityFramework);
        ITimeMasterRepository _timeMasterRepo = RepositoryFactory.Create<ITimeMasterRepository>(ContextTypes.EntityFramework);
        IDoctorEducationRepository _doctorEducationRep = RepositoryFactory.Create<IDoctorEducationRepository>(ContextTypes.EntityFramework);

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
            var result = _doctorRepo.Find(x => x.EmailConfirmed == true && x.IsDeleted == false).ToList();
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

        [Route("api/hospitalbypatient/{ClientID}")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Doctor
        public HttpResponseMessage hospitalbypatient(string ClientID)
        {
            var result = from a in _appointmentRepo.GetAll()
                         join
            d in _hospitaldetailsRepo.GetAll() on a.HospitalId equals d.HospitalId
                         where a.ClientId == ClientID && a.Status.ToLower() == "booked"
                         select new
                         {
                             Name = d.HospitalName,
                             Value = d.HospitalId
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
        public HttpResponseMessage Register([FromBody] Doctor obj)
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

                    //update profile photo
                    var result = _doctorRepo.Find(x => x.DoctorId == doctorId).FirstOrDefault();
                    if (result != null)
                    {
                        result.PhotoPath = "ProfilePic/Doctor/" + imageName;
                        var flag = _doctorRepo.Update(result);
                    }

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
            obj.EmailConfirmed = _getDoctorList.Find(x => x.DoctorId == obj.DoctorId).FirstOrDefault().EmailConfirmed;
            obj.AboutUs = _getDoctorList.Find(x => x.DoctorId == obj.DoctorId).FirstOrDefault().AboutUs;
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

                    DoctorAvailableTime doctorAvailable = _doctorAvailabilityRepo.Find(x => x.DoctorId == element.DoctorId && x.Days == element.Days && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                    if (doctorAvailable != null)
                    {
                        doctorAvailable.TimeId = element.TimeId;
                        var res = _doctorAvailabilityRepo.Update(doctorAvailable);
                    }
                    else
                    {
                        id = _doctorAvailabilityRepo.Insert(element);
                    }
                }
            }
            //var _Created = _doctorAvailabilityRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, id);
        }

        [Route("api/doctor/getDoctorAvailablity/{doctorid}/{date}")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult getDoctorAvailablity(string doctorid, string date)
        {
            try
            {
                DateTime calDate = Convert.ToDateTime(date, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

                List<DoctorScheduleTime> result = new List<DoctorScheduleTime>();
                //result = docAvailablity(doctorid);

                ApplicationDbContext dbContext = new ApplicationDbContext();
                var leaveDtls = dbContext.Database.SqlQuery<LeaveSchedule>(" set dateformat dmy;" +
                            " ; WITH CTE AS (" +
                            " SELECT ROW_NUMBER() OVER(ORDER BY ClientId) AS RowNo, 1 AS IterationID, TimeId, FromDate, ToDate" +
                            " from [dbo].[LeaveDetail] where clientid = '" + doctorid + "'" +
                            " and fromDate >= '" + calDate + "' and toDate <= '" + calDate.AddDays(6) + "'" +
                            " UNION ALL" +
                            " SELECT RowNo, IterationID + 1, TimeId, DATEADD(dd, 1, FromDate)  AS FromDate, ToDate" +
                            " FROM CTE WHERE FromDate < ToDate)" +
                            " select distinct x.FromDate as 'SchDate',(select top 1 TimeId from CTE where FromDate = x.FromDate order by len(TimeId) Desc) 'TimeIds' from CTE x" +
                            " order by FromDate").ToList();

                var docAvail = _appointmentRepo.GetAll().Where(x => x.IsDeleted == false && x.AppointmentDate >= calDate && x.AppointmentDate < calDate.AddDays(6) && x.DoctorId == doctorid && x.Status.ToLower().Trim()!= "rejected").ToList();
                List<DoctorAvailablity> docAvlList = new List<DoctorAvailablity>();
                for (int i = 0; i < 7; i++)
                {
                    DateTime schDate = i == 0 ? calDate : calDate.AddDays(i);
                    result = docAvailablityDayWise(doctorid, schDate.DayOfWeek.ToString());
                    //string serialized = JsonConvert.SerializeObject(result);
                    //var copy = JsonConvert.DeserializeObject<List<DoctorScheduleTime>>(serialized);

                    DoctorAvailablity obj = new DoctorAvailablity();
                    obj.SchDate = schDate;
                    obj.SchTime = result;
                    docAvlList.Add(obj);
                }

                foreach (var item in docAvlList)
                {
                    //already booked appointment
                    docAvail.Where(x => x.AppointmentDate == item.SchDate).ForEach(a => (item.SchTime.Where(it => it.TimeId == Convert.ToInt32(a.TimingId)).ToList()).ForEach(ob => ob.IsBooked = true));

                    //leave management
                    leaveDtls.Where(l => l.SchDate == item.SchDate).ForEach(ld =>
                    {
                        var timeIds = ld.TimeIds.Split(',');
                        int[] myInts = Array.ConvertAll(timeIds, s => int.Parse(s));
                        item.SchTime.Where(it => myInts.Contains(it.TimeId)).ToList().ForEach(ob => ob.IsLeave = true);
                    });
                }
                return Ok(docAvlList);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/doctor/availablity/{doctorid}")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult getAvailablity(string doctorid)
        {
            var result = docAvailablity(doctorid);
            return Ok(result);
        }

        private List<DoctorScheduleTime> docAvailablity(string doctorid)
        {
            var result = _doctorAvailabilityRepo.GetAll().Where(x => x.DoctorId == doctorid.Trim()).FirstOrDefault();

            if (result != null)
            {
                if (result.TimeId == null || result.TimeId == "")
                {
                    return null;
                }
                var timeIds = result.TimeId.Split(',');
                int[] myInts = Array.ConvertAll(timeIds, s => int.Parse(s));
                var timeList = _timeMasterRepo.GetAll().Where(x => myInts.Contains(x.Id)).ToList();
                var dataObj = (from d in timeList
                               select new
                               {
                                   TimeId = d.Id,
                                   TimeDesc = d.TimeFrom.Trim() + '-' + d.TimeTo.Trim() + ' ' + d.AM_PM.Trim()
                               }).ToList();
                List<DoctorScheduleTime> doctorSchedules = new List<DoctorScheduleTime>();
                foreach (var item in dataObj)
                {
                    DoctorScheduleTime obj = new DoctorScheduleTime();
                    obj.TimeId = item.TimeId;
                    obj.TimeDesc = item.TimeDesc;
                    doctorSchedules.Add(obj);
                }

                return doctorSchedules;
            }
            else
            {
                var timeMasterList = _timeMasterRepo.GetAll().Where(x => x.IsActive == true).ToList();
                var dataObj = (from d in timeMasterList
                               select new
                               {
                                   TimeId = d.Id,
                                   TimeDesc = d.TimeFrom.Trim() + '-' + d.TimeTo.Trim() + ' ' + d.AM_PM.Trim()
                               }).ToList();
                List<DoctorScheduleTime> doctorSchedules = new List<DoctorScheduleTime>();
                foreach (var item in dataObj)
                {
                    DoctorScheduleTime obj = new DoctorScheduleTime();
                    obj.TimeId = item.TimeId;
                    obj.TimeDesc = item.TimeDesc;
                    doctorSchedules.Add(obj);
                }

                return doctorSchedules;
            }
        }

        private List<DoctorScheduleTime> docAvailablityDayWise(string doctorid, string day)
        {
            var result = _doctorAvailabilityRepo.GetAll().Where(x => x.DoctorId == doctorid.Trim() && x.Days.ToLower().Trim() == day.ToLower().Trim() && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();

            if (result != null)
            {
                //if (result.TimeId == null || result.TimeId == "" || result.TimeId == "0")
                //{
                //    return null;
                //}
                var timeIds = result.TimeId.Split(',');
                int[] myInts = Array.ConvertAll(timeIds, s => int.Parse(s));
                var timeList = _timeMasterRepo.GetAll().Where(x => myInts.Contains(x.Id)).ToList();
                var dataObj = (from d in timeList
                               select new
                               {
                                   TimeId = d.Id,
                                   TimeDesc = d.TimeFrom.Trim() + '-' + d.TimeTo.Trim() + ' ' + d.AM_PM.Trim()
                               }).ToList();
                List<DoctorScheduleTime> doctorSchedules = new List<DoctorScheduleTime>();
                foreach (var item in dataObj)
                {
                    DoctorScheduleTime obj = new DoctorScheduleTime();
                    obj.TimeId = item.TimeId;
                    obj.TimeDesc = item.TimeDesc;
                    doctorSchedules.Add(obj);
                }

                return doctorSchedules;
            }
            else
            {
                var timeMasterList = _timeMasterRepo.GetAll().Where(x => x.IsActive == true).ToList();
                var dataObj = (from d in timeMasterList
                               select new
                               {
                                   TimeId = d.Id,
                                   TimeDesc = d.TimeFrom.Trim() + '-' + d.TimeTo.Trim() + ' ' + d.AM_PM.Trim()
                               }).ToList();
                List<DoctorScheduleTime> doctorSchedules = new List<DoctorScheduleTime>();
                foreach (var item in dataObj)
                {
                    DoctorScheduleTime obj = new DoctorScheduleTime();
                    obj.TimeId = item.TimeId;
                    obj.TimeDesc = item.TimeDesc;
                    doctorSchedules.Add(obj);
                }

                return doctorSchedules;
            }
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
            var degree = _doctorEducationRep.GetAll().OrderBy(x => x.Education).ToList();
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
                    Education = getDegree(d.Degree, degree),
                    AboutUs = d.AboutUs,
                    HospitalName = hospitals.HospitalName,
                    HospitalId = hospitals.HospitalId,
                    HospitalEmail = hospitals.Email,
                    HospitalAddress = hospitals.Address,
                    HospitalPicUrl = $"{constant.imgUrl}/" + hospitals.ProfilePath,
                    aboutMe = d.AboutUs,
                    DoctorAvilability = _doctorAvailabilityRepo.Find(x => x.DoctorId == d.DoctorId && x.IsActive == true),
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

        [Route("api/doctor/aboutus")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult UpdateAboutus()
        {
            var httpRequest = HttpContext.Current.Request;
            string doctorId = httpRequest.Form["ClientId"];
            string aboutus = httpRequest.Form["Aboutus"];
            Doctor _doctor = _doctorRepo.Find(x => x.DoctorId == doctorId).FirstOrDefault();
            if (_doctor != null)
            {
                _doctor.AboutUs = aboutus;
                var result = _doctorRepo.Update(_doctor);
                return Ok(result);
            }

            return Ok();
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
                    (countryId != "0" && x.Country == countryId && x.IsDocumentApproved == 1));

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

        private List<DoctorEducation> getDegree(string degreeIds, List<DoctorEducation> _degree)
        {
            if (degreeIds == null || degreeIds == "")
            {
                return new List<DoctorEducation>();
            }
            var degreeList = degreeIds.Split(',');
            int[] myInts = Array.ConvertAll(degreeList, s => int.Parse(s));
            var education = _degree.Where(x => myInts.Contains(x.Id)).ToList();
            return education;
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
                        ImgUrl = String.IsNullOrWhiteSpace(d.PhotoPath) ? $"{constant.imgUrl}/Doctor/{d.DoctorId}.Jpeg" : $"{constant.imgUrl}/{d.PhotoPath}"
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
                              Name = d.FirstName + " " + d.LastName + "(" + d.DoctorId + ") " + d.Degree,
                              DoctorId = d.DoctorId
                          };
            return Request.CreateResponse(HttpStatusCode.Accepted, docList.ToList().Distinct());
        }


        [Route("api/search")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage getSearchResult([FromBody] SearchFilter searchFilter)
        {
            result _result = new result();
            if (searchFilter != null)
            {
                if (searchFilter.HealthProvider == 3)
                {
                    //doctor Search
                    _result.Doctors = getDoctorList(searchFilter);
                }
                else
                {
                    //Hospital Search 
                    _result.Hospitals = getHospitalList(searchFilter);
                }
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, _result);
        }

        private List<Doctors> getDoctorList(SearchFilter searchFilter)
        {
            List<Doctor> doctors = new List<Doctor>();
            List<Doctors> _doctors = new List<Doctors>();
            Doctors _doctor = new Doctors();
            var hospitalService = _hospitalServicesRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
            var docObj = _doctorRepo.Find(x => x.EmailConfirmed == true).ToList();
            var degree = _doctorEducationRep.GetAll().OrderBy(x => x.Education).ToList();
            try
            {
                //country
                if (searchFilter.CountryId > 0)
                {
                    docObj = docObj.Where(x => x.CountryCode == searchFilter.CountryId).ToList();
                }

                //city
                if (searchFilter.CityID > 0)
                {
                    docObj = (from p in docObj
                              join d in _hospitaldetailsRepo.GetAll() on p.HospitalId equals d.HospitalId
                              where (d.City == searchFilter.CityID.ToString())
                              select p).ToList();
                }

                //hospitalid
                if (searchFilter.HospitalID != null && searchFilter.HospitalID.Length > 0)
                {
                    //docObj = docObj.Where(x => x.HospitalId == searchFilter.HospitalID).ToList();
                    docObj = docObj.Where(x => x.HospitalId.Split(',').Select(ele => ele.Trim()).
                        Any(ele => searchFilter.HospitalID.Contains(ele))).ToList();
                }

                //by doctorid
                if (searchFilter.DoctorID != null && searchFilter.DoctorID.Length > 0)
                {
                    //docObj = docObj.Where(x => x.DoctorId == searchFilter.DoctorID).ToList();
                    docObj = docObj.Where(x => x.DoctorId.Split(',').Select(ele => ele.Trim()).
                        Any(ele => searchFilter.DoctorID.Contains(ele))).ToList();
                }


                //by doctorname
                if (searchFilter.DoctorName != null && searchFilter.DoctorName.Length > 0)
                {
                    docObj = docObj.Where(x => x.FirstName.ToLower().Contains(searchFilter.DoctorName.ToLower()) || x.LastName.ToLower().Contains(searchFilter.DoctorName.ToLower())).ToList();
                }
                //by gender
                if (searchFilter.DoctorGender != null && searchFilter.DoctorGender.Length > 0)
                {
                    //docObj = docObj.Where(x => x.Gender == searchFilter.DoctorGender).ToList();
                    docObj = docObj.Where(x => x.Gender.ToString() != null && x.Gender.ToString().Split(',').Select(ele => ele.Trim()).
                        Any(ele => searchFilter.DoctorGender.Contains(ele))).ToList();
                }
                //looking for
                if (searchFilter.LookingFor != null && searchFilter.LookingFor.Length > 0)
                {
                    //docObj = docObj.Where(x => x.AgeGroupGender.ToLower().Contains(searchFilter.LookingFor.ToLower())).ToList();
                    docObj = docObj.Where(x => x.AgeGroupGender != null && x.AgeGroupGender.Split(',').Select(ele => ele.Trim()).
                       Any(ele => searchFilter.LookingFor.Contains(ele))).ToList();
                }
                //by language
                if (searchFilter.DoctorLanguage != null && searchFilter.DoctorLanguage.Length > 0)
                {
                    //docObj = docObj.Where(x => x.Language.ToLower().Contains(searchFilter.DoctorLanguage.ToLower())).ToList();
                    docObj = docObj.Where(x => x.Language != null && x.Language.Split(',').Select(ele => ele.Trim()).
                      Any(ele => searchFilter.DoctorLanguage.Contains(ele))).ToList();
                }
                //by diesiesTypes
                if (searchFilter.DiseaseType != null && searchFilter.DiseaseType.Length > 0)
                {
                    //docObj = docObj.Where(x => x.Specialization.ToLower().Contains(searchFilter.DiseaseType.ToLower())).ToList();
                    docObj = docObj.Where(x => x.Specialization != null && x.Specialization.Split(',').Select(ele => ele.Trim()).
                      Any(ele => searchFilter.DiseaseType.Contains(ele))).ToList();
                }

                //by price 
                if (searchFilter.ByPriceMax > 0 && searchFilter.ByPriceMin >= 0)
                {
                    docObj = docObj.Where(x => x.FeeMoney >= searchFilter.ByPriceMin && x.FeeMoney <= searchFilter.ByPriceMax).ToList();
                }
                //by Exp 
                try
                {
                    if (searchFilter.ByMaxExp > 0 && searchFilter.ByMinExp >= 0)
                    {
                        docObj = docObj.Where(x => Convert.ToInt32(x.Experience) >= searchFilter.ByMinExp && Convert.ToInt32(x.Experience) <= searchFilter.ByPriceMax).ToList();
                    }
                }
                catch (Exception ex) { }

                //by services
                if (searchFilter.Services != null && searchFilter.Services.Length > 0)
                {
                    docObj = (from p in docObj
                              join d in _doctorAvailabilityRepo.GetAll() on p.DoctorId equals d.DoctorId
                              where (d.TimeId == null)
                              select p).ToList();
                }

                //by insurance
                if (searchFilter.InsuranceId != null && searchFilter.InsuranceId.Length > 0)
                {
                    //objHosp = objHosp.Where(x => x.Services.ToLower().Contains(searchFilter.Services.ToLower())).ToList();
                    var hospObj = _hospitaldetailsRepo.GetAll().Where(x => x.InsuranceId != null && x.InsuranceId.Split(',').Select(ele => ele.Trim()).
                     Any(ele => searchFilter.InsuranceId.Contains(ele))).ToList();

                    docObj = (from p in docObj
                              join d in hospObj on p.HospitalId equals d.HospitalId
                              select p).ToList();

                    // docObj = docObj.Where(x => x.InsuranceId.Split(',').Select(ele => ele.Trim()).
                    //Any(ele => searchFilter.InsuranceId.Contains(ele))).ToList();
                }


                var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();

                doctors = docObj.ToList();
                foreach (var d in doctors ?? new List<Doctor>())
                {
                    var hospRepo = _hospitaldetailsRepo.Find(x => x.HospitalId == d.HospitalId).FirstOrDefault();
                    var feedback = _feedbackRepo.Find(x => x.PageId == d.DoctorId);
                    int likeCount = _likeVisitorRepo.Find(x => x.HFP_DOC_NCID.Trim() == d.DoctorId && x.IsDelete == false && x.Like_Dislike == true).Count();
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
                        Education = getDegree(d.Degree, degree),
                        SpecializationIds = Array.ConvertAll(d.Specialization.Split(','), s => int.Parse(s)),//d.Specialization,
                        Specialization = getSpecialization(d.Specialization, disease),
                        aboutMe = d.AboutUs,
                        //Likes = feedback.Where(x => x.ILike == true).Count(),
                        Likes = likeCount,
                        Feedbacks = feedback.Count(),
                        BookingUrl = $"booking/{d.DoctorId}",
                        ProfileDetailUrl = $"doctorDetails/{d.DoctorId}",
                        ImgUrl = String.IsNullOrWhiteSpace(d.PhotoPath) ? $"{constant.imgUrl}/ProfilePic/Doctor/{d.DoctorId}.Jpeg" : $"{constant.imgUrl}/{d.PhotoPath}",
                        HospitalEmail = hospRepo.Email,
                        HospitalName = hospRepo.HospitalName,
                        HospitalId = d.HospitalId,
                        AboutUs = d.AboutUs,
                        Country = GetCountryName(Convert.ToInt16(hospRepo.Country)),
                        City = GetCityName(Convert.ToInt16(hospRepo.City)),
                        HospitalWebsite = hospRepo.Website,
                        Mobile = hospRepo.Mobile,
                        Services = getHospitalService(hospRepo.Services, hospitalService)

                    };
                    _doctors.Add(_doctor);
                }
            }
            catch (Exception ex)
            {
                return _doctors;
            }
            return _doctors;
        }

        private List<Hospital> getHospitalList(SearchFilter searchFilter)
        {
            var hospitalService = _hospitalServicesRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
            var hospitalAmenitie = _hospitalAmenitieRepository.GetAll().OrderBy(x => x.HospitalAmenities).ToList();
            Hospital _hospital = new Hospital();
            List<Hospital> _hospitals = new List<Hospital>();
            List<HospitalDetails> hospitalDtls = new List<HospitalDetails>();
            //country
            var objHosp = _hospitaldetailsRepo.Find(x => x.EmailConfirmed == true && x.IsDocumentApproved == 1).ToList();
            try
            {
                //city
                if (searchFilter.CountryId > 0)
                {
                    objHosp = objHosp.Where(x => x.Country == searchFilter.CountryId.ToString()).ToList();
                }

                //city
                if (searchFilter.CityID > 0)
                {
                    objHosp = objHosp.Where(x => x.City == searchFilter.CityID.ToString()).ToList();
                }


                //type 0
                if (searchFilter.Type == "0")
                {
                    objHosp = objHosp.Where(x => x.IsDocumentApproved == 1).ToList();
                }
                else if (searchFilter.Type != null && searchFilter.Type.Length > 0)
                {
                    //type not 0 and not2
                    //objHosp = objHosp.Where(x => x.IsDocumentApproved == 1 && x.Type == searchFilter.Type.ToString() && x.FacilityId == searchFilter.FacilityId).ToList();
                    objHosp = objHosp.Where(x => x.FacilityId == searchFilter.FacilityId).ToList();
                    if (searchFilter.Type == "2" || searchFilter.Type == "5" || searchFilter.Type == "23" || searchFilter.Type == "6"
                        || searchFilter.Type == "32" || searchFilter.Type == "9")
                    {
                        objHosp = objHosp.Where(x => x.Type != null && x.Type.Split(',').Select(ele => ele.Trim()).
                      Any(ele => searchFilter.Type.Contains(ele))).ToList();
                    }

                }


                //hospital id
                if (searchFilter.HospitalID != null && searchFilter.HospitalID.Length > 0)
                {
                    //objHosp = objHosp.Where(x => x.HospitalId == searchFilter.HospitalID).ToList();
                    objHosp = objHosp.Where(x => x.HospitalId.Split(',').Select(ele => ele.Trim()).
                   Any(ele => searchFilter.HospitalID.Contains(ele))).ToList();
                }


                //hospitalname
                if (searchFilter.HospitalName != null && searchFilter.HospitalName.Length > 0)
                {
                    objHosp = objHosp.Where(x => x.HospitalName.ToLower().Contains(searchFilter.HospitalName.ToLower())).ToList();
                }

                //by diesiesTypes
                if (searchFilter.DiseaseType != null && searchFilter.DiseaseType.Length > 0)
                {
                    //objHosp = objHosp.Where(x => x.Specialization.ToLower().Contains(searchFilter.DiseaseType.ToLower())).ToList();
                    objHosp = objHosp.Where(x => x.Specialization != null && x.Specialization.Split(',').Select(ele => ele.Trim()).
                   Any(ele => searchFilter.DiseaseType.Contains(ele))).ToList();
                }

                //by services
                if (searchFilter.Services != null && searchFilter.Services.Length > 0)
                {
                    //objHosp = objHosp.Where(x => x.Services.ToLower().Contains(searchFilter.Services.ToLower())).ToList();
                    objHosp = objHosp.Where(x => x.Services != null && x.Services.Split(',').Select(ele => ele.Trim()).
                   Any(ele => searchFilter.Services.Contains(ele))).ToList();
                }

                //by insurance
                if (searchFilter.InsuranceId != null && searchFilter.InsuranceId.Length > 0)
                {
                    //objHosp = objHosp.Where(x => x.Services.ToLower().Contains(searchFilter.Services.ToLower())).ToList();
                    objHosp = objHosp.Where(x => x.InsuranceId != null && x.InsuranceId.Split(',').Select(ele => ele.Trim()).
                   Any(ele => searchFilter.InsuranceId.Contains(ele))).ToList();
                }


                hospitalDtls = objHosp.ToList();
                int[] a = new int[0];
                List<TblHospitalServices> _hospitalServices = new List<TblHospitalServices>();
                List<TblHospitalAmenities> _hospitalAmenities = new List<TblHospitalAmenities>();
                FilterDoctor _filterDoctor = new FilterDoctor();
                foreach (var h in hospitalDtls ?? new List<HospitalDetails>())
                {
                    var feedback = _feedbackRepo.Find(x => x.PageId == h.HospitalId);
                    int likeCount = _likeVisitorRepo.Find(x => x.HFP_DOC_NCID.Trim() == h.HospitalId && x.IsDelete == false && x.Like_Dislike == true).Count();
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
                    _hospital.Doctors = getDoctors(h.HospitalId, searchFilter.DiseaseType, "null", ref _filterDoctor);
                    //_hospital.Likes = feedback.Where(x => x.ILike == true).Count();
                    _hospital.Likes = likeCount;
                    _hospital.Feedbacks = feedback.Count();
                    _hospital.BookingUrl = $"booking/{h.HospitalId}";
                    _hospital.ProfileDetailUrl = $"hospitalDetails/{h.HospitalId}";
                    _hospital.ImgUrl = h.ProfilePath == null ? $"{constant.imgUrl}/ProfilePic/Hospital/{h.HospitalId}.Jpeg" : $"{constant.imgUrl}/{h.ProfilePath}";
                    _hospitalServices.AddRange(_hospital.Services);
                    _hospitalAmenities.AddRange(_hospital.Amenities);
                    _hospitals.Add(_hospital);

                }
            }
            catch (Exception ex)
            {
                return _hospitals;
            }
            return _hospitals;
        }

    }
}