using System.Data;
using System.Data.SQLite;

namespace DbDemo.Manage.Entities
{
	public class Database
	{
		private const string DbFile = "db.sqlite";

		private static SQLiteConnection OpenConnection()
		{
			var connection = new SQLiteConnection("Data Source=db.sqlite;Version=3;");

			connection.Open();

			return connection;
		}

		public static IDataReader ExecuteReader(string commandText)
		{
			var connection = OpenConnection();

			var command = connection.CreateCommand();
			command.CommandText = commandText;

			return command.ExecuteReader();
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