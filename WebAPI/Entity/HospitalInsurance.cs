using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("HospitalInsurance")]
    public partial class HospitalInsurance : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string HospitalId { get; set; }

        public int InsuranceId { get; set; }

        public bool? IsActive { get; set; }

        [MaxLength(100)]
        public string CreatedBy { get; set; }

        [MaxLength(100)]
        public string ModifiedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}