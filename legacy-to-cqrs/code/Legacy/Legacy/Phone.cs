using System;
using DapperExtensions;

namespace Legacy
{
	public class Phone : Entity
	{
		public string Number { get; set; }
		public string Extension { get; set; }

		public override void Save()
		{
			using (var connection = DB.OpenConnection())
			{
				if (ID == Guid.Empty)
				{
					ID = Guid.NewGuid();
					connection.Insert(this);
				}
				else
				{
					connection.Update(this);
				}
			}
		}
	}
}
