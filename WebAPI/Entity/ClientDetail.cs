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
    public int MobileNo { get; set; }
    public string EmailId { get; set; }
    public string ModifyBy { get; set; }
    public int Jobtype { get; set; }
    public bool EmailConfirmed { get; set; }
    public int MaritalStatus { get; set; }
    public string DOB { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CountryCode { get; set; }
    public int PinCode { get; set; }
}

