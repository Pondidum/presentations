using System;
using System.Data.SQLite;
using System.IO;

namespace DbDemo.Manage
{
	public class DatabaseStructure
	{
		public static void SetupDatabase()
		{
			if (File.Exists(Database.DbFile))
			{
				File.Delete(Database.DbFile);
			}

			SQLiteConnection.CreateFile(Database.DbFile);

			using (var connection = Database.OpenConnection())
			{

				var runCommand = new Action<SQLiteConnection, string>((con, text) =>
				{
					var c = con.CreateCommand();
					c.CommandText = text;
					c.ExecuteNonQuery();
				});

				runCommand(connection, "create table Addresses (id integer primary key autoincrement, line varchar(50), town varchar(50), county varchar(50), postcode varchar(10), type int, parentID int, parentType int)");

				runCommand(connection, "insert into addresses (line, town, county, postcode, type, parentID, parentType) " +
									   "values ('49 Dumas Drive', 'whiteley', 'hampshire', 'po14 7lu', 0, 0, 0)");

				runCommand(connection, "insert into addresses (line, town, county, postcode, type, parentID, parentType) " +
									   "values ('1450 Parkway, Whiteley Business Park', 'whiteley', 'hampshire', 'po15 7af', 0, 1, 0)");
			}

		}

		public static void Expand()
		{
			Database.ExecuteNonQuery("alter table addresses add Line1 varchar(50)");
			Database.ExecuteNonQuery("alter table addresses add Line2 varchar(50)");
		}

		public static void Collapse()
		{
			Database.ExecuteNonQuery("alter table addresses rename to addressesOld");
			Database.ExecuteNonQuery("create table Addresses (id integer primary key autoincrement, line1 varchar(50), line2 varchar(50), town varchar(50), county varchar(50), postcode varchar(10), type int, parentID int, parentType int)");
			
			Database.ExecuteNonQuery("insert into addresses (id, line1, line2, town, county, postcode, type, parentid, parenttype) " +
									 "select id, line1, line2, town, county, postcode, type, parentid, parenttype " +
									 "from addressesOld");

			Database.ExecuteNonQuery("drop table addressesOld");

		}
	}
}