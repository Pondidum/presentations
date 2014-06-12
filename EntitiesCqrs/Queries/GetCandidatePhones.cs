using System.Collections.Generic;
using System.Data.Common;
using Dapper;

namespace EntitiesCqrs.Queries
{
	public class GetCandidatePhones
	{
		private readonly DbConnection _connection;
		private readonly Candidate _parent;

		public GetCandidatePhones(DbConnection connection, Candidate parent)
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
