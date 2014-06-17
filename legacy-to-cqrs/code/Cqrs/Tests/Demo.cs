﻿using System.Data.SqlClient;
using Cqrs;
using Cqrs.Commands;
using Cqrs.Queries;

namespace Tests
{
	public class Demo
	{
		public void Forces_db_connection()
		{
			var connection = new SqlConnection();
			var query = new FindCandidateByNameQuery(connection, "*@gmail.com");

			var candidates = query.Execute();
		}

		public void Has_transaction_boundaries()
		{
			var c = new Candidate();

			c.Emails.Add(new EmailAddress { Email = "one@gmail.com" });
			c.Emails.Add(new EmailAddress { Email = "two@gmail.com" });

			using (var connection = new SqlConnection())
			{
				using (var tx = connection.BeginTransaction())
				{
					var command = new SaveCandidateCommand(connection, c);
					command.Execute();

					tx.Commit();
				}
			}
		}
	}
}