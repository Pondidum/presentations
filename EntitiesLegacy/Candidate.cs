using System;
using System.Collections.Generic;
using Dapper;

namespace EntitiesLegacy
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
					connection.Execute(
						"insert into candidates (ID, Name, Dob, Sex) values (@id, @name, @dob, @sex)", 
						this);
				}
				else
				{
					connection.Execute(
						"update candidates set Name = @name, Dob = @dob, Sex = @sex where ID = @id", 
						this);
				}
			}

			Emails.ParentID = ID;
			Emails.Save();

			Phones.ParentID = ID;
			Phones.Save();
		}
	}
}
