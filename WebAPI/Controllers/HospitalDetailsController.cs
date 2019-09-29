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
using System.Web.Http.OData;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class HospitalDetailsController : ApiController
    {
        IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _getHospitaldetailsList = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
        Registration _registration = new Registration();
        IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
        ITblHospitalServicesRepository _hospitalServicesRepository = RepositoryFactory.Create<ITblHospitalServicesRepository>(ContextTypes.EntityFramework);
        ITblHospitalAmenitiesRepository _hospitalAmenitieRepository = RepositoryFactory.Create<ITblHospitalAmenitiesRepository>(ContextTypes.EntityFramework);
        IFeedbackRepository _feedbackRepo = RepositoryFactory.Create<IFeedbackRepository>(ContextTypes.EntityFramework);


        [Route("api/hospitaldetails/getall")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/HospitalDetails
        public HttpResponseMessage GetAll()
        {
            var result = _hospitaldetailsRepo.GetAll().ToList();
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
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            Hospital _hospital = new Hospital();
            List<Hospital> _hospitals = new List<Hospital>();

            foreach (var h in hospitals ?? new List<HospitalDetails>())
            {
                var feedback = _feedbackRepo.Find(x => x.PageId == h.HospitalId);
                

                _hospital = new Hospital
                {
                    HospitalId = h.HospitalId,
                    HospitalName = h.HospitalName,
                    Mobile = h.Mobile,
                    AlternateNumber = h.AlternateNumber,
                    Email=h.Email,
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
                   // AmenitiesIds = Array.ConvertAll(h.Amenities.Split(','), s => int.Parse(s)),
                    Amenities = getHospitalAmenities(h.Amenities, hospitalAmenitie),
                   // ServicesIds = Array.ConvertAll(h.Services.Split(','), s => int.Parse(s)),
                    Services = getHospitalService(h.Services, hospitalService),
                    Specialization = getSpecialization(h.Specialization, disease),
                    Doctors = getDoctors(h.HospitalId),
                    Likes = feedback.Where(x => x.ILike == true).Count(),
                    Feedbacks = feedback.Count(),
                    BookingUrl = $"booking/{h.HospitalId}",
                    ProfileDetailUrl = $"hospitalDetails/{h.HospitalId}",
                    ImgUrl = $"{constant.imgUrl}/Hospital/{h.HospitalId}.Jpeg"
                };
             
                _hospitals.Add(_hospital);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, _hospitals);
        }

        #region Uility
       
        private List<Doctors> getDoctors(string HospitalId)
        {
            List<Disease> _disease = new List<Disease>();
            List<decimal> _priceses = new List<decimal>();
            Doctors _doctor = new Doctors();
            List<Doctors> _doctors = new List<Doctors>();
            List<Doctor> doctors = _doctorRepo.Find(x => x.HospitalId == HospitalId);
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

                _doctors.Add(_doctor);
            }
            return _doctors;
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
                }
            }
            catch (Exception)
            {
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
            int tblId = _getHospitaldetailsList.Find(h => h.HospitalId == hospitalid).FirstOrDefault().Id; // getTableId(hospitalid);
            var result = _hospitaldetailsRepo.Delete(tblId);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        // Update Hospital Details
        [Route("api/hospitaldetails/{hospitalId}/updatehospital")]
        [HttpPatch]
        [AllowAnonymous]
        // PUT: api/UpdateHospitalProfile/5
        public IHttpActionResult UpdateHospitalProfile(string hospitalId, Delta<HospitalDetails> obj)
        {
            HospitalDetails _hospitalDetails = _hospitaldetailsRepo.Find(x => x.HospitalId == hospitalId).FirstOrDefault();
            if (_hospitalDetails != null)
            {
                obj.Patch(_hospitalDetails);
                var result = _hospitaldetailsRepo.Update(_hospitalDetails);
                return Ok(result);
            }

            return Ok();
        }
    }
}
