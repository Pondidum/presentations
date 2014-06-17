using System;
using DapperExtensions;

namespace Legacy
{
	public class Candidate : Entity
	{
		public string Name { get; set; }
		public DateTime DoB { get; set; }
		public Sexes Sex { get; set; }

		public EmailAddressCollection Emails { get; private set; }
		public PhoneCollection Phones { get; private set; }

		public Candidate()
		{
			Emails = new EmailAddressCollection();
			Phones = new PhoneCollection();
		}

		public override void Save()
		{
			using (var connection = DB.OpenConnection())
			{
				if (ID == Guid.Empty)
				{
					ID = Guid.NewGuid();
					connection.Insert(this);
				}
				else
				{
					connection.Update(this);
				}
			}

			Emails.ParentID = ID;
			Emails.Save();

			Phones.ParentID = ID;
			Phones.Save();
		}
	}
}
