using System.Data;
using EntitiesCqrsCommandHandler.Commands;
using EntitiesCqrsCommandHandler.Infrastructure;

namespace EntitiesCqrsCommandHandler.CommandHandlers
{
	public class SaveCandidateCommandHandler : ICommandHandler<SaveCandidateCommand>
	{
		private readonly IDbConnection _connection;

		public SaveCandidateCommandHandler(IDbConnection connection)
		{
			_connection = connection;
		}

		public void Execute(SaveCandidateCommand command)
		{
			var candidate = command.Candidate;

			_connection.Upsert(candidate);

			candidate
				.Phones
				.Apply(phone => phone.ParentID = candidate.ID)
				.Each(phone => _connection.Upsert(phone));

			candidate
				.Emails
				.Apply(email => email.ParentID = candidate.ID)
				.Each(email => _connection.Upsert(email));
		}
	}
}
