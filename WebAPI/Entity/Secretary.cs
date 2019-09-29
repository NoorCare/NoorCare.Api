using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("Secretary")]
    public class Secretary : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(300)]
        public string FirstName { get; set; }
        [MaxLength(300)]
        public string LastName { get; set; }
        [MaxLength(256)]
        public string Email { get; set; }
        public int CountryCode { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string AlternatePhoneNumber { get; set; }
        public int Gender { get; set; }
        [MaxLength(50)]     
        public string YearOfExperience { get; set; }
        [MaxLength(50)]
        public string SecretaryId { get; set; }
        [MaxLength(50)]
        public string HospitalId { get; set; }
        public int jobType { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        [MaxLength(128)]
        public string ModifiedBy { get; set; }
        public DateTime? DateEntered { get; set; }
        public DateTime? DateModified { get; set; }
    }
}