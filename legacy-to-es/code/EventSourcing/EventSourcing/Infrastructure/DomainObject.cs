using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventSourcing.Infrastructure
{
	public class DomainObject : IEventStream
	{
		private const BindingFlags MethodFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		private readonly Dictionary<Type, Action<object>> _domainEventHandlers;
		private readonly List<DomainEvent> _events;
		private int _nextEventSequenceID;

		public Guid ID { get; protected set; }

		public DomainObject()
		{
			_nextEventSequenceID = 0;
			_events = new List<DomainEvent>();

			_domainEventHandlers = GetType()
				.GetMethods(MethodFlags)
				.Where(m => m.GetParameters().OneOnly())
				.Where(m => m.GetParameters().First().ParameterType.Inherits<DomainEvent>())
				.ToDictionary(
					m => m.GetParameters().First().ParameterType,
					m => new Action<Object>(x => m.Invoke(this, new object[] { x })));
		}

		protected void ApplyEvent(DomainEvent domainEvent)
		{
			domainEvent.SequenceID = _nextEventSequenceID;

			Apply(domainEvent);
			_events.Add(domainEvent);

			_nextEventSequenceID++;
		}

		private void Apply(DomainEvent domainEvent)
		{
			var specific = _domainEventHandlers.GetOrDefault(domainEvent.GetType());

			if (specific == null)
			{
				throw new DomainEventHandlerNotFoundException(GetType(), domainEvent.GetType());
			}

			specific.Invoke(domainEvent);

			domainEvent.AggregateID = ID;
		}

		IEnumerable<DomainEvent> IEventStream.GetEvents()
		{
			return _events;
		}

		void IEventStream.Clear()
		{
			_events.Clear();
		}

		void IEventStream.LoadFromEvents(IEnumerable<DomainEvent> events)
		{
			var lastID = 0;

			foreach (var domainEvent in events)
			{
				Apply(domainEvent);
				lastID = domainEvent.SequenceID;
			}

			_nextEventSequenceID = lastID + 1;
		}
	}
}
