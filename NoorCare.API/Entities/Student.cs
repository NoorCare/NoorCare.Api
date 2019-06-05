using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryPattern.Core.DomainObjecs
{
    [Serializable]
    [Table("Student")]
    public partial class Student: IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string SurName { get; set; }

        [Required]
        [StringLength(5)]
        public string Classroom { get; set; }
    }
}
