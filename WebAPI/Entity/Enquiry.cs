using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NoorCare;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{   

    [Serializable]
    [Table("Enquiry")]
    public class Enquiry : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public string ContactNo { get; set; }
        [Required]
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}