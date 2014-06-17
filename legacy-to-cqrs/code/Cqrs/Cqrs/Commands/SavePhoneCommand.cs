using System;
using System.Data;
using DapperExtensions;

namespace Cqrs.Commands
{
	public class SavePhoneCommand
	{
		private readonly IDbConnection  _connection;
		private readonly Candidate _parent;
		private readonly Phone _phone;

		public SavePhoneCommand(IDbConnection  connection, Candidate parent, Phone phone)
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
