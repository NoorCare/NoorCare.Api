using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Serializable]
[Table("HospitalDetails")]
public partial class HospitalDetails : IEntity<int>
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string HospitalId { get; set; }
    [MaxLength(300)]
    public string HospitalName { get; set; }
    public int CountryCode { get; set; }
    public Int64 Mobile { get; set; }
    public Int64 AlternateNumber { get; set; }
    [MaxLength(100)]
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    [MaxLength(100)]
    public string Website { get; set; }
    [MaxLength(50)]
    public string EstablishYear { get; set; }
    public Int64 NumberofBed { get; set; }
    public Int64 NumberofAmbulance { get; set; }
    [MaxLength(100)]
    public string PaymentType { get; set; }
    public bool Emergency { get; set; }
    public int FacilityId { get; set; }
    public int jobType { get; set; }

    //HospitalAddress------
    [MaxLength(300)]
    public string Address { get; set; }
    [MaxLength(300)]
    public string Street { get; set; }
    [MaxLength(100)]
    public string Country { get; set; }
    [MaxLength(200)]
    public string City { get; set; }
    public string PostCode { get; set; }
    [MaxLength(200)]
    public string Landmark { get; set; }

    [MaxLength(100)]
    public string InsuranceCompanies { get; set; }

    [MaxLength(100)]
    public string MapLocation { get; set; }

    [MaxLength(50)]
    public string Specialization { get; set; }
    [MaxLength(50)]
    public string Amenities { get; set; }
    [MaxLength(50)]
    public string Services { get; set; }

    public bool Timing { get; set; }

    //WorkingDay
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }
    public bool Sunday { get; set; }

    public bool IsDeleted { get; set; }
    [MaxLength(50)]
    public string CreatedBy { get; set; }
    [MaxLength(50)]
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? CreatedDate { get; set; }
}

