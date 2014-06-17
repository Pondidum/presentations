using System.Data;
using System.Data.SqlClient;
using CommandHandler;
using CommandHandler.Commands;
using CommandHandler.Entities;
using StructureMap;
using Xunit;

namespace Tests
{
	public class RouteBuilderTests
	{
		[Fact]
		public void When_creating_all_routes()
		{
			var container = new Container(config =>
			{
				config.Scan(scanner =>
				{
					scanner.AssemblyContainingType<ICommand>();
					scanner.WithDefaultConventions();
				});

				config.For<IDbConnection>().Use(() => new SqlConnection());
			});

			var bus = new InMemoryBus();
			var builder = new RouteBuilder(container, bus, new CommandHandlerRegistry());

			bus.Publish(new SaveCandidateCommand(new Candidate()));

		} 
	}
}