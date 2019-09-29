using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("TimeMaster")]
    public class TimeMaster : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(10)]
        public string TimeFrom { get; set; }
        [MaxLength(10)]
        public string TimeTo { get; set; }
        [MaxLength(10)]
        public string AM_PM { get; set; }
        public bool IsActive { get; set; }
    }
}