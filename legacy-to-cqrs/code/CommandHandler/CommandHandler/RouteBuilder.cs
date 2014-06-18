using System;
using System.Linq;
using System.Reflection;
using CommandHandler.Infrastructure;
using StructureMap;

namespace CommandHandler
{
	public class RouteBuilder
	{
		public RouteBuilder(IContainer container, IBus bus, ICommandHandlerRegistry registry)
		{

			foreach (var command in registry.Commands)
			{
				var handlers = registry.CommandHandlers.GetOrDefault(command);

				if (handlers.Any() == false)
				{
					throw new Exception(string.Format("No command handlers found for command {0},", command.FullName));
				}

				var wireMethod = typeof(IBus).GetMethod("Wire");
				var buildMethod = GetType().GetMethod("BuildAction", BindingFlags.NonPublic | BindingFlags.Instance);

				foreach (var handler in handlers)
				{
					var handlerInstance = container.GetInstance(handler);

					var action = buildMethod
						.MakeGenericMethod(command, handler)
						.Invoke(this, new[] { handlerInstance });

					wireMethod
						.MakeGenericMethod(command)
						.Invoke(bus, new[] { action });
				}
			}
		}

		private Action<TCommand> BuildAction<TCommand, THandler>(THandler handler)
			where TCommand : ICommand
			where THandler : ICommandHandler<TCommand>
		{
			return handler.Execute;
		}

	}
}
