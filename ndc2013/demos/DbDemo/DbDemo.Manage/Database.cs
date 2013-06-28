using System;
using System.Data;
using System.Data.SQLite;

namespace DbDemo.Manage
{
	public class Database
	{
		public const string DbFile = "db.sqlite";

		public static SQLiteConnection OpenConnection()
		{
			var connection = new SQLiteConnection("Data Source=db.sqlite;Version=3;");

			connection.Open();

			return connection;
		}

		public static void ExecuteReader(string commandText, Action<IDataReader> action)
		{
            using (var connection = OpenConnection())
			{
				var command = connection.CreateCommand();
				command.CommandText = commandText;

				using (var reader = command.ExecuteReader())
                {
                    action(reader);
                }
			}
		}

		public static void ExecuteNonQuery(string commandText)
		{
			using (var connection = OpenConnection())
			{
				var command = connection.CreateCommand();
				command.CommandText = commandText;
				command.ExecuteNonQuery();
			}
		}
	}
}