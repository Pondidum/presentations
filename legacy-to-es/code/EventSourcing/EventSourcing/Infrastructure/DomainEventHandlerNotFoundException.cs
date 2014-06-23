using System;

namespace EventSourcing.Infrastructure
{
	public class DomainEventHandlerNotFoundException : Exception
	{
		public DomainEventHandlerNotFoundException(Type aggregateType, Type eventType)
			: base(string.Format("Unable to find a method to handle {0} on {1}.", eventType.Name, aggregateType.Name))
		{
			
		}
	}
}
