using System.Data.Common;
using System.Data.SqlClient;

namespace Entities
{
	public class DB
	{
		public static DbConnection OpenConnection()
		{
			return new SqlConnection("");
		}
	}
}
