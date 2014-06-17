using System;

namespace Cqrs
{
	public class EmailAddress
	{
		public Guid ID { get; set; }
		public Guid ParentID { get; set; }

		public string Email { get; set; }
		public Boolean IsPrimary { get; set; }
	}
}
