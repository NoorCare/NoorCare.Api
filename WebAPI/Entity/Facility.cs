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
    [Table("Facility")]
    public class Facility : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string facility { get; set; }

    }

    [Serializable]
    [Table("CountryCode")]
    public class CountryCode : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string CountryCodes { get; set; }

    }

    [Serializable]
    [Table("citys")]
    public class City : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string city { get; set; }

    }

    [Serializable]
    [Table("tblState")]
    public class State : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string state { get; set; }

    }
}