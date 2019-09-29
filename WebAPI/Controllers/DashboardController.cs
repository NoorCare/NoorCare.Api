using AngularJSAuthentication.API.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        //IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);


        [HttpGet]
        [Route("api/GetDashboardDetails/{Type}/{pageId}/{searchDate?}")] //Type= Doctor/secretary, page Id is secratoryId
        [AllowAnonymous]
        public HttpResponseMessage GetDashboardDetails(string Type,string pageId, string searchDate = null)
        {
            var HospitalId = "";
            if (Type.ToLower() == "secretary")
            {
                HospitalId = _secretaryRepo.Find(s => s.SecretaryId == pageId).FirstOrDefault().HospitalId;
            }
            else if(Type.ToLower() == "doctor")
            {
                HospitalId = _doctorRepo.Find(d => d.DoctorId == pageId).FirstOrDefault().HospitalId;
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

                    DashboardTypeModel.BookedAppointment = _appointmentRepo.Find(a => a.HospitalId == HospitalId && a.Status == "booked").ToList().Count();

                    DashboardTypeModel.CancelAppointment = _appointmentRepo.Find(a => a.HospitalId == HospitalId && a.Status == "cancel").ToList().Count();

                    DashboardTypeModel.NewAppointment = _appointmentRepo.Find(a => a.HospitalId == HospitalId && a.Status == "pending").ToList().Count();

                    DashboardTypeModel.TodayAppointment = _appointmentRepo.Find(a => a.HospitalId == HospitalId && a.Status == "booked" && a.AppointmentDate == searchDate).ToList().Count();
                }
                foreach (var item in _appointmentRepo.Find(a => a.HospitalId == HospitalId).ToList())
                {
                    DashboardAppointmentListModel DashboardAppointmentListModel = new DashboardAppointmentListModel();

                    DashboardAppointmentListModel.AppointmentDate = item.AppointmentDate;
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
            }
            else if (Type.ToLower() == "patient")
            {
                DashboardTypeModel.BookedAppointment = _appointmentRepo.Find(a => a.ClientId == pageId && a.Status == "booked").ToList().Count();
                DashboardTypeModel.TotalFeedback = _feedbackRepo.Find(x => x.ClientID == pageId).ToList().Count();
                DashboardTypeModel.TotalDoctorPrescription = _patientPrescriptionRepo.Find(p => p.PatientId == pageId).ToList().Count();
                DashboardTypeModel.TotalMedicalFile = _quickUploadRepo.Find(q => q.ClientId == pageId).ToList().Count();
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
            var result = _medicalInformationRepo.Find(m =>m.clientId == Id).ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        //[Route("api/GetAllAppointmentList/{Id}")]
        //[HttpGet]
        //[AllowAnonymous]
        //public HttpResponseMessage GetAllAppointmentList(string Id)
        //{
        //    var result = _medicalInformationRepo.Find(m => m.clientId == Id).ToList();
        //    return Request.CreateResponse(HttpStatusCode.Accepted, result);
        //}
    }
}
