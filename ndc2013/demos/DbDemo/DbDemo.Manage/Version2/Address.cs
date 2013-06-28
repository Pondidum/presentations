using System;
using System.Data;
using System.Text;
using DbDemo.Manage.Entities;

namespace DbDemo.Manage.Version2
{
	public class Address
	{
		public int ID { get; protected set; }
		public String Line { get; set; }
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
			Line = reader.AsString(DBFields.Line);
			Town = reader.AsString(DBFields.Town);
			County = reader.AsString(DBFields.County);
			Postcode = reader.AsString(DBFields.Postcode);

			Type = reader.AsEnum<AddressType>(DBFields.Type);
			ParentID = reader.AsInteger(DBFields.ParentID);
			ParentType = reader.AsEnum<EntityType>(DBFields.ParentType);
		}
		
		public void Save()
		{
			var sql = new StringBuilder();

			sql.AppendLine("Update Addresses set");
			sql.AppendFormat("Line = '{0}',\n", Line);
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
			public const string Town = "Town";
			public const string County = "County";
			public const string Postcode = "Postcode";
			public const string Type = "Type";
			public const string ParentID = "ParentID";
			public const string ParentType = "ParentType";
		}
	}
}
