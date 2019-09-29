using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("Appointment")]
    public class Appointment : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string AppointmentId { get; set; }

        [MaxLength(50)]
        public string DoctorId { get; set; }

        [MaxLength(50)]
        public string HospitalId { get; set; }

        [MaxLength(50)]
        public string TimingId { get; set; }

        [MaxLength(50)]
        public string AppointmentDate { get; set; }
        
        [MaxLength(50)]
        public string ClientId { get; set; }

        [MaxLength(50)]
        public string MedicalInfoId { get; set; }

        public int CountryCode { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public bool IsDeleted { get; set; }
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        [MaxLength(128)]
        public string ModifiedBy { get; set; }
        public string DateEntered { get; set; }
        public string DateModified { get; set; }
    }
}