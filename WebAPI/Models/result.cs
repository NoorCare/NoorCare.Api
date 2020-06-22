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

        public List<Doctors> Doctors { get; set; }
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
        public int Id { get; set; }
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
        public int JobType { get; set; }
        public int JobTypePermission { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Landmark { get; set; }
        public string AboutUs { get; set; }
        public string InsuranceCompanies { get; set; }
        public int[] AmenitiesIds { get; set; }
        public List<TblHospitalAmenities> Amenities { get; set; }
        public int[] ServicesIds { get; set; }
        public List<TblHospitalServices> Services { get; set; }

        public int[] SpecializationIds { get; set; }
        public List<TblHospitalSpecialties> Specialization { get; set; }

        public List<Doctors> Doctors { get; set; }
        public List<Secretary> Secretary { get; set; }
        
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
        public string HospitalId { get; set; }
        public string location { get; set; }
        public string aboutMe { get; set; }
        public string website { get; set; }
        public string Address { get; set; }
        public string HospitalPicUrl { get; set; }
        public string HospitalEmail { get; set; }
        public string HospitalWebsite { get; set; }
        public string HospitalAddress { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public long Mobile { get; set; }
        public List<DoctorAvailableTime> DoctorAvilability { get; set; }
        public FilterDoctor FilterDoctor { get; set; }
        public List<Disease> Specialization { get; set; }
        public List<TimeMaster> TimeAvailability { get; set; }
        public List<Feedback> Feedback { get; set; }
        public List<TblHospitalAmenities> Amenities { get; set; }
        public List<TblHospitalServices> Services { get; set; }        
    }

    public class Facilities
    {
        public int Id { get; set; }
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
        //public string Specialization { get; set; }
        public int[] SpecializationIds { get; set; }
        public List<Disease> Specialization { get; set; }
        //public string Amenities { get; set; }
        //public string Services { get; set; }
        public List<TblHospitalAmenities> Amenities { get; set; }
        public List<TblHospitalServices> Services { get; set; }
        public string Timing { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DateEntered { get; set; }
        public DateTime? DateModified { get; set; }
    }


    public class DesiesTypeResult
    {
        public Int32 DiseaseType { get; set; }
        public string DesiesName { get; set; }
        public List<Int32?> Years { get; set; }
        public List<YearList> YearList { get; set; }
        public Boolean IsChecked { get; set; }
    }



    public class YearList
    {
        public Int32? Year { get; set; }
        public List<Int32?> Month { get; set; }
        public List<MonthList> MonthList { get; set; }
        public Boolean IsChecked { get; set; }
    }

    public class MonthList
    {
        public Int32? Month { get; set; }
        public List<FileName> FileList { get; set; }
        public Boolean IsChecked { get; set; }
    }

    public class FileName
    {
        public string DocName { get; set; }
        public string DocUrl { get; set; }
        public string HospitalId { get; set; }

        public string UploadedBy { get; set; }

        public int Id { get; set; }
        public Boolean IsChecked { get; set; }
    }

    public class PatientPrescriptionList 
    {
        
        public int Id { get; set; }
       
        public string PatientId { get; set; }

        
        public string DoctorId { get; set; }

        public string Prescription { get; set; }

        public string Report { get; set; }

        public bool IsDeleted { get; set; }
        
        public string CreatedBy { get; set; }
        
        public string ModifiedBy { get; set; }
        public DateTime? DateEntered { get; set; }
        public DateTime? DateModified { get; set; }

        public string DoctorImgUrl { get; set; }
        public string DoctorFirstName { get; set; }
        public string DoctorLastName { get; set; }
        public string DoctorPhoneNumber { get; set; }
        public string DoctorEmail { get; set; }
        public string HospitalPicUrl { get; set; }
        public string HospitalName { get; set; }
        public string HospitalAddress { get; set; }
        public string HospitalEmail { get; set; }
    }


    public class SearchFilter
    {
        public int CountryId { get; set; }
        public int CityID { get; set; }
        public int HealthProvider { get; set; }
        public string DiseaseType { get; set; }
        public int Type { get; set; }

        public int FacilityId { get; set; }
        public string HospitalID { get; set; }
        public string HospitalName { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public int ByPriceMax { get; set; }
        public int ByPriceMin { get; set; }
        public string LookingFor { get; set; }
        public int DoctorGender { get; set; }
        public string DoctorLanguage { get; set; }
    }
}