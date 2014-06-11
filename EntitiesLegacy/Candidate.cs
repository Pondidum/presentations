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

		public List<EmailAddress> Emails { get; private set; }
		public List<Phone> Phones { get; private set; }

		public override void Save()
		{
			using (var connection = DB.OpenConnection())
			{
				if (ID == Guid.Empty)
				{
					ID = Guid.NewGuid();
					connection.Execute("insert into candidates (ID, Name, Dob, Sex) values (@id, @name, @dob, @sex)", 
						this);
				}
				else
				{
					connection.Execute(
						"update candidates set Name = @name, Dob = @dob, Sex = @sex where ID = @id", 
						this);
				}
			}

			Emails.ForEach(e =>
			{
				e.ParentID = ID;
				e.Save();
			});

			Phones.ForEach(p =>
			{
				p.ParentID = ID;
				p.Save();
			});
		}
	}
}
