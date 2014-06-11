using System;
using System.Data.Common;
using System.Linq;
using Dapper;
using EntitiesCqrs.Infrastructure;

namespace EntitiesCqrs.Commands
{
	public class SaveCandidateCommand : ICommand
	{
		private readonly DbConnection _connection;
		private readonly Candidate _candidate;

		public SaveCandidateCommand(DbConnection connection, Candidate candidate)
		{
			_connection = connection;
			_candidate = candidate;
		}

		public void Execute()
		{
			if (_candidate.ID == Guid.Empty)
			{
				_candidate.ID = Guid.NewGuid();
				_connection.Execute(
					"insert into candidates (ID, Name, Dob, Sex) values (@id, @name, @dob, @sex)",
					_candidate);	
			}
			else
			{
				_connection.Execute(
					"update candidates set Name = @name, Dob = @dob, Sex = @sex where ID = @id",
					this);
			}

			_candidate
				.Phones
				.Select(p => new SavePhoneCommand(_connection, _candidate, p))
				.Each(command => command.Execute());

			_candidate
				.Emails
				.Select(e => new SaveEmailCommand(_connection, _candidate, e))
				.Each(command => command.Execute());
		}
	}
}
