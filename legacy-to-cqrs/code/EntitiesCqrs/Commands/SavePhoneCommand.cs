using System;
using System.Data.Common;
using DapperExtensions;

namespace EntitiesCqrs.Commands
{
	public class SavePhoneCommand
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
				_connection.Insert(_phone);
			}
			else
			{
				_connection.Update(_phone);
			}
		}
	}
}
