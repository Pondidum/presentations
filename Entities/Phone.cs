using System;
using Dapper;

namespace Entities
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
					connection.Execute(
						"insert into emails (ID, candidateID, number, extension) values (@id, @parentID, @number, @extension)",
						this);
				}
				else
				{
					connection.Execute(
						"update emails set candidateID = @parentID, number = @number, extension = @extension where ID = @id",
						this);
				}
			}
		}
	}
}
