using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("NewsBlogs")]
    public class NewsBlogs : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
       
        public string ContentText { get; set; }

        public Int64 NoOfRead { get; set; }
        public Int64 NoOfLikes { get; set; }
        public string Category { get; set; }

        [MaxLength(50)]
        public string UserId { get; set; }
        [MaxLength(50)]
        public string PageId { get; set; }

        public bool IsDeleted { get; set; }
        [MaxLength(128)]
        public string CreatedBy { get; set; }
        [MaxLength(128)]
        public string ModifiedBy { get; set; }
        [MaxLength(50)]
        public string CreatedDate { get; set; }
        [MaxLength(50)]
        public string ModifiedDate { get; set; }

    }
}