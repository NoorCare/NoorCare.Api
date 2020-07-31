﻿using NoorCare;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("ReportType")]
    public class Report : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public int? ReportID { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }
    }
}