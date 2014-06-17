using System.Data.Common;
using System.Data.SqlClient;

namespace Legacy
{
	public class DB
	{
		public static DbConnection OpenConnection()
		{
			return new SqlConnection("");
		}
	}
}
