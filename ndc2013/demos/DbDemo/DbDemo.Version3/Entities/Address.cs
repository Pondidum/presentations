using System;
using System.Data;
using System.Linq;
using System.Text;

namespace DbDemo.Version3.Entities
{
	public class Address
	{
		public int ID { get; protected set; }
		
		private String Line { get; set; }
		public String Line1 { get; set; }
		public String Line2 { get; set; }

		public String Town { get; set; }
		public String County { get; set; }
		public String Postcode { get; set; }

		public AddressType Type { get; set; }
		public int ParentID { get; set; }
		public EntityType ParentType { get; set; }

		public Address(IDataReader reader)
		{
			Read(reader);
		}

		private void Read(IDataReader reader)
		{
			ID = reader.AsInteger(DBFields.ID);
			Town = reader.AsString(DBFields.Town);
			County = reader.AsString(DBFields.County);
			Postcode = reader.AsString(DBFields.Postcode);

			Type = reader.AsEnum<AddressType>(DBFields.Type);
			ParentID = reader.AsInteger(DBFields.ParentID);
			ParentType = reader.AsEnum<EntityType>(DBFields.ParentType);

			Line = reader.AsString(DBFields.Line);
			Line1 = reader.AsString(DBFields.Line1);
			Line2 = reader.AsString(DBFields.Line2);

			if (string.IsNullOrWhiteSpace(Line1))
			{
				if (Line.Contains(','))
				{
					var lines = Line.Split(',');

					Line1 = lines.First();
					Line2 = String.Join(", ", lines.Skip(1));
				}
				else
				{
					Line1 = Line;
					Line2 = string.Empty;
				}
			}
		}

		public void Save()
		{
			var sql = new StringBuilder();

			sql.AppendLine("Update Addresses set");
			sql.AppendFormat("Line = '{0}',\n", Line);
			sql.AppendFormat("Line1 = '{0}',\n", Line1);
			sql.AppendFormat("Line2 = '{0}',\n", Line2);
			sql.AppendFormat("Town = '{0}',\n", Town);
			sql.AppendFormat("County = '{0}',\n", County);
			sql.AppendFormat("Postcode = '{0}',\n", Postcode);
			sql.AppendFormat("Type = {0},\n", (int)Type);
			sql.AppendFormat("ParentID = {0},\n", ParentID);
			sql.AppendFormat("ParentType = {0}\n", (int)ParentType);
			sql.AppendFormat("Where ID = {0}", ID);

			Database.ExecuteNonQuery(sql.ToString());
		}

		private class DBFields
		{
			public const string ID = "ID";
			public const string Line = "Line";
			public const string Line1 = "Line1";
			public const string Line2 = "Line2";
			public const string Town = "Town";
			public const string County = "County";
			public const string Postcode = "Postcode";
			public const string Type = "Type";
			public const string ParentID = "ParentID";
			public const string ParentType = "ParentType";
			
		}
	}
}
