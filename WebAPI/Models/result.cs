using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Entity;
using WebAPI.Models;

namespace WebAPI.Models
{
    public class result
    {
        public List<Hospital> Hospitals { get; set; }
        public FilterHospital FilterHospital { get; set; }
        public FilterDoctor FilterDoctor { get; set; }
    }

    public class CommanFilterHospital
    {
        public List<FilterData> Services { get; set; }
        public List<FilterData> Specialization { get; set; }

    }
    public class FilterHospital: CommanFilterHospital
    {
        public List<FilterData> Amenities { get; set; }
    }

    public class FilterDoctor : CommanFilterHospital
    {
        public List<decimal> Price { get; set; }
    }

    public class FilterData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Hospital
    {
        public string HospitalId { get; set; }
        public string HospitalName { get; set; }
        public Int64 Mobile { get; set; }
        public Int64 AlternateNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string EstablishYear { get; set; }
        public Int64 NumberofBed { get; set; }
        public Int64 NumberofAmbulance { get; set; }
        public string PaymentType { get; set; }
        public bool Emergency { get; set; }
        public int FacilityId { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Landmark { get; set; }
        public string InsuranceCompanies { get; set; }
        public int[] AmenitiesIds { get; set; }
        public List<TblHospitalAmenities> Amenities { get; set; }
        public int[] ServicesIds { get; set; }
        public List<TblHospitalServices> Services { get; set; }

        public int[] SpecializationIds { get; set; }
        public List<Disease> Specialization { get; set; }

        public List<Doctors> Doctors { get; set; }
        public FilterHospital FilterHospital { get; set; }
        public int Likes { get; set; }
        public int Feedbacks { get; set; }
        public string BookingUrl { get; set; }
        public string ProfileDetailUrl { get; set; }
        public string ImgUrl { get; set; }
    }

    public class Doctors
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public int Gender { get; set; }
        public string Experience { get; set; }
        public decimal FeeMoney { get; set; }
        public string Language { get; set; }
        public string AgeGroupGender { get; set; }
        public string Degree { get; set; }
        public int[] SpecializationIds { get; set; }
        public string AboutUs { get; set; }
        public int Likes { get; set; }
        public int Feedbacks { get; set; }
        public string BookingUrl { get; set; }
        public string ProfileDetailUrl { get; set; }
        public string ImgUrl { get; set; }
        public string HospitalName { get; set; }
        public string location { get; set; }
        public string aboutMe { get; set; }
        public string website { get; set; }
        public string Address { get; set; }
        public List<DoctorAvailableTime> DoctorAvilability { get; set; }
        public FilterDoctor FilterDoctor { get; set; }
        public List<Disease> Specialization { get; set; }
        public List<Feedback> Feedback { get; set; }
        public List<TblHospitalAmenities> Amenities { get; set; }
        public List<TblHospitalServices> Services { get; set; }
        
    }
}