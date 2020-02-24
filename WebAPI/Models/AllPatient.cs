using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class AllPatient
    {
        public List<Patient> Patients { get; set; }

    }
    public class Patient
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string appointmentId { get; set; }
        public string LastVisit { get; set; }
        public string LastVisitDate { get; set; }
        public string LastVisitTime { get; set; }
        public string TotalVisit { get; set; }
    }
}