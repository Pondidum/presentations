using System;
using System.Data.Common;
using Dapper;

namespace EntitiesCqrs.Commands
{
	public class SavePhoneCommand : ICommand
	{
		private readonly DbConnection _connection;
		private readonly Candidate _parent;
		private readonly Phone _phone;

		public SavePhoneCommand(DbConnection connection, Candidate parent, Phone phone)
		{
			_connection = connection;
			_parent = parent;
			_phone = phone;
		}

		public void Execute()
		{
			_phone.ParentID = _parent.ID;

			if (_phone.ID == Guid.Empty)
			{
				_phone.ID = Guid.NewGuid();
				_connection.Execute(
					"insert into emails (ID, candidateID, number, extension) values (@id, @parentID, @number, @extension)",
					_phone);
			}
			else
			{
				_connection.Execute(
					"update emails set candidateID = @parentID, number = @number, extension = @extension where ID = @id",
					_phone);
			}
		}
	}
}
