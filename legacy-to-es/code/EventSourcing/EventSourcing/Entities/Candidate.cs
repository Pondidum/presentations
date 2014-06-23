using System;
using System.Collections.Generic;

namespace EventSourcing.Entities
{
	public class Candidate
	{
		public string Name { get; set; }
		public DateTime DoB { get; set; }
		public Sexes Sex { get; set; }

		public List<EmailAddress> Emails { get; private set; }
		public List<Phone> Phones { get; private set; }

		public Candidate()
		{
			Emails = new List<EmailAddress>();
			Phones = new List<Phone>();
		}
	}
}
