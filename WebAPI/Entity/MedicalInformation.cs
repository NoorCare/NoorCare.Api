using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("MedicalInformation")]
    public class MedicalInformation
    {
        [Key]
        public int Id { get; set; }
        public string clientId { get; set; }
        public int Hight { get; set; }
        public int Wight { get; set; }
        public int BloodGroup { get; set; }
        public int Diseases { get; set; }
        public Boolean AnyAllergies { get; set; }
        public Boolean AnyHealthCrisis { get; set; }
        public Boolean AnyRegularMedication { get; set; }
        public Boolean Disability { get; set; }
        public Boolean Smoke { get; set; }
        public Boolean Drink { get; set; }
        public string OtherDetails { get; set; }
    }
}