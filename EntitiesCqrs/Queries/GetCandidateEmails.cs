using System.Collections.Generic;
using System.Data.Common;
using Dapper;

namespace EntitiesCqrs.Queries
{
	public class GetCandidateEmails
	{
		private readonly DbConnection _connection;
		private readonly Candidate _parent;

		public GetCandidateEmails(DbConnection connection, Candidate parent)
		{
			_connection = connection;
			_parent = parent;
		}

		public IEnumerable<EmailAddress> Execute()
		{
			return _connection.Query<EmailAddress>(
				"select ID, candidateID, email, isPrimary where candidateID = @candidateID",
				new { candidateID = _parent.ID });
		}
	}
}
