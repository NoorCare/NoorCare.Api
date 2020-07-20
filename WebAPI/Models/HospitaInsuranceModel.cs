using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class HospitaInsuranceModel
    {
        public string HospitalId { get; set; }
        public string InsuranceIdes { get; set; }
        public string InsuranceId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
        public bool? IsHospital { get; set; } = false;
    }
}