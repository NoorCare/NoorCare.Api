﻿using System;
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
        public int AnyAllergies { get; set; }
        public int AnyHealthCrisis { get; set; }
        public int AnyRegularMedication { get; set; }
        public int Disability { get; set; }
        public int Smoke { get; set; }
        public int Drink { get; set; }
        public string OtherDetails { get; set; }
        public int? Pressure { get; set; }
        public int? Heartbeats { get; set; }
        public int? Temprature { get; set; }
        public int? Sugar { get; set; }
        public int? Cholesterol { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }





    }
}