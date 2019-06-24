using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Entity
{
    [Serializable]
    [Table("EmergencyContact")]
    public class EmergencyContact : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string clientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public int Relationship { get; set; }
        public string Email { get; set; }
        public int Mobile { get; set; }
        public int AlternateNumber { get; set; }
        public int WorkNumber { get; set; }
        public string Address { get; set; }
    }
}