using NoorCare;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("DoctorEducation")]
    public class DoctorEducation : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string Education { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string ModifyAt { get; set; }
        public string ModifyBy { get; set; }
        public bool? Active { get; set; }
    }
}