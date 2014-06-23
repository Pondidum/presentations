using System.Collections.Generic;

namespace EventSourcing.Infrastructure
{
	public interface IEventStream
	{
		IEnumerable<DomainEvent> GetEvents();
		void Clear();
		void LoadFromEvents(IEnumerable<DomainEvent> events);
	}
}
