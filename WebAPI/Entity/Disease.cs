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
    [Table("DiseaseType")]
    public class Disease : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string DiseaseType { get; set; }

    }
}