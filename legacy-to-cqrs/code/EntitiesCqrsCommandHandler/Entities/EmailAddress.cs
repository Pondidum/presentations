using System;

namespace EntitiesCqrsCommandHandler.Entities
{
	public class EmailAddress : IKeyed
	{
		public Guid ID { get; set; }
		public Guid ParentID { get; set; }

		public string Email { get; set; }
		public Boolean IsPrimary { get; set; }
	}
}
