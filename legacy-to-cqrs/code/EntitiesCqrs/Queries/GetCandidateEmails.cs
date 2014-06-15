using System.Collections.Generic;
using System.Data;
using Dapper;

namespace EntitiesCqrs.Queries
{
	public class GetCandidateEmails
	{
		private readonly IDbConnection  _connection;
		private readonly Candidate _parent;

		public GetCandidateEmails(IDbConnection  connection, Candidate parent)
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
