﻿using System.Collections.Generic;
using System.Data;
using Cqrs.Infrastructure;
using Dapper;

namespace Cqrs.Queries
{
	public class FindCandidateByNameQuery
	{
		private readonly IDbConnection  _connection;
		private readonly string _name;

		public FindCandidateByNameQuery(IDbConnection  connection, string name)
		{
			_connection = connection;
			_name = name;
		}

		public IEnumerable<Candidate> Execute()
		{
			var arg = new { name = "%" + _name + "%" };

			return _connection
				.Query<Candidate>("select ID, Name, Dob, Sex from candidates where name like @name", arg)
				.Apply(candidate => candidate.Emails.AddRange(new GetCandidateEmailsQuery(_connection, candidate).Execute()))
				.Apply(candidate => candidate.Phones.AddRange(new GetCandidatePhonesQuery(_connection, candidate).Execute()));
		}
	}
}