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

        public string Category { get; set; }

		public string NewsTitle { get; set; }

		public string ImageURL { get; set; }

		public long  NoOfRead { get; set; }

		public long NoOfLikes { get; set; }

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

    [Serializable]
    [Table("ReadLike")]
    public class ReadLike : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string UserId { get; set; }
        [MaxLength(50)]
        public string Type { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsLike { get; set; } = false;
        [MaxLength(50)]
        public string ReadDate { get; set; }
        [MaxLength(50)]
        public string LikeDate { get; set; }

    }
}