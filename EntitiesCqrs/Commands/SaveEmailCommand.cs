using System;
using System.Data.Common;
using Dapper;

namespace EntitiesCqrs.Commands
{
	public class SaveEmailCommand : ICommand
	{
		private readonly DbConnection _connection;
		private readonly Candidate _parent;
		private readonly EmailAddress _email;

		public SaveEmailCommand(DbConnection connection, Candidate parent, EmailAddress email)
		{
			_connection = connection;
			_parent = parent;
			_email = email;
		}

		public void Execute()
		{
			_email.ParentID = _parent.ID;

			if (_email.ID == Guid.Empty)
			{
				_email.ID = Guid.NewGuid();
				_connection.Execute(
					"insert into emails (ID, candidateID, email, isPrimary) values (@id, @parentID, @email, @isPrimary)",
					_email);
			}
			else
			{
				_connection.Execute(
					"update emails set candidateID = @parentID, email = @email, isPrimary = @isPrimary where ID = @id",
					_email);
			}
		}
	}
}
