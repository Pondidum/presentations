﻿using System;
using System.Linq;
using StructureMap;

namespace EntitiesCqrsCommandHandler
{
	public class RouteBuilder
	{
		public RouteBuilder(IContainer container, IBus bus, CommandHandlerRegistry registry)
		{

			foreach (var command in registry.Commands)
			{
				var handlers = registry.CommandHandlers.FirstOrDefault(ch => ch.Key == command).Value;

				if (handlers == null)
				{
					throw new Exception(string.Format("No command handlers found for command {0},", command.FullName));
				}

				var wireMethod = typeof(IBus).GetMethod("Wire");
				var buildMethod = GetType().GetMethod("BuildAction");

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

		public Action<TCommand> BuildAction<TCommand, THandler>(THandler handler)
			where TCommand : ICommand
			where THandler : ICommandHandler<TCommand>
		{
			return handler.Execute;
		}

	}
}
