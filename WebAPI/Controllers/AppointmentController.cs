using AngularJSAuthentication.API.Services;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Repository;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class AppointmentController : ApiController
    {
        EmailSender _emailSender = new EmailSender();
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
            var result = _appointmentRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/appointment/getUpcommingAppointment/{ClientId}")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Appointment
        public HttpResponseMessage getUpcommingAppointment(string ClientId)
        {
            var result = _appointmentRepo.GetAll().ToList();
            var resultTime = _timeMasterRepo.GetAll().ToList();
            var appointDetail = from a in result
                                join t in resultTime on a.TimingId equals t.Id.ToString()
                                where a.ClientId == ClientId
                                select new
                                {
                                    Time = t.TimeFrom + "-" + t.TimeTo + " " + t.AM_PM,
                                    Date = Convert.ToDateTime(Convert.ToDateTime(a.AppointmentDate + " " + t.TimeTo + t.AM_PM).ToString("dd/MM/yyyy hh:mm")),
                                    ClientId = a.ClientId,
                                    DateEntered = a.DateEntered,
                                    DoctorId = a.DoctorId
                                };
            //var abc = Convert.ToDateTime(Convert.ToDateTime("2020-01-21" + " " + "1:30" + "PM").ToString("dd/MM/yyyy hh:mm"));
            var todaydate = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
            var appintmentresult = appointDetail.Where(x => x.Date >= todaydate);
            return Request.CreateResponse(HttpStatusCode.Accepted, appintmentresult);
        }

        [Route("api/appointment/getdetail/{appointmentid}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetDetail(string appointmentid)
        {
            var result = _appointmentRepo.Find(x => x.AppointmentId == appointmentid).FirstOrDefault();
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
            string AppointmentId = _registration.creatId(5, obj.CountryCode, 0);
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
            var appointmentList = _getAppointmentList.Find(x => x.AppointmentId == obj.AppointmentId).FirstOrDefault();
            if (appointmentList != null)
            {
                obj.Id = appointmentList.Id;
            }
            var result = _appointmentRepo.Update(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/appointment/updateStatus/{AppointmentId}/{status}")]
        [HttpPost]
        [AllowAnonymous]
        // PUT: api/Appointment/5
        public HttpResponseMessage UpdateStatus(string AppointmentId, string status)
        {
            Appointment obj = _getAppointmentList.Find(x => x.AppointmentId == AppointmentId).FirstOrDefault();
            if (obj != null)
            {
                obj.Status = status;
            }
            var result = _appointmentRepo.Update(obj);

            //Email sent on status change
            string html = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Services/templat.html"));
             //_emailSender.email_send(model.Email, model.FirstName + " " + model.LastName == null ? "" : model.LastName, model.Id, model.JobType, model.PasswordHash);

            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/appointmentconfirmorreject/updatestatus")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage AppointmentConfirmorReject(Appointment appointment)
        {
            Appointment obj = _getAppointmentList.Find(x => x.AppointmentId == appointment.AppointmentId).FirstOrDefault();
            if (obj != null)
            {
                obj.Status = appointment.Status;
                obj.Comment = appointment.Comment;
                obj.DoctorId = appointment.DoctorId;
            }
            var result = _appointmentRepo.Update(obj);
            //Email sent on status change
            string html = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Services/templat.html"));
            //_emailSender.email_send(model.Email, model.FirstName + " " + model.LastName == null ? "" : model.LastName, model.Id, model.JobType, model.PasswordHash);

            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }
        [Route("api/appointment/delete/{appointmentid}")]
        [HttpDelete]
        [AllowAnonymous]
        // DELETE: api/Appointment/5
        public HttpResponseMessage Delete(string appointmentid)
        {
            int tbleId = 0;
            var appointmentList = _getAppointmentList.Find(x => x.AppointmentId == appointmentid).FirstOrDefault();
            if (appointmentList != null)
            {
                tbleId = appointmentList.Id;
            }
            var result = _appointmentRepo.Delete(tbleId);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/GetAppointmentTime")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAppointmentTime()
        {
            var TimeMasterList = _timeMasterRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, TimeMasterList);
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
