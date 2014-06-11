using System;
using Dapper;

namespace Entities
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
					connection.Execute(
						"insert into emails (ID, candidateID, email, isPrimary) values (@id, @parentID, @email, @isPrimary)", 
						this);
				}
				else
				{
					connection.Execute(
						"update emails set candidateID = @parentID, email = @email, isPrimary = @isPrimary where ID = @id", 
						this);
				}
			}
		}
	}
}
