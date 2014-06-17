using System;
using System.Data;
using System.Linq;
using Cqrs.Infrastructure;
using DapperExtensions;

namespace Cqrs.Commands
{
	public class SaveCandidateCommand
	{
		private readonly IDbConnection  _connection;
		private readonly Candidate _candidate;

		public SaveCandidateCommand(IDbConnection  connection, Candidate candidate)
		{
			_connection = connection;
			_candidate = candidate;
		}

		public void Execute()
		{
			if (_candidate.ID == Guid.Empty)
			{
				_candidate.ID = Guid.NewGuid();
				_connection.Insert(_candidate);
			}
			else
			{
				_connection.Update(_candidate);
			}

			_candidate.Phones
				.Select(p => new SavePhoneCommand(_connection, _candidate, p))
				.Each(command => command.Execute());

			_candidate.Emails
				.Select(e => new SaveEmailCommand(_connection, _candidate, e))
				.Each(command => command.Execute());
		}
	}
}
