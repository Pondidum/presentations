using System.Collections.Generic;
using System.Data;
using Dapper;

namespace Cqrs.Queries
{
	public class GetCandidatePhonesQuery
	{
		private readonly IDbConnection  _connection;
		private readonly Candidate _parent;

		public GetCandidatePhonesQuery(IDbConnection  connection, Candidate parent)
		{
			_connection = connection;
			_parent = parent;
		}

		public IEnumerable<Phone> Execute()
		{
			return _connection.Query<Phone>(
				"select ID, candidateID, number, extension from phones where candidateID = @candidateID",
				new { candidateID = _parent.ID });
		}
	}
}
