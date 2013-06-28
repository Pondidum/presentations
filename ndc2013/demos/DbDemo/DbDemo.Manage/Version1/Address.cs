using System;
using System.Data;
using System.Text;
using DbDemo.Manage.Entities;

namespace DbDemo.Manage.Version1
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
			ID = reader.AsInteger((int)DBFields.ID);
			Line = reader.AsString((int)DBFields.Line);
			Town = reader.AsString((int)DBFields.Town);
			County = reader.AsString((int)DBFields.County);
			Postcode = reader.AsString((int)DBFields.Postcode);

			Type = reader.AsEnum<AddressType>((int)DBFields.Type);
			ParentID = reader.AsInteger((int)DBFields.ParentID);
			ParentType = reader.AsEnum<EntityType>((int)DBFields.ParentType);
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

		private enum DBFields
		{
			ID,
			Line,
			Town,
			County,
			Postcode,
			Type,
			ParentID,
			ParentType,
		}
	}
}
