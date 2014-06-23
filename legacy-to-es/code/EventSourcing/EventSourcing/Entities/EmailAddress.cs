using System;

namespace EventSourcing.Entities
{
	public class EmailAddress
	{
		public string Email { get; set; }
		public Boolean IsPrimary { get; set; } 
	}
}
