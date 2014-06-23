using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Events;
using EventSourcing.Infrastructure;

namespace EventSourcing.Entities
{
	public class Candidate : DomainObject
	{
		private readonly List<EmailAddress> _emails;
		private readonly List<Phone> _phones;

		public string Name { get; set; }
		public DateTime DoB { get; set; }
		public Sexes Sex { get; set; }

		public IEnumerable<EmailAddress> Emails { get { return _emails; } }
		public IEnumerable<Phone> Phones { get { return _phones; } }

		public Candidate()
		{
			_emails = new List<EmailAddress>();
			_phones = new List<Phone>();
		}

		public static Candidate Create(string name, DateTime dob)
		{
			var candidate = new Candidate();
			candidate.ApplyEvent(new CreateCandidateEvent(name, dob));

			return candidate;
		}

		public void AddPhone(string number, string extension)
		{
			ApplyEvent(new AddPhoneEvent(number, extension));
		}


		private void OnCreateCandidate(CreateCandidateEvent domainEvent)
		{
			ID = domainEvent.CandidateID;
			Name = domainEvent.Name;
			DoB = domainEvent.DoB;
		}

		private void OnAddPhone(AddPhoneEvent domainEvent)
		{
			_phones.Add(new Phone
			{
				Number = domainEvent.Number, 
				Extension = domainEvent.Extension
			});
		}
	}
}
