using System;
using System.Linq;
using Legacy;

namespace Tests
{
	public class Demo
	{
		public void No_db_setup_done()
		{
			//oops forgot to setup the db...

			var candidates = new CandidateCollection();
			candidates.LoadByEmail("*@gmail.com");

		}

		public void Candidate_collections_mutable()
		{
			var candidates = new CandidateCollection();
			candidates.LoadByEmail("*@gmail.com");

			var main = candidates[0];
			var other = candidates[1];

			//I wonder what the implication of this is...
			other.Emails.ParentID = main.ID;
			other.Emails.Save();
		}

		public void You_can_load_by_anything()
		{
			var candidates = new CandidateCollection();
			var candidate = candidates.First();

			//eek
			candidate.Emails.ParentID = Guid.NewGuid();
			candidate.Emails.LoadByParentID();
		}

		public void Not_efficient()
		{
			var c = new Candidate();

			c.Emails.Add(new EmailAddress() { Email = "one@gmail.com"});
			c.Emails.Add(new EmailAddress() { Email = "two@gmail.com" });

			c.Save();
			//3 db connections opened and closed
			//no transaction boundary available
		}

	}
}
