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
using System.Web.Http.OData;

namespace WebAPI.Controllers
{
    public class HospitalDetailsController : ApiController
    {
        IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _getHospitaldetailsList = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
        IDoctorAvailableTimeRepository _doctorAvailabilityRepo = RepositoryFactory.Create<IDoctorAvailableTimeRepository>(ContextTypes.EntityFramework);
        ITimeMasterRepository _timeMasterRepo = RepositoryFactory.Create<ITimeMasterRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
        ITblHospitalServicesRepository _hospitalServicesRepository = RepositoryFactory.Create<ITblHospitalServicesRepository>(ContextTypes.EntityFramework);
        ITblHospitalAmenitiesRepository _hospitalAmenitieRepository = RepositoryFactory.Create<ITblHospitalAmenitiesRepository>(ContextTypes.EntityFramework);
        IFeedbackRepository _feedbackRepo = RepositoryFactory.Create<IFeedbackRepository>(ContextTypes.EntityFramework);
        ISecretaryRepository _secretaryRepo = RepositoryFactory.Create<ISecretaryRepository>(ContextTypes.EntityFramework);
        IHospitalDocumentsRepository _hospitalDocumentsRepo = RepositoryFactory.Create<IHospitalDocumentsRepository>(ContextTypes.EntityFramework);
        ITblHospitalSpecialtiesRepository _hospitalSpecialtiesRepo = RepositoryFactory.Create<ITblHospitalSpecialtiesRepository>(ContextTypes.EntityFramework);
        IInsuranceInformationRepository _insuranceInformationRepository =
        RepositoryFactory.Create<IInsuranceInformationRepository>(ContextTypes.EntityFramework);
        IFacilityRepository _facilityRepo = RepositoryFactory.Create<IFacilityRepository>(ContextTypes.EntityFramework);
        IFacilityImagesRepository _facilityImagesRepo = RepositoryFactory.Create<IFacilityImagesRepository>(ContextTypes.EntityFramework);
        IHospitalInsuranceRepository _hospitalInsuranceRepo = RepositoryFactory.Create<IHospitalInsuranceRepository>(ContextTypes.EntityFramework);
        IInsuranceMasterRepository _insuranceMasterRepo = RepositoryFactory.Create<IInsuranceMasterRepository>(ContextTypes.EntityFramework);

        [Route("api/hospitaldetails/getall")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/HospitalDetails
        public HttpResponseMessage GetAll()
        {
            var result = _hospitaldetailsRepo.Find(x => x.EmailConfirmed == true && x.IsDocumentApproved == 1 && x.IsDeleted == false).ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        //[Route("api/getHospitalDetail/{hospitalid}")] this is renamed by below name
        [Route("api/hospitaldetails/getdetail/{hospitalid}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetHospitalDetail(string hospitalid)
        {
            List<HospitalDetails> hospitals = _hospitaldetailsRepo.Find(x => x.HospitalId == hospitalid);

            var hospitalService = _hospitalServicesRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
            var hospitalAmenitie = _hospitalAmenitieRepository.GetAll().OrderBy(x => x.HospitalAmenities).ToList();
            var disease = _hospitalSpecialtiesRepo.GetAll().OrderBy(x => x.HospitalSpecialties).ToList();
            Hospital _hospital = new Hospital();
            List<Hospital> _hospitals = new List<Hospital>();

            foreach (var h in hospitals ?? new List<HospitalDetails>())
            {
                var feedback = _feedbackRepo.Find(x => x.PageId == h.HospitalId);
                var facility = _facilityRepo.Find(x => x.JobType == h.jobType).FirstOrDefault();

                _hospital = new Hospital
                {
                    HospitalId = h.HospitalId,
                    HospitalName = h.HospitalName,
                    Mobile = h.Mobile,
                    AlternateNumber = h.AlternateNumber,
                    Email = h.Email,
                    Website = h.Website,
                    EstablishYear = h.EstablishYear,
                    NumberofBed = h.NumberofBed,
                    NumberofAmbulance = h.NumberofAmbulance,
                    PaymentType = h.PaymentType,
                    Emergency = h.Emergency,
                    FacilityId = h.FacilityId,
                    JobType = h.jobType,
                    JobTypePermission = facility.Permission,
                    Address = h.Address,
                    Street = h.Street,
                    Country = h.Country,
                    City = h.City,
                    PostCode = h.PostCode,
                    Landmark = h.Landmark,
                    AboutUs = h.AboutUs,
                    InsuranceCompanies = h.InsuranceCompanies,

                    // AmenitiesIds = Array.ConvertAll(h.Amenities.Split(','), s => int.Parse(s)),
                    Amenities = getHospitalAmenities(h.Amenities, hospitalAmenitie),
                    // ServicesIds = Array.ConvertAll(h.Services.Split(','), s => int.Parse(s)),
                    Services = getHospitalService(h.Services, hospitalService),
                    Specialization = getHospitalSpecialization(h.Specialization, disease),
                    Doctors = getDoctors(h.HospitalId),
                    Secretary = getSecretary(h.HospitalId),
                    Likes = feedback.Where(x => x.ILike == true).Count(),
                    Feedbacks = feedback.Count(),
                    BookingUrl = $"booking/{h.HospitalId}",
                    ProfileDetailUrl = $"hospitalDetails/{h.HospitalId}",
                    ImgUrl = $"{constant.imgUrl}/ProfilePic/Hospital/{h.HospitalId}.Jpeg",
                    IsDocumentApproved = h.IsDocumentApproved.ToString()
                };

                _hospitals.Add(_hospital);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, _hospitals);
        }

        [Route("api/hospital/details/{clientid}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetHospitalDetailByClientId(string clientid)
        {
            var clientType = clientid.Split('-')[0];
            string hospitalId = string.Empty;
            if (clientType == "NCD")
            {
                var user = _doctorRepo.Find(x => x.DoctorId == clientid).ToList();
                if (user.Count > 0)
                {
                    hospitalId = user[0].HospitalId;
                }
            }
            else if (clientType == "NCH")
            {
                hospitalId = clientid;
            }
            else if (clientType == "NCS")
            {
                var user = _secretaryRepo.Find(x => x.SecretaryId == clientid);
                if (user.Count > 0)
                {
                    hospitalId = user[0].HospitalId;
                }
            }
            List<HospitalDetails> hospitals = _hospitaldetailsRepo.Find(x => x.HospitalId == hospitalId);

            var hospitalService = _hospitalServicesRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
            var hospitalAmenitie = _hospitalAmenitieRepository.GetAll().OrderBy(x => x.HospitalAmenities).ToList();
            var disease = _hospitalSpecialtiesRepo.GetAll().OrderBy(x => x.HospitalSpecialties).ToList();
            Hospital _hospital = new Hospital();
            List<Hospital> _hospitals = new List<Hospital>();

            foreach (var h in hospitals ?? new List<HospitalDetails>())
            {
                var feedback = _feedbackRepo.Find(x => x.PageId == h.HospitalId);
                var facility = _facilityRepo.Find(x => x.JobType == h.jobType).FirstOrDefault();
                _hospital = new Hospital
                {
                    Id = h.Id,
                    HospitalId = h.HospitalId,
                    HospitalName = h.HospitalName,
                    Mobile = h.Mobile,
                    AlternateNumber = h.AlternateNumber,
                    Email = h.Email,
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
                    AboutUs = h.AboutUs,
                    InsuranceCompanies = h.InsuranceCompanies,
                    JobType = h.jobType,
                    JobTypePermission = facility.Permission,

                    // AmenitiesIds = Array.ConvertAll(h.Amenities.Split(','), s => int.Parse(s)),
                    Amenities = getHospitalAmenities(h.Amenities, hospitalAmenitie),
                    // ServicesIds = Array.ConvertAll(h.Services.Split(','), s => int.Parse(s)),
                    Services = getHospitalService(h.Services, hospitalService),
                    Specialization = getHospitalSpecialization(h.Specialization, disease),
                    Doctors = getDoctors(h.HospitalId),
                    Secretary = getSecretary(h.HospitalId),
                    Likes = feedback.Where(x => x.ILike == true).Count(),
                    Feedbacks = feedback.Count(),
                    BookingUrl = $"booking/{h.HospitalId}",
                    ProfileDetailUrl = $"hospitalDetails/{h.HospitalId}",
                    ImgUrl = $"{constant.imgUrl}/ProfilePic/Hospital/{h.HospitalId}.Jpeg"
                };

                _hospitals.Add(_hospital);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, _hospitals);
        }

        #region Uility      

        private List<Doctors> getDoctors(string HospitalId)
        {
            List<TimeMaster> _timeMaster = new List<TimeMaster>();
            var timeMaster = _timeMasterRepo.GetAll().OrderBy(x => x.Id).ToList();


            List<Disease> _disease = new List<Disease>();
            List<decimal> _priceses = new List<decimal>();
            Doctors _doctor = new Doctors();
            List<Doctors> _doctors = new List<Doctors>();
            List<Doctor> doctors = _doctorRepo.Find(x => x.HospitalId == HospitalId);
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();

            foreach (var d in doctors ?? new List<Doctor>())
            {
                var feedback = _feedbackRepo.Find(x => x.PageId == d.DoctorId);
                var doctorAvailability = _doctorAvailabilityRepo.Find(x => x.DoctorId == d.DoctorId).FirstOrDefault();
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
                    TimeAvailability = doctorAvailability != null ? getDoctorAvilability(doctorAvailability.TimeId, timeMaster) : null,
                    Likes = feedback.Where(x => x.ILike == true).Count(),
                    Feedbacks = feedback.Count(),
                    BookingUrl = $"booking/{d.DoctorId}",
                    ProfileDetailUrl = $"doctorDetails/{d.DoctorId}",
                    ImgUrl = $"{constant.imgUrl}/ProfilePic/Doctor/{d.DoctorId}.Jpeg"
                };

                _doctors.Add(_doctor);
            }
            return _doctors;
        }

        [Route("api/hospital/doctor/{HospitalId}/{Specialtyid}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getDoctorsBySpecialty(string HospitalId, string Specialtyid)
        {
            List<TimeMaster> _timeMaster = new List<TimeMaster>();
            var timeMaster = _timeMasterRepo.GetAll().OrderBy(x => x.Id).ToList();


            List<Disease> _disease = new List<Disease>();
            List<decimal> _priceses = new List<decimal>();
            Doctors _doctor = new Doctors();
            List<Doctors> _doctors = new List<Doctors>();
            List<Doctor> doctors = new List<Doctor>();
            if (Specialtyid != "0")
            {
                doctors = _doctorRepo.Find(x => x.HospitalId == HospitalId && x.Specialization.Contains(Specialtyid));
            }
            else
            {
                doctors = _doctorRepo.Find(x => x.HospitalId == HospitalId);
            }

            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();

            foreach (var d in doctors ?? new List<Doctor>())
            {
                var feedback = _feedbackRepo.Find(x => x.PageId == d.DoctorId);
                var doctorAvailability = _doctorAvailabilityRepo.Find(x => x.DoctorId == d.DoctorId).FirstOrDefault();
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
                    TimeAvailability = doctorAvailability != null ? getDoctorAvilability(doctorAvailability.TimeId, timeMaster) : null,
                    Likes = feedback.Where(x => x.ILike == true).Count(),
                    Feedbacks = feedback.Count(),
                    BookingUrl = $"booking/{d.DoctorId}",
                    ProfileDetailUrl = $"doctorDetails/{d.DoctorId}",
                    ImgUrl = $"{constant.imgUrl}/ProfilePic/Doctor/{d.DoctorId}.Jpeg"
                };

                _doctors.Add(_doctor);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, _doctors);
        }
        private List<Secretary> getSecretary(string HospitalId)
        {
            Secretary _secretarys = new Secretary();
            List<Secretary> secretary = new List<Secretary>();
            List<Secretary> secretaries = _secretaryRepo.Find(x => x.HospitalId == HospitalId);

            foreach (var s in secretaries ?? new List<Secretary>())
            {
                _secretarys = new Secretary
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    CountryCode = s.CountryCode,
                    PhoneNumber = s.PhoneNumber,
                    AlternatePhoneNumber = s.AlternatePhoneNumber,
                    Gender = s.Gender,
                    YearOfExperience = s.YearOfExperience,
                    SecretaryId = s.SecretaryId,
                    HospitalId = s.HospitalId,
                    jobType = s.jobType,
                    AboutUs = s.AboutUs,
                    IsDeleted = s.IsDeleted,
                    CreatedBy = s.CreatedBy,
                    ModifiedBy = s.ModifiedBy,
                    DateEntered = s.DateEntered,
                    DateModified = s.DateModified
                    //ImgUrl = $"{constant.imgUrl}/Secretary/{d.DoctorId}.Jpeg"
                };

                secretary.Add(_secretarys);
            }
            return secretary;
        }

        private List<TimeMaster> getDoctorAvilability(string TimeId, List<TimeMaster> TimeMaster)
        {
            if (!string.IsNullOrEmpty(TimeId))
            {
                var TimeIds = TimeId.TrimEnd(',').Split(',');
                int[] TimeInts = Array.ConvertAll(TimeIds, s => int.Parse(s));
                var timeList = TimeMaster.Where(x => TimeInts.Contains(x.Id)).ToList();
                return timeList;
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

        private List<TblHospitalSpecialties> getHospitalSpecialization(string diesiesType, List<TblHospitalSpecialties> diseases)
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
        #endregion

        [HttpPost]
        [Route("api/hospitaldetails/UploadProfilePic")]
        [AllowAnonymous]
        public IHttpActionResult UploadProfilePic()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;

            string hospitalId = httpRequest.Form["HospitalId"];
            try
            {
                var postedFile = httpRequest.Files["Image"];
                if (postedFile != null)
                {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).
                        Take(10).ToArray()).
                        Replace(" ", "-");
                    imageName = hospitalId + "." + ImageFormat.Jpeg;
                    var filePath = HttpContext.Current.Server.MapPath("~/ProfilePic/Hospital/" + imageName);
                    bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/ProfilePic/Hospital/" + imageName));
                    if (exists)
                    {
                        File.Delete(filePath);
                    }
                    postedFile.SaveAs(filePath);

                    // Save ProfilePath in table HospitalDetails
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok(hospitalId);
        }

        [HttpGet]
        [Route("api/hospitaldetails/profile/{hospitalId}")]
        [AllowAnonymous]
        public IHttpActionResult getHospitalDetailsProfile(string hospitalId)
        {
            return Ok(_hospitaldetailsRepo.Find(x => x.HospitalId == hospitalId));
        }


        [Route("api/hospitaldetails/delete/{hospitalid}")]
        [HttpDelete]
        [AllowAnonymous]
        // DELETE: api/HospitalDetails/5
        public HttpResponseMessage Delete(string hospitalid)
        {
            var hospital = _getHospitaldetailsList.Find(h => h.HospitalId == hospitalid).FirstOrDefault();
            int tblId = 0;
            if (hospital != null)
            {
                tblId = hospital.Id;
            }
            var result = _hospitaldetailsRepo.Delete(tblId);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/hospitaldetails/{hospitalId}/updatehospital")]
        [HttpPatch]
        [AllowAnonymous]
        public IHttpActionResult UpdateHospitalProfile(string hospitalId, Delta<HospitalDetails> obj)
        {
            HospitalDetails _hospitalDetails = _hospitaldetailsRepo.Find(x => x.HospitalId == hospitalId).FirstOrDefault();
            if (_hospitalDetails != null)
            {
                _hospitalDetails.ProfilePath = "ProfilePic/hospital/" + hospitalId + ".jpeg";
                ; obj.Patch(_hospitalDetails);
                var result = _hospitaldetailsRepo.Update(_hospitalDetails);
                return Ok(result);
            }

            return Ok();
        }

        [Route("api/hospitaldetails/updateaddress")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult UpdateHospitalAddress()
        {
            var httpRequest = HttpContext.Current.Request;
            string hospitalId = httpRequest.Form["ClientId"];
            string Address = httpRequest.Form["Address"];
            string Country = httpRequest.Form["Country"];
            string City = httpRequest.Form["City"];
            string PinCode = httpRequest.Form["PinCode"];
            HospitalDetails _hospitalDetails = _hospitaldetailsRepo.Find(x => x.HospitalId == hospitalId).FirstOrDefault();
            if (_hospitalDetails != null)
            {
                _hospitalDetails.Address = Address;
                _hospitalDetails.Country = Country;
                _hospitalDetails.City = City;
                _hospitalDetails.PostCode = PinCode;
                var result = _hospitaldetailsRepo.Update(_hospitalDetails);
                return Ok(result);
            }

            return Ok();
        }

        [Route("api/hospitaldetails/aboutus")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult UpdateAboutus()
        {
            var httpRequest = HttpContext.Current.Request;
            string hospitalId = httpRequest.Form["ClientId"];
            string aboutus = httpRequest.Form["Aboutus"];
            HospitalDetails _hospitalDetails = _hospitaldetailsRepo.Find(x => x.HospitalId == hospitalId).FirstOrDefault();
            if (_hospitalDetails != null)
            {
                _hospitalDetails.AboutUs = aboutus;
                var result = _hospitaldetailsRepo.Update(_hospitalDetails);
                return Ok(result);
            }

            return Ok();
        }

        [HttpGet]
        [Route("api/hospital/latlong/{hospitalId}")]
        [AllowAnonymous]
        public IHttpActionResult getHospitalLatLong(string hospitalId)
        {
            return Ok(_hospitalDocumentsRepo.Find(x => x.HospitalId == hospitalId).FirstOrDefault());
        }


        [HttpPost]
        [Route("api/hospital/uploadhospitaldocuments")]
        [AllowAnonymous]
        public IHttpActionResult uploadhospitaldocuments()
        {
            var httpRequest = HttpContext.Current.Request;
            string hospitalId = httpRequest.Form["HospitalId"];
            string address = httpRequest.Form["address"];
            decimal latitude = Convert.ToDecimal(httpRequest.Form["Latitude"]);
            decimal longitude = Convert.ToDecimal(httpRequest.Form["Longitude"]);
            int objId = 0;
            try
            {
                string basicPath = "~/HospitalDoc/" + hospitalId + "/";
                var directoryBackViewPath = Directory.CreateDirectory(HttpContext.Current.Server.MapPath(basicPath + "BackView"));
                var directoryFrontViewPath = Directory.CreateDirectory(HttpContext.Current.Server.MapPath(basicPath + "FrontView"));
                var directoryCRFrontViewPath = Directory.CreateDirectory(HttpContext.Current.Server.MapPath(basicPath + "CRFrontView"));
                var directoryLicenseFrontViewPath = Directory.CreateDirectory(HttpContext.Current.Server.MapPath(basicPath + "LicenseFrontView"));
                string IdBackView = constant.imgUrl + "HospitalDoc/" + hospitalId + "/BackView/" + httpRequest.Files["IdBackView"].FileName;
                string IdFrontView = constant.imgUrl + "HospitalDoc/" + hospitalId + "/IdFrontView/" + httpRequest.Files["IdFrontView"].FileName;
                string CrFrontView = constant.imgUrl + "HospitalDoc/" + hospitalId + "/CrFrontView/" + httpRequest.Files["CrFrontView"].FileName;
                string LicenseFrontView = constant.imgUrl + "HospitalDoc/" + hospitalId + "/LicenseFrontView/" + httpRequest.Files["LicenseFrontView"].FileName;
                HospitalDocumentVerification hospitalDocuments = new HospitalDocumentVerification
                {
                    HospitalId = hospitalId,
                    IdBackView = IdBackView,
                    IdFrontView = IdFrontView,
                    CrFrontView = CrFrontView,
                    LicenseFrontView = LicenseFrontView,
                    Address = address,
                    Latitude = latitude,
                    Longitude = longitude,
                };
                objId = _hospitalDocumentsRepo.Insert(hospitalDocuments);
                string filepath = HttpContext.Current.Server.MapPath(basicPath + "BackView/" + httpRequest.Files["IdBackView"].FileName);
                httpRequest.Files["IdBackView"].SaveAs(HttpContext.Current.Server.MapPath(basicPath + "BackView/" + httpRequest.Files["IdBackView"].FileName));
                httpRequest.Files["IdFrontView"].SaveAs(HttpContext.Current.Server.MapPath(basicPath + "FrontView/" + httpRequest.Files["IdBackView"].FileName));
                httpRequest.Files["CrFrontView"].SaveAs(HttpContext.Current.Server.MapPath(basicPath + "CrFrontView/" + httpRequest.Files["CrFrontView"].FileName));
                httpRequest.Files["LicenseFrontView"].SaveAs(HttpContext.Current.Server.MapPath(basicPath + "LicenseFrontView/" + httpRequest.Files["LicenseFrontView"].FileName));
            }
            catch (Exception ex)
            {
            }
            return Ok(objId);
        }

        [HttpGet]
        [Route("api/hospital/get/insuranceinfo/{HospitalId}")]
        public IHttpActionResult getInsurancenformationt(string HospitalId)
        {
            InsuranceInformation _insuranceContact = _insuranceInformationRepository.Find(x => x.ClientId == HospitalId && x.Type == "Hospital").FirstOrDefault();
            return Ok(_insuranceContact);
        }

        [HttpGet]
        [Route("api/hospital/get/insurancedetail/{insuranceId}")]
        public IHttpActionResult getinsurancedetail(int insuranceId)
        {
            InsuranceInformation _insuranceContact = _insuranceInformationRepository.Find(x => x.Id == insuranceId).FirstOrDefault();
            return Ok(_insuranceContact);
        }

        [HttpPost]
        [Route("api/hospital/add/insuranceinfo/{ClientId}")]
        [AllowAnonymous]
        public IHttpActionResult AddInsurancenformationt(string ClientId, InsuranceInformation insuranceInformation)
        {
            InsuranceInformation _insuranceInformation = new InsuranceInformation
            {
                ClientId = ClientId,
                CompanyName = insuranceInformation.CompanyName,
                InsuraceNo = insuranceInformation.InsuraceNo,
                ExpiryDate = insuranceInformation.ExpiryDate,
                IsActive = true,
                Type = "Hospital"


            };
            return Ok(_insuranceInformationRepository.Insert(_insuranceInformation));
        }

        [HttpPost]
        [Route("api/hospital/update/insuranceinfo")]
        public IHttpActionResult UpdateInsurancenformationt(InsuranceInformation insuranceInformation)
        {
            InsuranceInformation _insuranceInformation = _insuranceInformationRepository.Find(x => x.Id == insuranceInformation.Id).FirstOrDefault();
            _insuranceInformation.CompanyName = insuranceInformation.CompanyName;
            _insuranceInformation.InsuraceNo = insuranceInformation.InsuraceNo;
            _insuranceInformation.ExpiryDate = insuranceInformation.ExpiryDate;
            return Ok(_insuranceInformationRepository.Update(_insuranceInformation));
        }

        [HttpPost]
        [Route("api/hospital/delete/insuranceinfo")]
        public IHttpActionResult DeleteInsurancenformationt(InsuranceInformation insuranceInformation)
        {
            InsuranceInformation _insuranceInformation = _insuranceInformationRepository.Find(x => x.Id == insuranceInformation.Id).FirstOrDefault();
            _insuranceInformation.IsActive = insuranceInformation.IsActive;
            return Ok(_insuranceInformationRepository.Update(_insuranceInformation));
        }
        [HttpGet]
        [Route("api/hospital/facilityimages/{hospitalId}")]
        [AllowAnonymous]
        public IHttpActionResult FacilityImages(string hospitalId)
        {
            var facilityimages = _facilityImagesRepo.Find(x => x.FacilityNoorCareNumber == hospitalId);
            return Ok(facilityimages);
        }

        [HttpGet]
        [Route("api/hospital/facilityimages/delete/{id}")]
        [AllowAnonymous]
        public IHttpActionResult DeleteFacilityImages(int id)
        {
            _facilityImagesRepo.Delete(id);
            return Ok("");
        }

        [HttpPost]
        [Route("api/hospital/assign/insurance")]
        public IHttpActionResult AssignInsurance(string InsuranceIds, string HospitalId)
        {
            string message = string.Empty;
            try
            {
                var insuranceIdsList = InsuranceIds.Trim(',').Split(',');
                foreach (var insuranceid in insuranceIdsList)
                {
                    Int32 _insuranceid = Convert.ToInt32(insuranceid);
                    HospitalInsurance hpli = _hospitalInsuranceRepo.Find(x => x.HospitalId == HospitalId && x.InsuranceId == _insuranceid).FirstOrDefault();
                    if (hpli != null)
                    {
                        hpli.IsActive = false;
                        hpli.ModifiedBy = HospitalId;
                        hpli.ModifiedDate = DateTime.Now;
                        _hospitalInsuranceRepo.Update(hpli);
                    }

                    if (insuranceid != "")
                    {
                        HospitalInsurance hospitalInsurance = new HospitalInsurance();
                        hospitalInsurance = new HospitalInsurance();
                        hospitalInsurance.HospitalId = HospitalId;
                        hospitalInsurance.InsuranceId = Convert.ToInt32(insuranceid);
                        hospitalInsurance.CreatedBy = HospitalId;
                        _hospitalInsuranceRepo.Insert(hospitalInsurance);
                    }
                }
                message = "success";
            }
            catch (Exception ex)
            {

                message = ex.Message;
            }

            return Ok(message);
        }

        [HttpGet]
        [Route("api/hospital/get/HospitalInsurance/{HospitalId}")]
        [AllowAnonymous]
        public IHttpActionResult HospitalInsurance(string HospitalId)
        {
            List<HospitaInsuranceModel> hospitalInsurancesList = new List<HospitaInsuranceModel>();
            var _hospitalInsuranceList = _hospitalInsuranceRepo.GetAll().Where(x => x.HospitalId == HospitalId && x.IsActive==true).ToList();
            var _insuranceMaster = _insuranceMasterRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
            foreach (var item in _insuranceMaster.ToList())
            {
                HospitaInsuranceModel hospitaInsuranceModel = new HospitaInsuranceModel();
                foreach (var hi in _hospitalInsuranceList)
                {
                    if (hi.InsuranceId == item.Id)
                    {
                        hospitaInsuranceModel.InsuranceId = hi.InsuranceId.ToString();
                        hospitaInsuranceModel.HospitalId = hi.HospitalId;
                        hospitaInsuranceModel.IsHospital = Convert.ToBoolean(hi.IsActive);
                        hospitaInsuranceModel.Name = item.InsuranceCompanyName;
                        hospitalInsurancesList.Add(hospitaInsuranceModel);
                    }
                }
            }

            return Ok(hospitalInsurancesList);
            //List<HospitalInsurance> hospitalInsurancesList = new List<HospitalInsurance>();
            //var _hospitalInsuranceList = _hospitalInsuranceRepo.GetAll().Where(x => x.HospitalId == HospitalId).ToList();
            //var _insuranceMaster = _insuranceMasterRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
            //var query = from hi in _hospitalInsuranceList.ToList()
            //            join i in _insuranceMaster.ToList()
            //            on hi.InsuranceId equals i.Id
            //            where hi.HospitalId == HospitalId
            //            select new { Name = i.InsuranceCompanyName, Code = i.InsuranceCompanyCode, i.Id, hi.HospitalId };
            //return Ok(query);
        }

        [HttpGet]
        [Route("api/hospital/get/HospitalInsuranceMaster/{HospitalId}")]
        [AllowAnonymous]
        public IHttpActionResult HospitalInsuranceMaster(string HospitalId)
        {
            List<HospitaInsuranceModel> hospitalInsurancesList = new List<HospitaInsuranceModel>();
            var _hospitalInsuranceList = _hospitalInsuranceRepo.GetAll().Where(x => x.HospitalId == HospitalId).ToList();
            var _insuranceMaster = _insuranceMasterRepo.GetAll().Where(x => x.IsDeleted == false).ToList();
            foreach (var item in _insuranceMaster.ToList())
            {
                HospitaInsuranceModel hospitaInsuranceModel = new HospitaInsuranceModel();
                foreach (var hi in _hospitalInsuranceList)
                {
                    if (hi.InsuranceId== item.Id)
                    {
                        hospitaInsuranceModel.HospitalId = hi.HospitalId;
                        hospitaInsuranceModel.IsHospital =Convert.ToBoolean (hi.IsActive);
                        
                    }
                }
                hospitaInsuranceModel.Name = item.InsuranceCompanyName;
                hospitaInsuranceModel.Code = item.InsuranceCompanyCode;
                hospitaInsuranceModel.InsuranceId = item.Id.ToString();
                hospitalInsurancesList.Add(hospitaInsuranceModel);
            }

            return Ok(hospitalInsurancesList);
        }

        

            [HttpPost]
        [Route("api/hospital/saveinsurance")]
        [AllowAnonymous]
        public IHttpActionResult SaveInsurance(HospitaInsuranceModel hospitaInsuranceModel)
        {
            string message = string.Empty;
            try
            {
                HospitalInsurance hpli = _hospitalInsuranceRepo.GetAll().Where(x => x.HospitalId == hospitaInsuranceModel.HospitalId && x.InsuranceId == Convert.ToInt16(hospitaInsuranceModel.InsuranceIdes)).FirstOrDefault();
                if (hpli == null)
                {
                    HospitalInsurance hospitalInsurance = new HospitalInsurance();
                    hospitalInsurance = new HospitalInsurance();
                    hospitalInsurance.HospitalId = hospitaInsuranceModel.HospitalId;
                    hospitalInsurance.InsuranceId = Convert.ToInt32(hospitaInsuranceModel.InsuranceIdes);
                    hospitalInsurance.CreatedBy = hospitaInsuranceModel.HospitalId;
                    hospitalInsurance.CreatedDate = System.DateTime.Now;
                    hospitalInsurance.IsActive = true;
                    _hospitalInsuranceRepo.Insert(hospitalInsurance);
                }
                else
                {
                    if (hpli.IsActive==true)
                    {
                        hpli.IsActive = false;
                    }
                    else
                    {
                        hpli.IsActive = true;
                    }
                    hpli.ModifiedBy = hospitaInsuranceModel.HospitalId;
                    hpli.ModifiedDate = DateTime.Now;
                    _hospitalInsuranceRepo.Update(hpli);
                }
                message = "success";

            }
            catch (Exception ex)
            {

                message = ex.Message;
            }

            return Ok(message);
        }
    }
}