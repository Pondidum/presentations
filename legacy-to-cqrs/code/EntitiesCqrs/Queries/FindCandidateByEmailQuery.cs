using System.Collections.Generic;
using System.Data;
using Dapper;
using EntitiesCqrs.Infrastructure;

namespace EntitiesCqrs.Queries
{
	public class FindCandidateByEmailQuery
	{
		private readonly IDbConnection  _connection;
		private readonly string _email;

		public FindCandidateByEmailQuery(IDbConnection  connection, string email)
		{
			_connection = connection;
			_email = email;
		}

		public IEnumerable<Candidate> Execute()
		{
			var arg = new { email = "%" + _email + "%" };

			return _connection
				.Query<Candidate>(
					"select ID, Name, Dob, Sex " +
					"from candidates " + 
					"join emails on e.parentID = c.ID " +
					"where e.email like @email", arg)
				.Apply(candidate => candidate.Emails.AddRange(new GetCandidateEmails(_connection, candidate).Execute()))
				.Apply(candidate => candidate.Phones.AddRange(new GetCandidatePhones(_connection, candidate).Execute()));
		}
	}
}
