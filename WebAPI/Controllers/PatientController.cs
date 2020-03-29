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
        IMedicalInformationRepository _medicalInformationRepo = RepositoryFactory.Create<IMedicalInformationRepository>(ContextTypes.EntityFramework);
        IInsuranceInformationRepository _insuranceInformationRepo = RepositoryFactory.Create<IInsuranceInformationRepository>(ContextTypes.EntityFramework);
        IAppointmentRepository _appointmentRepo = RepositoryFactory.Create<IAppointmentRepository>(ContextTypes.EntityFramework);
        ITimeMasterRepository _timeMasterRepo = RepositoryFactory.Create<ITimeMasterRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);
        IQuickHealthRepository _quickHealthRepository =
            RepositoryFactory.Create<IQuickHealthRepository>(ContextTypes.EntityFramework);

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
           List<PatientPrescription> lstPrescription = _prescriptionRepo.GetAll().Where(x=>x.PatientId== patientId).ToList<PatientPrescription>().OrderByDescending(x=>x.Id).ToList();

            return Request.CreateResponse(HttpStatusCode.Accepted, lstPrescription);
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
                                           PrescriptionDate = p.DateEntered,
                                           PrescriptionId = p.Id,
                                           Specialization = getSpecialization(d.Specialization, disease),
                                       };
            return Request.CreateResponse(HttpStatusCode.Accepted, patientPrescriptions);
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
                                       into mif from medi in mif.DefaultIfEmpty()
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
                                           Temperature = medi?.Temprature ?? 0 ,
                                           Hight = medi?.Hight??0,
                                           Weight = medi?.Wight??0,
                                           Cholesterol = medi?.Cholesterol ?? 0,
                                           Others = medi?.OtherDetails ?? "N/A",
                                           Sugar = medi?.Sugar ?? 0 ,
                                           Heartbeat = medi?.Heartbeats ?? 0,
                                       };

            
            return Request.CreateResponse(HttpStatusCode.Accepted, patientPrescriptions);
        }
        public string GetAge(string dateOfbirth)
        {
            if (dateOfbirth==null)
            {
                return "N/A";
            }
            DateTime dateOfBirth=(Convert.ToDateTime(dateOfbirth));
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
    }
}

