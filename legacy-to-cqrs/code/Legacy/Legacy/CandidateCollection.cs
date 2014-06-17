using System.Collections.Generic;
using Dapper;

namespace Legacy
{
	public class CandidateCollection : List<Candidate>
	{
		public void LoadByName(string name)
		{
			using (var connection = DB.OpenConnection())
			{
				var candidates = connection.Query<Candidate>(
						"select ID, Name, Dob, Sex from candidates where name like @name",
						new { name = "%" + name + "%" });

				Clear();	
				AddRange(candidates);
			}
		}

		public void LoadByEmail(string email)
		{
			using (var connection = DB.OpenConnection())
			{
				var candidates = connection.Query<Candidate>(
						@"
						select c.ID, c.Name, c.Dob, c.Sex 
						from candidates c
						join emails on e.parentID = c.ID
						where e.email like @email",
						new { email = "%" + email + "%" });

				Clear();
				AddRange(candidates);
			}
		}
	}
}
