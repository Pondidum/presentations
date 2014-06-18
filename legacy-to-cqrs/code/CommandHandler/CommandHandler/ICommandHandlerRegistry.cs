using System;
using System.Collections.Generic;

namespace CommandHandler
{
	public interface ICommandHandlerRegistry
	{
		IEnumerable<Type> Commands { get; }
		IDictionary<Type, IList<Type>> CommandHandlers { get; }
	}
}
