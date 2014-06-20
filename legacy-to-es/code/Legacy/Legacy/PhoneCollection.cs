using System;
using System.Collections.Generic;
using Dapper;

namespace Legacy
{
	public class PhoneCollection : List<Phone>
	{
		public Guid ParentID { get; set; }

		public void LoadByParentID()
		{
			using (var connection = DB.OpenConnection())
			{
				var phones = connection.Query<Phone>(
					"select ID, candidateID, number, extension from phones where candidateID = @candidateID",
					new { candidateID = ParentID });

				Clear();
				AddRange(phones);
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
