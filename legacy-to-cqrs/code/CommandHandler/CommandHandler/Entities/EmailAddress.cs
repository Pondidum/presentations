using System;

namespace CommandHandler.Entities
{
	public class EmailAddress : IKeyed
	{
		public Guid ID { get; set; }
		public Guid ParentID { get; set; }

		public string Email { get; set; }
		public Boolean IsPrimary { get; set; }
	}
}
