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

			other.Emails.ParentID = main.ID;
			other.Emails.Save();
			//interesting...
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
