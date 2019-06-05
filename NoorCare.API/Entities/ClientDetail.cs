using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Serializable]
[Table("ClientDetail")]
public partial class ClientDetail : IEntity<int>
{
        [Key]
        public Nullable<int> id { get; set; }

        public string clientId { get; set; }
        public string name { get; set; }
        public int gender { get; set; }
        public string Address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public int mobileNo { get; set; }
        public string emailId { get; set; }
        public DateTime createDate { get; set; }
        public DateTime modifyDate { get; set; }
        public string modifyBy { get; set; }
        public int jobtype { get; set; }
    
    }

