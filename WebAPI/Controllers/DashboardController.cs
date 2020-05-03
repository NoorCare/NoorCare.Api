using AngularJSAuthentication.API.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
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
    public class DashboardController : ApiController
    {
        ISecretaryRepository _secretaryRepo = RepositoryFactory.Create<ISecretaryRepository>(ContextTypes.EntityFramework);
        IFeedbackRepository _feedbackRepo = RepositoryFactory.Create<IFeedbackRepository>(ContextTypes.EntityFramework);
        IAppointmentRepository _appointmentRepo = RepositoryFactory.Create<IAppointmentRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        ITimeMasterRepository _timeMasterRepo = RepositoryFactory.Create<ITimeMasterRepository>(ContextTypes.EntityFramework);
        IClientDetailRepository _clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        IPatientPrescriptionRepository _patientPrescriptionRepo = RepositoryFactory.Create<IPatientPrescriptionRepository>(ContextTypes.EntityFramework);
        IQuickUploadRepository _quickUploadRepo = RepositoryFactory.Create<IQuickUploadRepository>(ContextTypes.EntityFramework);
        IMedicalInformationRepository _medicalInformationRepo = RepositoryFactory.Create<IMedicalInformationRepository>(ContextTypes.EntityFramework);

        [HttpGet]
        [Route("api/GetDashboardDetails/{Type}/{pageId}/{searchDate?}")] //Type= Doctor/secretary, page Id is secratoryId
        [AllowAnonymous]
        public HttpResponseMessage GetDashboardDetails(string Type, string pageId, string searchDate = null)
        {
            var HospitalId = "";
            if (Type.ToLower() == "secretary")
            {
                if (_secretaryRepo.Find(s => s.SecretaryId == pageId).FirstOrDefault() != null)
                {
                    HospitalId = _secretaryRepo.Find(s => s.SecretaryId == pageId).FirstOrDefault().HospitalId;
                }

            }
            else if (Type.ToLower() == "doctor")
            {
                if (_doctorRepo.Find(d => d.DoctorId == pageId).FirstOrDefault() != null)
                {
                    HospitalId = _doctorRepo.Find(d => d.DoctorId == pageId).FirstOrDefault().HospitalId;
                }
            }
            else if (Type.ToLower() == "hospital")
            {
                HospitalId = pageId;
            }

            DashboardTypeModel DashboardTypeModel = new DashboardTypeModel();
            DashboardModel dashboardModel = new DashboardModel();
            List<DashboardAppointmentListModel> lstDashboardAppointmentListModel = new List<DashboardAppointmentListModel>();

            if (Type.ToLower() != "patient")
            {
                if (!string.IsNullOrEmpty(HospitalId))
                {
                    DashboardTypeModel.HospitalId = HospitalId;

                    DashboardTypeModel.TotalFeedback = _feedbackRepo.Find(x => x.PageId == HospitalId).ToList().Count();

                    DashboardTypeModel.TotalDoctor = _doctorRepo.Find(d => d.HospitalId == HospitalId).ToList().Count();

                    DashboardTypeModel.BookedAppointment = _appointmentRepo.Find(a => a.HospitalId == HospitalId && a.Status == "Booked").ToList().Count();

                    DashboardTypeModel.CancelAppointment = _appointmentRepo.Find(a => a.HospitalId == HospitalId && a.Status == "Rejected").ToList().Count();

                    DashboardTypeModel.NewAppointment = _appointmentRepo.Find(a => a.HospitalId == HospitalId && a.Status == "0").ToList().Count();
                    var appointment = _appointmentRepo.Find(a => a.HospitalId == HospitalId);
                    int count = 0;
                    foreach (var item in appointment)
                    {
                        var apdate = item.AppointmentDate;
                        var tdate = System.DateTime.Today.ToString("yyyy-MM-dd");
                        if (apdate== tdate)
                        {
                            count++;
                        }
                    }
                    DashboardTypeModel.TodayAppointment = count;
                }
                var appointmentList = _appointmentRepo.Find(a => a.HospitalId == HospitalId).ToList();
                foreach (var item in appointmentList)
                {
                    DashboardAppointmentListModel DashboardAppointmentListModel = new DashboardAppointmentListModel();

                    DashboardAppointmentListModel.AppointmentDate = item.AppointmentDate;
                    DashboardAppointmentListModel.AppointmentId = item.AppointmentId;
                    DashboardAppointmentListModel.Status = item.Status;
                    DashboardAppointmentListModel.TimeId = item.TimingId;
                    int appointmentTimeId = Convert.ToInt32(item.TimingId);
                    if (_timeMasterRepo != null)
                    {
                        var timeDetails = _timeMasterRepo.Find(t => t.Id == appointmentTimeId && t.IsActive == true).FirstOrDefault();
                        if (timeDetails != null)
                        {
                            DashboardAppointmentListModel.AppointmentTime = timeDetails.TimeFrom.Trim() + "-" + timeDetails.TimeTo.Trim() + " " + timeDetails.AM_PM.Trim();
                        }
                    }
                    var clientDetail = _clientDetailRepo.Find(x => x.ClientId == item.ClientId).FirstOrDefault();
                    if (clientDetail != null)
                    {
                        DashboardAppointmentListModel.DOB = clientDetail.DOB;
                        DashboardAppointmentListModel.PatientName = clientDetail.FirstName;

                        DashboardAppointmentListModel.NoorCareNumber = clientDetail.ClientId;
                        DashboardAppointmentListModel.Gender = clientDetail.Gender.ToString();
                    }

                    var doctorDetails = _doctorRepo.Find(d => d.DoctorId == item.DoctorId).FirstOrDefault();
                    if (doctorDetails != null)
                    {
                        DashboardAppointmentListModel.DoctorName = doctorDetails.FirstName +" "+doctorDetails.LastName;
                    }
                    lstDashboardAppointmentListModel.Add(DashboardAppointmentListModel);
                }
            }
            else if (Type.ToLower() == "patient")
            {
                DashboardTypeModel.BookedAppointment = _appointmentRepo.Find(a => a.ClientId == pageId).ToList().Count();
                DashboardTypeModel.TotalFeedback = _feedbackRepo.Find(x => x.ClientID == pageId).ToList().Count();
                DashboardTypeModel.TotalDoctorPrescription = _patientPrescriptionRepo.Find(p => p.PatientId == pageId).ToList().Count();
                DashboardTypeModel.TotalMedicalFile = _quickUploadRepo.Find(q => q.ClientId == pageId).ToList().Count();
            }

            dashboardModel.DashboardTypeModel = DashboardTypeModel;
            dashboardModel.DashboardAppointmentListModel = lstDashboardAppointmentListModel;
            return Request.CreateResponse(HttpStatusCode.Accepted, dashboardModel);
        }

        [HttpGet]
        [Route("api/GetDoctorDashboardDetails/{pageId}")] //Type= Doctor/secretary, page Id is secratoryId
        [AllowAnonymous]
        public HttpResponseMessage GetDoctorDashboardDetails(string pageId)
        {
            var HospitalId = "";
            if (_doctorRepo.Find(d => d.DoctorId == pageId).FirstOrDefault() != null)
            {
                HospitalId = _doctorRepo.Find(d => d.DoctorId == pageId).FirstOrDefault().HospitalId;
            }
            DashboardTypeModel DashboardTypeModel = new DashboardTypeModel();
            DashboardModel dashboardModel = new DashboardModel();
            List<DashboardAppointmentListModel> lstDashboardAppointmentListModel = new List<DashboardAppointmentListModel>();
            DashboardTypeModel.HospitalId = HospitalId;
            DashboardTypeModel.TotalFeedback = _feedbackRepo.Find(x => x.PageId == pageId).ToList().Count();
            DashboardTypeModel.TotalDoctor = _doctorRepo.Find(d => d.HospitalId == pageId).ToList().Count();
            DashboardTypeModel.BookedAppointment = _appointmentRepo.Find(a => a.DoctorId == pageId && a.Status == "Booked").ToList().Count();
            DashboardTypeModel.CancelAppointment = _appointmentRepo.Find(a => a.DoctorId == pageId && a.Status == "Cancel").ToList().Count();
            var apDate = _appointmentRepo.Find(a => a.DoctorId == pageId ).ToList();
           int appointMentCount = 0;
            foreach (var apt in apDate)
            {
                var dbDate = Convert.ToDateTime(apt.AppointmentDate);
                if (dbDate== DateTime.Today)
                {
                    appointMentCount = appointMentCount + 1;
                }
            }
            DashboardTypeModel.TodayAppointment = _appointmentRepo.Find(a => a.DoctorId == pageId && a.AppointmentDate.ToString() == DateTime.Today.ToString("yyyy-MM-dd")).ToList().Count();
            DashboardTypeModel.NewAppointment = _appointmentRepo.Find(a => a.DoctorId == pageId && a.Status == "0").ToList().Count();

            foreach (var item in _appointmentRepo.Find(a => a.HospitalId == HospitalId).ToList())
            {
                DashboardAppointmentListModel DashboardAppointmentListModel = new DashboardAppointmentListModel();

                DashboardAppointmentListModel.AppointmentDate = item.AppointmentDate;
                DashboardAppointmentListModel.AppointmentId = item.AppointmentId;
                DashboardAppointmentListModel.Status = item.Status;
                DashboardAppointmentListModel.TimeId = item.TimingId;
                int appointmentTimeId = Convert.ToInt32(item.TimingId);
                if (_timeMasterRepo != null)
                {
                    var timeDetails = _timeMasterRepo.Find(t => t.Id == appointmentTimeId && t.IsActive == true).FirstOrDefault();
                    if (timeDetails != null)
                    {
                        DashboardAppointmentListModel.AppointmentTime = timeDetails.TimeFrom.Trim() + "-" + timeDetails.TimeTo.Trim() + " " + timeDetails.AM_PM.Trim();
                    }
                }
                var clientDetail = _clientDetailRepo.Find(x => x.ClientId == item.ClientId).FirstOrDefault();
                if (clientDetail != null)
                {
                    DashboardAppointmentListModel.DOB = clientDetail.DOB;
                    DashboardAppointmentListModel.PatientName = clientDetail.FirstName;

                    DashboardAppointmentListModel.NoorCareNumber = clientDetail.ClientId;
                    DashboardAppointmentListModel.Gender = clientDetail.Gender.ToString();
                }

                var doctorDetails = _doctorRepo.Find(d => d.DoctorId == item.DoctorId).FirstOrDefault();
                if (doctorDetails != null)
                {
                    DashboardAppointmentListModel.DoctorName = doctorDetails.FirstName;
                }
                lstDashboardAppointmentListModel.Add(DashboardAppointmentListModel);
            }

            dashboardModel.DashboardTypeModel = DashboardTypeModel;
            dashboardModel.DashboardAppointmentListModel = lstDashboardAppointmentListModel;
            return Request.CreateResponse(HttpStatusCode.Accepted, dashboardModel);
        }


        [Route("api/GetUpcomingAppointment/{Id}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetUpcomingAppointment(string Id)
        {
            var result = _appointmentRepo.Find(a => DateTime.Parse(a.AppointmentDate) >= DateTime.Now && a.ClientId == Id).ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/GetMedicalInformation/{Id}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetMedicalInformation(string Id)
        {
            var result = _medicalInformationRepo.Find(m => m.clientId == Id).ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [HttpPost]
        [Route("api/UploadBanner/{RequestFrom}")]
        [AllowAnonymous]
        public IHttpActionResult UploadBanner(string RequestFrom)
        {
            string imageName = null;
            string RequestID = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            // string doctorId = httpRequest.Form["DoctorId"];
            string NoorCareId = string.Empty;

            if (RequestFrom.ToLower() == "secretary")
            {
                RequestID = httpRequest.Form["secretaryid"];
                RequestFrom = "Secretary";
            }
            else if (RequestFrom.ToLower() == "doctor")
            {
                RequestID = httpRequest.Form["doctorid"];
                RequestFrom = "Doctor";
            }
            else if (RequestFrom.ToLower() == "hospital")
            {
                RequestID = httpRequest.Form["hospitalid"];
                RequestFrom = "Hospital";
            }

            NoorCareId = httpRequest.Form["noorcareid"];

            try
            {
                var postedFile = httpRequest.Files["Image"];
                if (postedFile != null)
                {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).
                        Take(10).ToArray()).
                        Replace(" ", "-");
                    imageName = RequestID + "." + ImageFormat.Jpeg;

                    var filePath = HttpContext.Current.Server.MapPath("~/ProfilePic/" + RequestFrom + "/" + NoorCareId + "/" + imageName);

                    var DirPath = HttpContext.Current.Server.MapPath("~/ProfilePic/" + RequestFrom + "/" + NoorCareId);

                    bool IfDirectoryNotExists = System.IO.Directory.Exists(DirPath);
                    if (!IfDirectoryNotExists)
                    {
                        System.IO.Directory.CreateDirectory(DirPath);
                    }

                    bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/ProfilePic/" + RequestFrom + "/" + NoorCareId + "/" + imageName));
                    if (exists)
                    {
                        File.Delete(filePath);
                    }

                    postedFile.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
            }
            return Ok(RequestID);
        }


    }
}
