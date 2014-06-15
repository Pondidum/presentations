using System.Collections.Generic;
using System.Data;
using Dapper;

namespace EntitiesCqrs.Queries
{
	public class GetCandidatePhones
	{
		private readonly IDbConnection  _connection;
		private readonly Candidate _parent;

		public GetCandidatePhones(IDbConnection  connection, Candidate parent)
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
