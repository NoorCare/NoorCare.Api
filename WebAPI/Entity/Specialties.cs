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
    [Table("Specialties")]
    public class Specialties : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public int? SpecialtyId { get; set; }
        public string Name { get; set; }
    }
}