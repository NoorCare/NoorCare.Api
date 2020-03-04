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
    [Table("FacilityImages")]
    public class FacilityImages : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string FacilityNoorCareNumber { get; set; }
        [MaxLength(20)]
        public string FacilityImageType { get; set; }
        [MaxLength(300)]
        public string FileName { get; set; }
        [MaxLength(500)]
        public string FilePath { get; set; }
        [MaxLength(50)]
        public string ExpiryDate { get; set; }

        public DateTime DateEntered { get; set; }

        public DateTime? DateModified { get; set; }
    }
}