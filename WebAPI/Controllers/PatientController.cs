using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
                if (patinet.Count==0)
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
            var result= _insuranceInformationRepo.Find(x => x.ClientId == clientId).FirstOrDefault();

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
    }
}

