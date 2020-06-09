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
    public class PatientController : ApiController
    {
        IClientDetailRepository _clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);
        IPatientPrescriptionRepository _prescriptionRepo = RepositoryFactory.Create<IPatientPrescriptionRepository>(ContextTypes.EntityFramework);

        IPatientPrescriptionAssignRepository _prescriptionAssignRepo = RepositoryFactory.Create<IPatientPrescriptionAssignRepository>(ContextTypes.EntityFramework);

        IMedicalInformationRepository _medicalInformationRepo = RepositoryFactory.Create<IMedicalInformationRepository>(ContextTypes.EntityFramework);
        IInsuranceInformationRepository _insuranceInformationRepo = RepositoryFactory.Create<IInsuranceInformationRepository>(ContextTypes.EntityFramework);
        IAppointmentRepository _appointmentRepo = RepositoryFactory.Create<IAppointmentRepository>(ContextTypes.EntityFramework);
        ITimeMasterRepository _timeMasterRepo = RepositoryFactory.Create<ITimeMasterRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
        IQuickHealthRepository _quickHealthRepository =
            RepositoryFactory.Create<IQuickHealthRepository>(ContextTypes.EntityFramework);
        //IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _getDoctorList = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        //IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        IDoctorAvailableTimeRepository _doctorAvailabilityRepo = RepositoryFactory.Create<IDoctorAvailableTimeRepository>(ContextTypes.EntityFramework);
        //IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
        ITblHospitalServicesRepository _hospitalServicesRepository = RepositoryFactory.Create<ITblHospitalServicesRepository>(ContextTypes.EntityFramework);
        ITblHospitalAmenitiesRepository _hospitalAmenitieRepository = RepositoryFactory.Create<ITblHospitalAmenitiesRepository>(ContextTypes.EntityFramework);
        IFeedbackRepository _feedbackRepo = RepositoryFactory.Create<IFeedbackRepository>(ContextTypes.EntityFramework);
        //IAppointmentRepository _appointmentRepo = RepositoryFactory.Create<IAppointmentRepository>(ContextTypes.EntityFramework);

        [Route("api/patient/GetAllPatient")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllPatient()
        {
            var result = _clientDetailRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/patient/GetAllPatient/{DoctorId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllPatientByDoctor(string DoctorId)
        {
            var result = _appointmentRepo.GetAll().ToList();
            var resultTime = _timeMasterRepo.GetAll().ToList();
            var appointDetail = from a in result
                                join t in resultTime on a.TimingId equals t.Id.ToString()
                                join c in _clientDetailRepo.GetAll() on a.ClientId equals c.ClientId
                                where a.DoctorId == DoctorId
                                select new
                                {
                                    Id = a.Id,
                                    Time = t.TimeFrom + "-" + t.TimeTo + " " + t.AM_PM,
                                    Date = Convert.ToDateTime(a.AppointmentDate).ToShortDateString(),
                                    ClientId = a.ClientId,
                                    DateEntered = a.DateEntered,
                                    DoctorId = a.DoctorId,
                                    Name = c.FirstName + " " + c.LastName,
                                };
            List<Patient> _patientList = new List<Patient>();
            foreach (var item in appointDetail.Distinct().ToList())
            {
                var patinet = _patientList.Where(x => x.ClientId != item.ClientId).ToList();
                if (patinet.Count == 0)
                {
                    Patient patient = new Patient();
                    patient.Name = item.Name;
                    patient.ClientId = item.ClientId;
                    patient.TotalVisit = appointDetail.Where(x => x.ClientId == item.ClientId).ToList().Count.ToString();
                    patient.LastVisitDate = appointDetail.Where(x => x.ClientId == item.ClientId).OrderByDescending(x => x.Id).FirstOrDefault().Date;
                    patient.LastVisitTime = appointDetail.Where(x => x.ClientId == item.ClientId).OrderByDescending(x => x.Id).FirstOrDefault().Time;
                    _patientList.Add(patient);
                }

            }
            return Request.CreateResponse(HttpStatusCode.Accepted, _patientList.Distinct());
        }

        [Route("api/patient/GetPatient/{patientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetPatient(string patientId)
        {
            var result = _clientDetailRepo.Find(x => x.ClientId == patientId).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/patient/GetMedicalInformation/{patientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetMedicalInformation(string patientId)
        {
            var result = _medicalInformationRepo.Find(m => m.clientId == patientId).ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/user/getMobile/{clientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getMobile(string clientId)
        {
            var result = _clientDetailRepo.Find(m => m.ClientId == clientId).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.Accepted, result.MobileNo.ToString());
        }


        [HttpGet]
        [Route("api/patient/insuranceinfo/{clientId}")]
        public IHttpActionResult getInsuranceInformation(string clientId)
        {
            var result = _insuranceInformationRepo.Find(x => x.ClientId == clientId).FirstOrDefault();

            return Ok(result);
        }

        [Route("api/patient/getPrescription/{patientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getPrescription(string patientId)
        {
            List<PatientPrescription> lstPrescription = _prescriptionRepo.GetAll().Where(x => x.PatientId == patientId).ToList<PatientPrescription>().OrderByDescending(x => x.Id).ToList();
            List<PatientPrescription> lstPrescriptionList = new List<PatientPrescription>();
            foreach (var item in lstPrescription)
            {
                //item.Doctors = getDoctorDetailByDoctorId(item.DoctorId);
                //item.DateEntered = Convert.ToDateTime(item.DateEntered).ToString();
                lstPrescriptionList.Add(item);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, lstPrescriptionList.OrderBy(x => x.Id));
        }

        [Route("api/patient/getPrescription/{patientId}/{doctorId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getAssignPrescription(string patientId, string doctorId)
        {
            //var doctors = _doctorRepo.GetAll().ToList();
            //var hospitals = _hospitaldetailsRepo.GetAll().ToList();

            List<PatientPrescriptionList> _patientPres = new List<PatientPrescriptionList>();

            var prescription = _prescriptionRepo.GetAll().ToList();
            var presAssign = _prescriptionAssignRepo.GetAll().ToList();
            var result = (from PatientPrescription in prescription
                          where PatientPrescription.DoctorId == doctorId  && PatientPrescription.PatientId==patientId||
                          (from PatientPrescriptionAssign in presAssign
                           where
                             PatientPrescriptionAssign.AssignId == doctorId && PatientPrescriptionAssign.AssignBy==patientId &&
                             Convert.ToString(PatientPrescriptionAssign.IsActive) == "True"
                           select new
                           {
                               PatientPrescriptionAssign.PatientPresId
                           }).Contains(new { PatientPresId = PatientPrescription.Id })
                          select PatientPrescription).ToList();

            foreach (var res in result )
            {
                Doctor doctor = _doctorRepo.Find(x => x.DoctorId == res.DoctorId).FirstOrDefault();
                HospitalDetails hospitals = _hospitaldetailsRepo.Find(x => x.HospitalId == doctor.HospitalId).FirstOrDefault();
                PatientPrescriptionList _pres = new PatientPrescriptionList();
                _pres.DoctorFirstName = doctor.FirstName;
                _pres.DoctorLastName = doctor.LastName;
                _pres.DoctorEmail = doctor.Email;
                _pres.DoctorPhoneNumber = doctor.PhoneNumber;
                _pres.DoctorImgUrl = $"{constant.imgUrl}/ProfilePic/Doctor/{doctor.DoctorId}.Jpeg";
                _pres.HospitalName = hospitals.HospitalName;
                _pres.HospitalEmail = hospitals.Email;
                _pres.HospitalAddress = hospitals.Address;
                _pres.HospitalPicUrl = $"{constant.imgUrl}/" + hospitals.ProfilePath;

                _pres.DoctorId = res.DoctorId;
                _pres.Id = res.Id;
                _pres.PatientId = res.PatientId;
                _pres.Prescription = res.Prescription;
                _pres.Report = res.Report;
                _pres.CreatedBy = res.CreatedBy;
                _pres.DateEntered = res.DateEntered;
                _patientPres.Add(_pres);
            }

                return Request.CreateResponse(HttpStatusCode.Accepted, _patientPres.OrderBy(x => x.Id));
        }

        [Route("api/patient/SavePatientPrescription")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage SavePatientPrescription(PatientPrescription obj)
        {
            var _prescriptionCreated = _prescriptionRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, obj.Id);
        }



        [Route("api/patient/IsValidNoorCare/{patientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage IsValidNoorCare(string patientId)
        {
            var result = _clientDetailRepo.Find(x => x.ClientId == patientId);
            if (result.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, true);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, false);
            }
        }

        [Route("api/patient/getallprescription/{patientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getAllPrescription(string patientId)
        {
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            var patientPrescriptions = from p in _prescriptionRepo.GetAll()
                                       join d in _doctorRepo.GetAll() on p.DoctorId equals d.DoctorId
                                       join h in _hospitaldetailsRepo.GetAll() on d.HospitalId equals h.HospitalId
                                       where (p.PatientId == patientId)
                                       select new
                                       {
                                           HospitalName = h.HospitalName,
                                           HospitalNCNumber = h.HospitalId,
                                           DoctorName = d.FirstName + " " + d.LastName,
                                           DoctorNCNumber = d.DoctorId,
                                           Prescription = p.Prescription,
                                           Report = p.Report,
                                           PrescriptionDate = p.DateEntered,
                                           PrescriptionId = p.Id,
                                           Specialization = getSpecialization(d.Specialization, disease),
                                           //Doctors = getDoctorDetailByDoctorId(d.DoctorId)
                                       };

            return Request.CreateResponse(HttpStatusCode.Accepted, patientPrescriptions);
        }

        public Doctors getDoctorDetailByDoctorId(string doctorid)
        {
            Doctors _doctor = new Doctors();
            List<Feedback> feedbacks = new List<Feedback>();
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            Doctor d = _doctorRepo.Find(x => x.DoctorId == doctorid).FirstOrDefault();
            var feedback = _feedbackRepo.Find(x => x.PageId == doctorid);
            var hospitalService = _hospitalServicesRepository.GetAll().OrderBy(x => x.HospitalServices).ToList();
            var hospitalAmenitie = _hospitalAmenitieRepository.GetAll().OrderBy(x => x.HospitalAmenities).ToList();
            HospitalDetails hospitals = _hospitaldetailsRepo.Find(x => x.HospitalId == d.HospitalId).FirstOrDefault();
            if (d != null)
            {
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


            }
            return _doctor;
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
        [Route("api/patient/getprescriptiondetail/{prescriptionId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage getPrescriptionDetail(Int32 prescriptionId)
        {
            // on p.PatientId equals mi.clientId
            //var medicalinformation = _medicalInformationRepo.GetAll();
            //join pet in pets on person equals pet.Owner into gj
            //   from subpet in gj.DefaultIfEmpty()
            //    select new { person.FirstName, PetName = subpet?.Name ?? String.Empty };

            var patientPrescriptions = from p in _prescriptionRepo.GetAll()
                                       join d in _doctorRepo.GetAll() on p.DoctorId equals d.DoctorId
                                       join h in _hospitaldetailsRepo.GetAll() on d.HospitalId equals h.HospitalId
                                       join c in _clientDetailRepo.GetAll() on p.PatientId equals c.ClientId
                                       join mi in _medicalInformationRepo.GetAll() on p.PatientId equals mi.clientId
                                       into mif
                                       from medi in mif.DefaultIfEmpty()
                                       where (p.Id == prescriptionId)
                                       select new
                                       {
                                           HospitalName = h.HospitalName,
                                           HospitalNCNumber = h.HospitalId,
                                           DoctorName = d.FirstName + " " + d.LastName,
                                           DoctorNCNumber = d.DoctorId,
                                           Prescription = p.Prescription,
                                           PrescriptionDate = p.DateEntered,
                                           PrescriptionId = p.Id,
                                           PatientId = p.PatientId,
                                           Date = p.DateEntered,
                                           Name = c.FirstName + " " + c.LastName,
                                           Age = this.GetAge(c.DOB),
                                           Gender = c.Gender == 1 ? "Male" : "FeMale",
                                           Pressure = medi?.Pressure ?? 0,
                                           Temperature = medi?.Temprature ?? 0,
                                           Hight = medi?.Hight ?? 0,
                                           Weight = medi?.Wight ?? 0,
                                           Report = p.Report,
                                           Cholesterol = medi?.Cholesterol ?? 0,
                                           Others = medi?.OtherDetails ?? "N/A",
                                           Sugar = medi?.Sugar ?? 0,
                                           Heartbeat = medi?.Heartbeats ?? 0,
                                       };


            return Request.CreateResponse(HttpStatusCode.Accepted, patientPrescriptions);
        }
        public string GetAge(string dateOfbirth)
        {
            if (dateOfbirth == null)
            {
                return "N/A";
            }
            DateTime dateOfBirth = (Convert.ToDateTime(dateOfbirth));
            var today = DateTime.Today;
            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;

            return ((a - b) / 10000).ToString();
        }

        //private MedicalInformation getMedicalInformation(string clientId)
        //{
        //    var medicalInformation = _medicalInformationRepo.Find(x => x.clientId == clientId);
        //    if (medicalInformation.Count>0)
        //    {
        //        return medicalInformation.FirstOrDefault();
        //    }

        //}
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


        #region Patient Prescription Assign

        [HttpPost]
        [Route("api/patient/presassign")]
        [AllowAnonymous]
        public IHttpActionResult AssignPrescription(PatientPrescriptionAssign data)
        {
            try
            {
                int id = 0;
                if (data != null)
                {
                    string assignId = data.AssignId;
                    int patientPresId = data.PatientPresId;
                    int count = _prescriptionAssignRepo.Find(x => x.AssignId == assignId && x.PatientPresId == patientPresId).Count;
                    if (count <= 0)
                    {
                        id = _prescriptionAssignRepo.Insert(data);
                    }

                    //foreach (PatientPrescriptionAssign element in data)
                    //{
                    //    string assignId = element.AssignId;
                    //    int quickUploadId = element.QuickUploadId;
                    //    int count = _QuickUploadAssignRepo.Find(x => x.AssignId == assignId && x.QuickUploadId == quickUploadId).Count;
                    //    if (count <= 0)
                    //    {
                    //        id = _QuickUploadAssignRepo.Insert(element);
                    //    }
                    //}
                }
                return Ok(id);

            }
            catch (Exception ex)
            {
                return Ok("Dharmendra");
                //return InternalServerError(ex);
            }
            //return Ok(mailBoxid);
        }

        #endregion

    }
}

