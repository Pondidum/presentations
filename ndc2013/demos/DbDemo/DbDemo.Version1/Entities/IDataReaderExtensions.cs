using System;
using System.Data;

namespace DbDemo.Version1.Entities
{
	public static class IDataReaderExtensions
	{
		 public static int AsInteger(this IDataReader reader, int column)
		 {
			 if (reader == null) throw new ArgumentNullException("reader");

			 return Convert.ToInt32(reader.GetValue(column));
		 }

		public static string AsString(this IDataReader reader, int column)
		{
			if (reader == null ) throw new ArgumentNullException("reader");

			return Convert.ToString(reader.GetValue(column));
		}

		public static T AsEnum<T>(this IDataReader reader, int column)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			return (T) Enum.ToObject(typeof(T), reader.AsInteger(column));
		}



		public static int AsInteger(this IDataReader reader, string column)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			return Convert.ToInt32(reader[column]);
		}

		public static string AsString(this IDataReader reader, string column)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			return Convert.ToString(reader[column]);
		}

		public static T AsEnum<T>(this IDataReader reader, string column)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			return (T)Enum.ToObject(typeof(T), reader[column]);
		}


	}
}
