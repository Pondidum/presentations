using System.Collections.Generic;

namespace DbDemo.Version3.Entities
{
	public class AddressCollection : List<Address>
	{
		private const string Sql = "select id, line, town, county, postcode, type, parentID, parentType, line1, line2 from addresses";

		public void Load()
		{
			Clear();

			using (var reader = Database.ExecuteReader(Sql))
			{
				while (reader.Read())
				{
					Add(new Address(reader));
				}
			}
		}
	}
}