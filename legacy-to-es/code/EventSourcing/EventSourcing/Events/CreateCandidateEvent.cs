using System;
using EventSourcing.Infrastructure;

namespace EventSourcing.Events
{
	public class CreateCandidateEvent : DomainEvent
	{
		public Guid CandidateID { get; private set; }
		public string Name { get; private set; }
		public DateTime DoB { get; private set; }

		public CreateCandidateEvent(Guid candidateID, string name, DateTime dob)
		{
			CandidateID = candidateID;
			Name = name;
			DoB = dob;
		}
	}
}
