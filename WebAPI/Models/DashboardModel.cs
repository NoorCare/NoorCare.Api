using System.Collections.Generic;
namespace WebAPI.Models
{
    public class DashboardModel
    {
        public DashboardTypeModel DashboardTypeModel { get; set; }
        public List<DashboardAppointmentListModel> DashboardAppointmentListModel { get; set; }

    }

    public class DashboardTypeModel
    {
        public string HospitalId { get; set; }
        public int TotalDoctor { get; set; }
        public int BookedAppointment { get; set; }
        public int CancelAppointment { get; set; }
        public int NewAppointment { get; set; }
        public int TodayAppointment { get; set; }
        public int DoctorOnLeave { get; set; }
        public int TotalFeedback { get; set; }
        public int TotalDoctorPrescription { get; set; }
        public int TotalMedicalFile { get; set; }
    }

    public class DashboardAppointmentListModel
    {
        public string AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string TimeId { get; set; }
        public string Status { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string NoorCareNumber { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
    }
}