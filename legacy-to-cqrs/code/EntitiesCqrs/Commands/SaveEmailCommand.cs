using System;
using System.Data.Common;
using DapperExtensions;

namespace EntitiesCqrs.Commands
{
	public class SaveEmailCommand
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
				_connection.Insert(_email);
			}
			else
			{
				_connection.Update(_email);
			}
		}
	}
}
