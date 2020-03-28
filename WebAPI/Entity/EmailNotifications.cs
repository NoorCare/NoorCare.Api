using NoorCare;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebAPI.Entity
{
	[Serializable]
	[Table("EmailNotifications")]
	public class EmailNotifications : IEntity<int>
	{
		[Key]
		public int Id { get; set; }
		public string To { get; set; }
		public string From { get; set; }
		public string Description { get; set; }
		public bool IsDeleted { get; set; }
		public string Subject { get; set; }
		public string Attachments { get; set; }
	}
}