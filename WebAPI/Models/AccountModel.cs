using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class AccountModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int jobType { get; set; }
        public string AccountType { get; set; }
        public int Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string LoggedOn { get; set; }
        public bool EmailConfirmed { get; set; }
        public int CountryCode { get; set; }
        public int FacilityId { get; set; }
    }

    public class FacilityModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FacilityDetailId { get; set; }
        public int FacilityId { get; set; }
        public string ProviderName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CountryCode { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public int jobType { get; set; }
        public string AboutUs { get; set; }
        public string PhotoPath { get; set; }
        public string Website { get; set; }
        public string EstablishYear { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Landmark { get; set; }
        public string MapLocation { get; set; }
        public string Specialization { get; set; }
        public string Amenities { get; set; }
        public string Services { get; set; }
        public string Timing { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DateEntered { get; set; }
        public string DateModified { get; set; }
    }

    public class ViewAccount
    {
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo { get; set; }
        public string JobType { get; set; }
    }

    public class ChangePassword
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
      
    }

    public class ForgetPassword
    {
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string Email { get; set; }
    }
}