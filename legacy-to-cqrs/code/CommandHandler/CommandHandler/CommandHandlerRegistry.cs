using System;
using System.Collections.Generic;
using System.Linq;
using CommandHandler.Infrastructure;

namespace CommandHandler
{
	public class CommandHandlerRegistry : ICommandHandlerRegistry
	{
		public IEnumerable<Type> Commands { get; private set; }
		public IDictionary<Type, IList<Type>> CommandHandlers { get; private set; }

		public CommandHandlerRegistry()
		{
			Commands = GetCommands();
			CommandHandlers = GetCommandHandlers();
		}

		private IDictionary<Type, IList<Type>> GetCommandHandlers()
		{
			IDictionary<Type, IList<Type>> handlers = new Dictionary<Type, IList<Type>>();

			typeof(ICommandHandler<>)
				.Assembly
				.GetExportedTypes()
				.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
				.Each(t => Add(handlers, t));
			return handlers;
		}

		private IEnumerable<Type> GetCommands()
		{
			return typeof(ICommand)
				.Assembly
				.GetExportedTypes()
				.Where(x => x.GetInterfaces().Contains(typeof(ICommand)))
				.ToList();
		}

		private void Add(IDictionary<Type, IList<Type>> commands, Type type)
		{
			var command = type
				.GetInterfaces()
				.First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
				.GetGenericArguments()
				.First();

			if (commands.ContainsKey(command) == false)
			{
				commands[command] = new List<Type>();
			}

			commands[command].Add(type);
		}
	}
}
