﻿using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("Feedback")]
    public class Feedback : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string FeedbackID { get; set; }
        
        public string ClientID { get; set; }

        public string DoctorID { get; set; }

        public string FeedbackDetails { get; set; }

        public bool RecommendedDoctor { get; set; }
        public string Recommended { get; set; }

        public bool ILike { get; set; }

        public string PageId { get; set; }

        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime DateEntered { get; set; }
        public DateTime DateModified { get; set; }
    }



    [Serializable]
    [Table("ContactUs")]
    public class ContactUs : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string MobileNumber { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }

        [MaxLength(50)]
        public string PageId { get; set; }

        [MaxLength(200)]
        public string EmailId { get; set; }

        public bool IsDeleted {
            get;
            set;
          }
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        [MaxLength(128)]
        public string ModifiedBy { get; set; }
        [MaxLength(50)]
        public string DateEntered { get; set; }
        [MaxLength(50)]
        public string DateModified { get; set; }

    }
}