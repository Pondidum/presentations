using System;

namespace EventSourcing.Infrastructure
{
	public class DomainEvent
	{
		public Guid AggregateID { get; set; }
		public int SequenceID { get; set; }
	}
}
