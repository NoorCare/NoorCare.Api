using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Repository;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class AppointmentController : ApiController
    {
        Registration _registration = new Registration();
        IAppointmentRepository _appointmentRepo = RepositoryFactory.Create<IAppointmentRepository>(ContextTypes.EntityFramework);
        IAppointmentRepository _getAppointmentList = RepositoryFactory.Create<IAppointmentRepository>(ContextTypes.EntityFramework);
        ITimeMasterRepository _timeMasterRepo = RepositoryFactory.Create<ITimeMasterRepository>(ContextTypes.EntityFramework);

        [Route("api/appointment/getall")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Appointment
        public HttpResponseMessage GetAll()
        {
            var result =  _appointmentRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/appointment/getdetail/{appointmentid}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetDetail(string appointmentid)
        {
            var result = _appointmentRepo.Find(x => x.DoctorId == appointmentid).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);            
        }

        [Route("api/appointment/register")]
        [HttpPost]
        [AllowAnonymous]
        // POST: api/Appointment
        public HttpResponseMessage Register(Appointment obj)
        {
            ICountryCodeRepository _countryCodeRepository = RepositoryFactory.Create<ICountryCodeRepository>(ContextTypes.EntityFramework);
            CountryCode countryCode = _countryCodeRepository.Find(x => x.Id == obj.CountryCode).FirstOrDefault();
            string AppointmentId= _registration.creatId(5, obj.CountryCode, 0);
            obj.AppointmentId = AppointmentId;

            var _appointmentCreated = _appointmentRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, obj.AppointmentId);
        }
 

        [Route("api/appointment/update")]
        [HttpPut]
        [AllowAnonymous]
        // PUT: api/Appointment/5
        public HttpResponseMessage Update(Appointment obj)
        {
            obj.Id = _getAppointmentList.Find(x => x.AppointmentId == obj.AppointmentId).FirstOrDefault().Id;
            var result = _appointmentRepo.Update(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/appointment/delete/{appointmentid}")]
        [HttpDelete]
        [AllowAnonymous]
        // DELETE: api/Appointment/5
        public HttpResponseMessage Delete(string appointmentid)
        {
            int tbleId= _getAppointmentList.Find(x => x.AppointmentId == appointmentid).FirstOrDefault().Id;

            var result = _appointmentRepo.Delete(tbleId);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/GetAppointmentTimebyTimeId/{TimeId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAppointmentTimebyTimeId(string TimeId)
        {
            var TimeMaster = _timeMasterRepo.GetAll().ToList();
            var result = getTimeList(TimeId, TimeMaster);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        private List<TimeMaster> getTimeList(string TimeList, List<TimeMaster> timeMstr)
        {
            if (TimeList == null || TimeList == "")
            {
                return new List<TimeMaster>();
            }
            var lst = TimeList.Split(',');
            int[] myInts = Array.ConvertAll(lst, s => int.Parse(s));
            var TimeLst = timeMstr.Where(x => myInts.Contains(x.Id)).ToList();
            return TimeLst;
        }
    }
}
