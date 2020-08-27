using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Serializable]
[Table("ClientDetail")]
public partial class ClientDetail : IEntity<int>
{
    [Key]
    public int Id { get; set; }
    public string ClientId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public int Gender { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string MobileNo { get; set; }
    public string EmailId { get; set; }
    public string ModifyBy { get; set; }
    public int Jobtype { get; set; }
    public bool EmailConfirmed { get; set; }
    public int MaritalStatus { get; set; }
    public string DOB { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CountryCode { get; set; }
    public string CountryShortCode { get; set; }
    public string CountryShortCodeAlt { get; set; }
    public string CountryCodeAlt { get; set; }
    public int PinCode { get; set; }

    public string ImageUrl { get; set; }

    public bool IsActive { get; set; }
}

[Serializable]
[Table("PatientPrescription")]
public class PatientPrescription : IEntity<int>
{
    [Key]
    public int Id { get; set; }
    [MaxLength(50)]
    public string PatientId { get; set; }
    
    [MaxLength(50)]
    public string DoctorId { get; set; }

    public string Prescription { get; set; }

    public string Report { get; set; }

    public string InsuranceId { get; set; }

    public bool IsDeleted { get; set; }
    [MaxLength(50)]
    public string CreatedBy { get; set; }
    [MaxLength(50)]
    public string ModifiedBy { get; set; }
    public DateTime? DateEntered { get; set; }
    public DateTime? DateModified { get; set; }
    //public WebAPI.Models.Doctors Doctors { get; set; }
}

[Serializable]
[Table("PatientPrescriptionAssign")]
public class PatientPrescriptionAssign : IEntity<int>
{
    [Key]
    public int Id { get; set; }
    public int PatientPresId { get; set; }

    public string AssignId { get; set; }
    public bool IsActive { get; set; }
    public string AssignBy { get; set; }
   
    public DateTime? AssignDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}


[Serializable]
[Table("LEAD")]
public partial class Lead : IEntity<int>
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }     
    public string City { get; set; }
    public string Country { get; set; }
    public string Website { get; set; }
    public DateTime DateEntered { get; set; }
}

