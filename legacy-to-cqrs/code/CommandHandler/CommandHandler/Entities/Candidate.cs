using System;
using System.Collections.Generic;

namespace CommandHandler.Entities
{
	public class Candidate : IKeyed
	{
		public Guid ID { get; set; }
		public string Name { get; set; }
		public DateTime DoB { get; set; }
		public Sexes Sex { get; set; }

		public List<EmailAddress> Emails { get; internal set; }
		public List<Phone> Phones { get; internal set; }

		public Candidate()
		{
			Emails=new List<EmailAddress>();
			Phones = new List<Phone>();
		}
	}
}
