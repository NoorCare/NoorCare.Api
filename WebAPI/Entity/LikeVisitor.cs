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
    [Table("LikeVisitor")]
    public class LikeVisitor : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string NoorId { get; set; }
        public string HFP_DOC_NCID { get; set; }
        public bool Like_Dislike { get; set; }
        public bool Visitor { get; set; }
        public bool IsDelete { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifyBy { get; set; }


    }
}