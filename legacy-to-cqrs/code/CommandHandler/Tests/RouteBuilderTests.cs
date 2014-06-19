using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CommandHandler;
using CommandHandler.Commands;
using CommandHandler.Entities;
using NSubstitute;
using StructureMap;
using StructureMap.Graph;
using Xunit;
using Xunit.Sdk;

namespace Tests
{
	public class RouteBuilderTests
	{
		private readonly Container _container;

		public RouteBuilderTests()
		{
			_container = new Container(config => config.Scan(scan =>
			{
				scan.TheCallingAssembly();
				scan.WithDefaultConventions();
			}));
		}

		private ICommandHandlerRegistry RegisteryFor(Type command, IEnumerable<Type> handlers)
		{
			var registry = Substitute.For<ICommandHandlerRegistry>();

			registry.Commands.Returns(new[] { command });
			registry.CommandHandlers.Returns(new Dictionary<Type, IList<Type>>()
			{
				{ command, handlers.ToList() }
			});

			return registry;
		}

		[Fact]
		public void When_registering_a_command_to_a_single_handler()
		{
			var bus = new InMemoryBus();
			var registry = RegisteryFor(typeof(TestCommand), new[] { typeof(TestCommandHandler) });

			var builder = new RouteBuilder(_container, bus, registry);

			var command = new TestCommand { First = "One", Second = "Two" };

			bus.Publish(command);

			Assert.Equal("One:Two", command.Result);
		}

		[Fact]
		public void When_registering_a_command_to_multiple_handlers()
		{
			var bus = new InMemoryBus();
			var registry = RegisteryFor(typeof(TestCommand), new[] { typeof(TestCommandHandler), typeof(SecondTestCommandHandler) });

			var builder = new RouteBuilder(_container, bus, registry);

			var command = new TestCommand { First = "One", Second = "Two" };

			bus.Publish(command);

			Assert.Equal(2, command.Count);
		}

		[Fact]
		public void When_there_is_no_handler_for_a_command()
		{
			var bus = new InMemoryBus();
			var registry = RegisteryFor(typeof(TestCommand), Enumerable.Empty<Type>());

			Assert.Throws<Exception>(() => new RouteBuilder(_container, bus, registry));
		}


		private class TestCommand : ICommand
		{
			public string First { get; set; }
			public string Second { get; set; }
			public string Result { get; set; }
			public int Count { get; set; }
		}

		private class TestCommandHandler : ICommandHandler<TestCommand>
		{
			public void Execute(TestCommand command)
			{
				command.Result = command.First + ":" + command.Second;
				command.Count++;
			}
		}

		private class SecondTestCommandHandler : ICommandHandler<TestCommand>
		{
			public void Execute(TestCommand command)
			{
				command.Count++;
			}
		}
	}
}