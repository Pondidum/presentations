using System;
using System.Data;
using DapperExtensions;
using EntitiesCqrsCommandHandler.Commands;
using EntitiesCqrsCommandHandler.Entities;
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

			Upsert(candidate);

			candidate
				.Phones
				.Apply(phone => phone.ParentID = candidate.ID)
				.Each(Upsert);

			candidate
				.Emails
				.Apply(email => email.ParentID = candidate.ID)
				.Each(Upsert);
		}

		private void Upsert(IKeyed entity)
		{
			if (entity.ID == Guid.Empty)
			{
				entity.ID = Guid.NewGuid();
				_connection.Insert(entity);
			}
			else
			{
				_connection.Update(entity);
			}
		}
	}
}
