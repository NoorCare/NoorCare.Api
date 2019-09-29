using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("Doctor")]
    public class Doctor : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(300)]
        public string FirstName { get; set; }
        [MaxLength(300)]
        public string LastName { get; set; }
        [MaxLength(256)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string AlternatePhoneNumber { get; set; }

        public int Gender { get; set; }

        [MaxLength(50)]     
        public string Experience { get; set; }
        [Column(TypeName = "Money")]
        public decimal FeeMoney { get; set; }
        [MaxLength(100)]
        public string Language { get; set; }
        [MaxLength(50)]
        public string AgeGroupGender { get; set; }
        [MaxLength(50)]
        public string Degree { get; set; }
        [MaxLength(200)]
        public string Specialization { get; set; }
        [MaxLength(500)]
        public string PhotoPath { get; set; }
        [MaxLength(50)]
        public string DoctorId { get; set; }
        [MaxLength(50)]
        public string HospitalId { get; set; }

        public int jobType { get; set; }
        public int CountryCode { get; set; }

        public bool IsDeleted { get; set; }
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        [MaxLength(128)]
        public string ModifiedBy { get; set; }
        public DateTime? DateEntered { get; set; }
        public DateTime? DateModified { get; set; }

        [MaxLength(1000)]
        public string AboutUs { get; set; }

    }


    [Serializable]
    [Table("DoctorAvailableTime")]
    public class DoctorAvailableTime : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string DoctorId { get; set; }
        [MaxLength(50)]
        public string TimeId { get; set; }
        [MaxLength(100)]
        public string Days { get; set; }
        [MaxLength(50)]
        public string Date { get; set; }
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
        [MaxLength(200)]
        public string CreatedBy { get; set; }
        [MaxLength(200)]
        public string ModifiedBy { get; set; }
        [MaxLength(50)]
        public string DateEntered { get; set; }
        [MaxLength(50)]
        public string DateModified { get; set; }
    }
}