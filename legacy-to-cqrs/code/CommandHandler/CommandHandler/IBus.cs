using System;

namespace CommandHandler
{
	public interface IBus
	{
		void Publish<T>(T message);

		void Wire<T>(Action<T> handler);
		void UnWire<T>(Action<T> handler);
	}
}
