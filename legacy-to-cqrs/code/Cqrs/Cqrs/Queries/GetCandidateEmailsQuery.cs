using System.Collections.Generic;
using System.Data;
using Dapper;

namespace Cqrs.Queries
{
	public class GetCandidateEmailsQuery
	{
		private readonly IDbConnection  _connection;
		private readonly Candidate _parent;

		public GetCandidateEmailsQuery(IDbConnection  connection, Candidate parent)
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
