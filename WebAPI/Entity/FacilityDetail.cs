using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("FacilityDetail")]
    public class FacilityDetail : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string FacilityDetailId { get; set; }

        public int FacilityId { get; set; }
        [MaxLength(300)]
        public string ProviderName { get; set; }
        [MaxLength(200)]
        public string FirstName { get; set; }
        [MaxLength(200)]
        public string LastName { get; set; }
        public int CountryCode { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        public int jobType { get; set; }

        [MaxLength(1000)]
        public string AboutUs { get; set; }

        [MaxLength(250)]
        public string PhotoPath { get; set; }
        [MaxLength(100)]
        public string Website { get; set; }

        [MaxLength(50)]
        public string EstablishYear { get; set; }

        [MaxLength(300)]
        public string Address { get; set; }
        [MaxLength(300)]
        public string Street { get; set; }

        [MaxLength(300)]
        public string Country { get; set; }

        [MaxLength(300)]
        public string City { get; set; }

        [MaxLength(20)]
        public string PostCode { get; set; }
        [MaxLength(300)]
        public string Landmark { get; set; }
        [MaxLength(100)]
        public string MapLocation { get; set; }
        [MaxLength(100)]
        public string Specialization { get; set; }
        [MaxLength(100)]
        public string Amenities { get; set; }

        [MaxLength(100)]
        public string Services { get; set; }

        [MaxLength(100)]
        public string Timing { get; set; }

        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DateEntered { get; set; }
        public string DateModified { get; set; }

    }
}