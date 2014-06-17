using System;
using DapperExtensions;

namespace EntitiesLegacy
{
	public class EmailAddress : Entity
	{
		public string Email { get; set; }
		public Boolean IsPrimary { get; set; }

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
