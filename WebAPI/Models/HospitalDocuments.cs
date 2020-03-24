using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class HospitalDocuments
    {
        public string HospitalId { get; set; }
        public string IdBackView { get; set; }
        public string IdFrontView { get; set; }
        public string CrFrontView { get; set; }

        
        public string LicenseFrontView { get; set; }

       
        public string Address { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}