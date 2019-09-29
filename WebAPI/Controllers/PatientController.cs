using NoorCare.Repository;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class PatientController : ApiController
    {
        IClientDetailRepository _clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);
        IPatientPrescriptionRepository _prescriptionRepo = RepositoryFactory.Create<IPatientPrescriptionRepository>(ContextTypes.EntityFramework);
        IMedicalInformationRepository _medicalInformationRepo = RepositoryFactory.Create<IMedicalInformationRepository>(ContextTypes.EntityFramework);
        IInsuranceInformationRepository _insuranceInformationRepo = RepositoryFactory.Create<IInsuranceInformationRepository>(ContextTypes.EntityFramework);

        [Route("api/patient/GetAllPatient")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllPatient()
        {
            var result = _clientDetailRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
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
            var result = _prescriptionRepo.Find(x => x.PatientId == patientId).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/patient/SavePatientPrescription")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage SavePatientPrescription(PatientPrescription obj)
        {
            var _prescriptionCreated = _prescriptionRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, obj.Id);
        }
    }
}

