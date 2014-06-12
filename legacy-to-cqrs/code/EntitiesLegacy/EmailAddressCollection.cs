using System;
using System.Collections.Generic;
using Dapper;

namespace EntitiesLegacy
{
	public class EmailAddressCollection : List<EmailAddress>
	{
		public Guid ParentID { get; set; }

		public void LoadByParentID()
		{
			using (var connection = DB.OpenConnection())
			{
				var emails = connection.Query<EmailAddress>(
					"select ID, candidateID, email, isPrimary where candidateID = @candidateID",
					new { candidateID = ParentID });

				Clear();
				AddRange(emails);
			}
		}

		public void Save()
		{
			ForEach(e =>
			{
				e.ParentID = ParentID;
				e.Save();
			});
		}
	}
}
