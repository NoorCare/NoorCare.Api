using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("HospitalDocumentVerification")]
    public partial class HospitalDocumentVerification : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]

        public string HospitalId { get; set; }
        [MaxLength(300)]

        public string IdBackView { get; set; }

        [MaxLength(300)]
        public string IdFrontView { get; set; }

        [MaxLength(300)]
        public string CrFrontView { get; set; }

        [MaxLength(300)]
        public string LicenseFrontView { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}