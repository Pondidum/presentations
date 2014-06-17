using System;
using System.Collections.Generic;

namespace CommandHandler
{
	public class InMemoryBus : IBus
	{
		private readonly Dictionary<Type, List<Handler>> _handlers;

		public InMemoryBus()
		{
			_handlers = new Dictionary<Type, List<Handler>>();
		}

		public void Publish<T>(T message)
		{
			var type = typeof(T);

			List<Handler> handlers;
			_handlers.TryGetValue(type, out handlers);

			if (handlers == null)
			{
				return;
			}

			foreach (var entry in handlers)
			{
				entry.Action.Invoke(message);
			}
		}

		public void Wire<T>(Action<T> handler)
		{
			if (handler == null) throw new ArgumentNullException("handler");

			var type = typeof (T);

			List<Handler> handlers;
			_handlers.TryGetValue(type, out handlers);

			if (handlers == null)
			{
				handlers = new List<Handler>();
				_handlers[type] = handlers;
			}

			handlers.Add(new Handler(handler.GetHashCode(), message => handler.Invoke((T)message)));

		}

		public void UnWire<T>(Action<T> handler)
		{
			if (handler == null) throw new ArgumentNullException("handler");

			var type = typeof (T);
			var hash = handler.GetHashCode();

			List<Handler> handlers;
			_handlers.TryGetValue(type, out handlers);

			if (handlers == null)
			{
				return;
			}

			handlers.RemoveAll(h => h.Hash == hash);
		}

		private struct Handler
		{
			public readonly int Hash;
			public readonly Action<object> Action;

			public Handler(int hash, Action<Object> action)
			{
				Hash = hash;
				Action = action;
			}
		}
	}
}
