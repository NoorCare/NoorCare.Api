using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
   
    [Serializable]
    [Table("MailBox")]
    public class MailBox : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string EmailFrom { get; set; }

        [MaxLength(300)]
        public string EmailTo { get; set; }

        [MaxLength(1000)]
        public string EmailBody { get; set; }

        [MaxLength(200)]
        public string EmailSubject { get; set; }

        [MaxLength(100)]
        public string LabelName { get; set; }

        [MaxLength(50)]
        public string EmailStatus { get; set; }

        public string CreateUser { get; set; }        
        public DateTime CreateDate { get; set; }
        public string ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }

    }
}