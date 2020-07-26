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
    [Table("LeaveDetail")]
    public class LeaveDetail : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string ClientId { get; set; }
        public int LeaveId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TimeId { get; set; }
        public bool IsDeleted { get; set; } 
        public string Remarks { get; set; }
        public string Createdby { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ModifyBy { get; set; }
    }
}