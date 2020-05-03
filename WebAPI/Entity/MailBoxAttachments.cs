using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
   
    [Serializable]
    [Table("MailBoxAttachments")]
    public class MailBoxAttachments : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("MailBox")]
        public int MailBoxId { get; set; }

        [MaxLength(100)]
        public string FileType { get; set; }

        [MaxLength(100)]
        public string FileName { get; set; }

        [MaxLength(250)]
        public string FilePath { get; set; }

        //[MaxLength(100)]
        //public DateTime? ExpireDate { get; set; }

        [MaxLength(10)]
        public string Status { get; set; }

        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }

    }
}